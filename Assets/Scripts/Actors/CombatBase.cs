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

            Projectile bullet = BulletPool.Instance.GetBullet(bulletType);
            InitBullet(bullet);
            bullet.transform.position = bulletSource[currSource].position;
            bullet.gameObject.SetActive(true);
            bullet.SetVelocity(bulletSource[currSource].forward);

            if (++currSource >= bulletSource.Length)
                currSource = 0;
        }
    }

    protected virtual void InitBullet(Projectile bullet)
    {
        bullet.Initialize(gameObject, bulletSpeed, bulletDamage, targetTag);
    }

    protected virtual void FireSpecial()
    {

    }
}
