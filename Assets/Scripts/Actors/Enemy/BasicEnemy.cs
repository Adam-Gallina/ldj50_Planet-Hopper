using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MovementBase
{
    [Header("AI")]
    [SerializeField] protected float distFromEnemies;
    [SerializeField] protected float moveSpeedFromEnemies;
    [SerializeField] protected float attackDist;
    private float dodgeDir;
    [SerializeField] private float dodgeDist = 3;

    [Header("Slow spawn")]
    [SerializeField] protected float minSpawnTime;
    [SerializeField] protected float maxSpawnTime;
    protected float spawnEnd;
    protected bool spawned;

    [Header("Spawn on death")]
    [SerializeField] protected GameObject deathDropPrefab;
    [SerializeField] protected float deathForce;
    [SerializeField] protected float deathRotation;


    protected override void Awake()
    {
        base.Awake();

        dodgeDir = Random.Range(0, 2) == 1 ? -1 : 1;

        spawnEnd = Time.time + Random.Range(minSpawnTime, maxSpawnTime);
        nextShot = spawnEnd + Random.Range(0f, fireSpeed) * 4;
    }

    protected override void Start()
    {
        base.Start();
        targetTag = Constants.PlayerTag;
    }

    private void Update()
    {
        if (GameController.Instance.paused)
            return;

        if (!spawned && Time.time > spawnEnd)
            spawned = true;

        UpdateDirection();

        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < attackDist)
            Fire();
    }

    protected virtual void FixedUpdate()
    {
        HandleMovement();
    }

    protected virtual void UpdateDirection()
    {
        transform.forward = PlayerController.Instance.transform.position - transform.position;
    }

    protected override void HandleMovement()
    {
        Vector3 toPlayer = (PlayerController.Instance.transform.position + transform.right * dodgeDir * dodgeDist - transform.position);

        if (spawned)
            AddForce(toPlayer, moveSpeed);
        else
            AddForce(toPlayer, moveSpeed * -0.5f);

        Collider[] colls = Physics.OverlapSphere(transform.position, distFromEnemies, 1 << Constants.EnemyLayer);
        foreach (Collider c in colls)
        {
            AddForce(transform.position - c.transform.position, moveSpeedFromEnemies, ForceMode.Force);
        }
    }

    protected override void Death(GameObject source)
    {
        GameObject obj = Instantiate(deathDropPrefab);
        obj.transform.position = transform.position;
        obj.transform.localScale = transform.localScale;

        Rigidbody oRb = obj.GetComponent<Rigidbody>();
        oRb.velocity = ((transform.position - PlayerController.Instance.transform.position).normalized * deathForce + rb.velocity) / 2;
        oRb.AddTorque(transform.forward * Random.Range(0, deathRotation) + transform.right * Random.Range(0, deathRotation), ForceMode.VelocityChange);
        
        Stats.Instance.enemiesDestroyed++;

        base.Death(source);
    }
}
