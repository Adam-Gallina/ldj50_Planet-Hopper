using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatBase : MovementBase
{
    private List<Projectile> bulletPool;

    [Header("Combat")]
    [SerializeField] private Transform[] bulletSource;
    private int currSource = 0;
    [SerializeField] private BulletType bulletType = BulletType.Enemy;
    [SerializeField] protected float fireSpeed;
    protected float nextShot = 0;
    [SerializeField] protected float bulletSpeed = 60;
    [SerializeField] protected float bulletDamage = 1;
    [SerializeField] protected WeaponModBase weapon = new WeaponModBase();
    [SerializeField] private float bulletOffset;

    //[SerializeField] private Transform specialSource;

    [HideInInspector] public string targetTag;

    protected override void Start()
    {
        base.Start();

        bulletPool = new List<Projectile>();
    }

    protected virtual void Fire()
    {
        if (fireSpeed != 0 && Time.time >= nextShot)
        {
            nextShot = Time.time + fireSpeed;

            List<ProjectileStats> bullets = weapon.GetBullets(GetBaseProjectileStats());

            foreach (ProjectileStats stats in bullets)
            {
                Projectile b = BulletPool.Instance.GetBullet(bulletType);
                b.Initialize(gameObject, stats);

                Vector3 mod = Random.insideUnitSphere * bulletOffset;
                b.transform.position = bulletSource[currSource].position + mod;
                b.gameObject.SetActive(true);
                b.SetVelocity(bulletSource[currSource].forward);
            }

            if (++currSource >= bulletSource.Length)
                currSource = 0;
        }
    }

    protected virtual ProjectileStats GetBaseProjectileStats()
    {
        return new ProjectileStats
        {
            damage = bulletDamage,
            speed = bulletSpeed,
            targetTag = targetTag
        };
    }

    protected virtual void FireSpecial()
    {

    }
}
