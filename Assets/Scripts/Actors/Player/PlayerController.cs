using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(InputController))]
[RequireComponent(typeof(PlayerManager))]
public class PlayerController : CombatBase
{
    public static PlayerController Instance;
    [HideInInspector] public PlayerManager manager;

    [Header("Resource Collection")]
    [SerializeField] private LineRenderer resourceIndicator;
    [SerializeField] private float maxResourceDist;
    private Resource targetResource = null;
    [SerializeField] private float miningSpeed;
    private float nextMine;
    [SerializeField] private float baseMiningStrength = 1;
    [SerializeField] private Color closeColor;
    [SerializeField] private Color farColor;

    [Header("Warping")]
    public int uraniumPerWarp;

    private InputController inp;

    protected override void Awake()
    {
        base.Awake();

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        targetTag = Constants.EnemyTag;

        inp = GetComponent<InputController>();
        manager = GetComponent<PlayerManager>();
    }

    private void Update()
    {

        if (GameController.Instance.paused)
            return;

        HandleCombat();
        HandleInput();

        CheckMining();

    }

    protected override void HandleMovement()
    {
        int x = 0, y = 0;

        if (inp.left)
            x -= 1;
        if (inp.right)
            x += 1;

        if (inp.up)
            y += 1;
        if (inp.down)
            y -= 1;

        AddForce(new Vector3(x, 0, y), moveSpeed + manager.shipSpeedMod);
    }

    private void HandleCombat()
    {

        if (inp.fire)
            Fire();
    }

    private void HandleInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane xz = new Plane(Vector3.up, Vector3.zero);
        xz.Raycast(ray, out float distance);
        transform.forward = ray.GetPoint(distance) - transform.position;

        Resource target = CheckForResources();

        SetResourceIndicator(target?.transform);

        if (inp.interact.down && manager.miningPenalty.level < manager.maxRepair)
        {
            targetResource = target;
            LevelUI.Instance.SetMiningIndicator(target);
        }

        if (inp.warp.down)
        {
            if (CheckWarp(true))
                GameController.Instance.NextLevel();
        }
    }

    #region Mining

    private void CheckMining()
    {
        if (!targetResource || Vector3.Distance(targetResource.transform.position, transform.position + Constants.ToGround) > maxResourceDist)
            return;

        SetResourceIndicator(targetResource.transform);

        if (Time.time > nextMine)
        {
            nextMine = Time.time + miningSpeed + manager.miningPenalty;
            targetResource.Damage(gameObject, baseMiningStrength + manager.miningSpeedMod);
        }
    }

    private Resource CheckForResources()
    {
        Collider[] available = Physics.OverlapSphere(transform.position + Constants.ToGround, maxResourceDist, 1 << Constants.ResourceLayer);

        Collider closest = null;
        float closestDist = float.MaxValue;

        foreach (Collider c in available)
        {
            float dist = Vector3.Distance(c.transform.position, transform.position);
            if (dist < closestDist)
            {
                closest = c;
                closestDist = dist;
            }
        }

        return closest?.gameObject.GetComponentInParent<Resource>();
    }

    private void SetResourceIndicator(Transform target)
    {
        resourceIndicator.gameObject.SetActive(target);

        if (target)
        {
            float distPercent = Vector3.Distance(transform.position + Constants.ToGround, target.transform.position) / maxResourceDist;
            Color col = closeColor + (farColor - closeColor) * distPercent;

            resourceIndicator.startColor = col;
            resourceIndicator.endColor = col;
            resourceIndicator.SetPositions(new Vector3[] { transform.position, target.transform.position });
        }
    }

    #endregion
    

    public bool CheckWarp(bool doWarp)
    {
        if (manager.inventory[ResourceType.Uranium] < uraniumPerWarp + manager.drivePenalty || manager.drivePenalty.level >= manager.maxRepair)
            return false;

        if (doWarp)
            manager.inventory[ResourceType.Uranium] -= uraniumPerWarp + (int)manager.drivePenalty;
        return true;
    }

    protected override void InitBullet(Projectile bullet)
    {
        bullet.Initialize(gameObject, bulletSpeed, bulletDamage + manager.damageMod, targetTag);
    }

    protected override void Death(GameObject source)
    {
        GameController.Instance.SetPause(true, false);
        LevelUI.Instance.ToggleMenu(Menu.death);
    }
}
