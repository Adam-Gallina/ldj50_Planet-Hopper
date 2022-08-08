using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatBase : HealthBase
{
    private List<Projectile> bulletPool;

    [Header("Combat")]
    [SerializeField] private Transform[] bulletSource;
    private int currSource = 0;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] protected float fireSpeed;
    protected float nextShot = 0;
    [SerializeField] protected float bulletDamage;

    [SerializeField] private Transform specialSource;

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

            Projectile bullet = GetBullet();
            bullet.transform.position = bulletSource[currSource].position;
            bullet.gameObject.SetActive(true);
            bullet.SetVelocity(bulletSource[currSource].forward);

            if (++currSource >= bulletSource.Length)
                currSource = 0;
        }
    }

    protected virtual void FireSpecial()
    {

    }

    protected virtual Projectile GetBullet()
    {
        if (bulletPool.Count == 0 || !bulletPool[0])
        {
            bulletPool.Clear();

            GameObject newBullet = Instantiate(bulletPrefab);
            newBullet.GetComponent<Projectile>().Initialize(this, targetTag);
            newBullet.GetComponent<Projectile>().SetDamage(bulletDamage);
            newBullet.SetActive(false);

            bulletPool.Add(newBullet.GetComponent<Projectile>());
        }

        Projectile ret = bulletPool[0];
        bulletPool.Remove(ret);
        return ret;
    }

    public void ReturnToBulletPool(Projectile bullet)
    {
        bullet.gameObject.SetActive(false);

        bulletPool.Add(bullet);
    }
}
