using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UpgradeType { damage, miningSpeed, shipSpeed, shield }
public enum RepairType { drill, drive, shield }
[RequireComponent(typeof(InputController))]
public class PlayerController : MovementBase
{
    public static PlayerController Instance;

    [Header("Resource Collection")]
    [SerializeField] private LineRenderer resourceIndicator;
    [SerializeField] private float maxResourceDist;
    private Resource targetResource = null;
    [SerializeField] private float miningSpeed;
    private float nextMine;
    [SerializeField] private float baseMiningStrength = 1;
    [SerializeField] private Color closeColor;
    [SerializeField] private Color farColor;
    public Dictionary<ResourceType, int> inventory = new Dictionary<ResourceType, int>();

    [Header("Warping")]
    public int uraniumPerWarp;

    [Header("Upgrades")]
    [SerializeField] private int maxUpgrade;
    public int upgradeCost;
    [SerializeField] private int upgradeCostMod;
    [SerializeField] private float damageMod;
    [SerializeField] private float miningSpeedMod;
    [SerializeField] private float shipSpeedMod;
    [SerializeField] private float shieldHealthMod;
    [SerializeField] private float shieldRateMod;
    public Dictionary<UpgradeType, int> upgrades = new Dictionary<UpgradeType, int>();

    [Header("Repairs")]
    public int maxRepair = 3;
    [HideInInspector] public int drillLevel;
    [SerializeField] private float miningPenalty;
    [HideInInspector] public int driveLevel;
    public int drivePenalty;
    [HideInInspector] public int shieldLevel;
    [SerializeField] private float shieldPenalty;

    // Adam is lazy
    private float baseShieldDelay;
    private float baseShieldRate;
    private float baseShieldHealth;

    private InputController inp;
    [HideInInspector] public Shield shield;

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

        inventory.Add(ResourceType.Metal, 0);
        inventory.Add(ResourceType.Electronics, 0);
        inventory.Add(ResourceType.Uranium, 0);

        upgrades.Add(UpgradeType.damage, 0);
        upgrades.Add(UpgradeType.miningSpeed, 0);
        upgrades.Add(UpgradeType.shipSpeed, 0);
        upgrades.Add(UpgradeType.shield, 0);

        targetTag = Constants.EnemyTag;

        drillLevel = maxRepair;
        driveLevel = maxRepair;
        shieldLevel = maxRepair;

        inp = GetComponent<InputController>();
    }

    protected override void Start()
    {
        base.Start();

        shield = GetComponentInChildren<Shield>();
        if (shield)
        {
            baseShieldDelay = shield.shieldRechargeDelay;
            baseShieldHealth = shield.maxHealth;
            baseShieldRate = shield.shieldRechargeRate;
        }
    }

    private void Update()
    {

        if (GameController.Instance.paused)
            return;

        HandleCombat();
        HandleInput();

        CheckMining();

        if (shield)
        {
            shield.maxHealth = baseShieldHealth + upgrades[UpgradeType.shield] * shieldHealthMod;
            shield.shieldRechargeRate = baseShieldRate + upgrades[UpgradeType.shield] * shieldRateMod;
            shield.shieldRechargeDelay = baseShieldDelay + (maxRepair - shieldLevel) * shieldPenalty;
            if (shieldLevel <= 0)
                shield.shieldRechargeDelay = -1;
        }
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

        AddForce(new Vector3(x, 0, y), (moveSpeed + upgrades[UpgradeType.shipSpeed] * shipSpeedMod));
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

        if (inp.interact.down && drillLevel > 0)
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
            nextMine = Time.time + miningSpeed + (maxRepair - drillLevel) * miningPenalty;
            targetResource.Damage(gameObject, baseMiningStrength + upgrades[UpgradeType.miningSpeed] * miningSpeedMod);
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
    
    #region Upgrades
    public bool UpgradeShip(UpgradeType upgrade)
    {
        if (inventory[ResourceType.Electronics] < upgradeCost)
            return false;

        if (upgrades[upgrade] == maxUpgrade)
            return false;

        upgrades[upgrade]++;

        inventory[ResourceType.Electronics] -= upgradeCost;
        upgradeCost += upgradeCostMod;

        return true;
    }
    #endregion

    #region Repairs
    public void CheckForBreak()
    {
        int RandomBreak() { return Random.Range(0, 1000) < 300 + 700 * (-Mathf.Pow(GameController.Instance.completedLevels, 0.2f) + 1.99f) ? 0 : 1; }

        if (drillLevel > 0)
            drillLevel -= RandomBreak();
        if (driveLevel > 0)
            driveLevel -= RandomBreak();
        if (shieldLevel > 0)
            shieldLevel -= RandomBreak();
    }

    public bool RepairShip(RepairType repair)
    {
        if (inventory[ResourceType.Metal] < 5)
            return false;

        switch (repair)
        {
            case RepairType.drill:
                if (drillLevel >= maxRepair)
                    return false;
                drillLevel += 1;
                break;
            case RepairType.drive:
                if (driveLevel >= maxRepair)
                    return false;
                driveLevel += 1;
                break;
            case RepairType.shield:
                if (shieldLevel >= maxRepair)
                    return false;
                shieldLevel += 1;
                break;
        }

        inventory[ResourceType.Metal] -= 5;

        return true;
    }
    #endregion
    
    public void AddResource(ResourceType resource, int amount)
    {
        inventory[resource] += amount;
    }

    public bool CheckWarp(bool doWarp)
    {
        if (inventory[ResourceType.Uranium] < uraniumPerWarp + (maxRepair - driveLevel) * drivePenalty || driveLevel <= 0)
            return false;

        if (doWarp)
            inventory[ResourceType.Uranium] -= uraniumPerWarp + (maxRepair - driveLevel) * drivePenalty;
        return true;
    }

    protected override Projectile GetBullet()
    {
        Projectile bullet = base.GetBullet();
        bullet.SetDamage(bulletDamage + upgrades[UpgradeType.damage] * damageMod);
        return bullet;
    }

    protected override void Death(GameObject source)
    {
        GameController.Instance.SetPause(true, false);
        LevelUI.Instance.ToggleMenu(Menu.death);
    }
}
