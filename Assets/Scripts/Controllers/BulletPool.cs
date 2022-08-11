using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType { Player, Enemy, None }
public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [SerializeField] private BulletPrefab[] prefabData;
    private Dictionary<BulletType, BulletPrefab> prefabs = new Dictionary<BulletType, BulletPrefab>();
    private Dictionary<BulletType, List<Projectile>> bulletPools = new Dictionary<BulletType, List<Projectile>>();

    private void Awake()
    {
        Instance = this;

        foreach (BulletPrefab p in prefabData)
        {
            prefabs.Add(p.type, p);
        }
    }

    private List<Projectile> GetPool(BulletType type)
    {
        if (!bulletPools.ContainsKey(type))
            bulletPools.Add(type, new List<Projectile>());

        return bulletPools[type];
    }

    private GameObject GetPrefab(BulletType type)
    {
        if (!prefabs.ContainsKey(type)) {
            Debug.LogError($"Trying to instantiate a {type} bullet, but no prefab is known");
            return null;
        }

        return prefabs[type].prefab;
    }

    public virtual Projectile GetBullet(BulletType type)
    {
        List<Projectile> p = GetPool(type);

        if (p.Count == 0)
        {
            GameObject newBullet = Instantiate(GetPrefab(type), transform);
            newBullet.GetComponent<Projectile>().type = type;
            newBullet.SetActive(false);

            p.Add(newBullet.GetComponent<Projectile>());
        }

        if (!p[0])
        {
            Debug.LogError($"Bullet was destroyed and not removed from {type} pool");
            p.RemoveAt(0);
            return GetBullet(type);
        }

        Projectile ret = p[0];
        p.Remove(ret);
        return ret;
    }

    public void ReturnToBulletPool(Projectile bullet)
    {
        bullet.gameObject.SetActive(false);

        GetPool(bullet.type).Add(bullet);
    }
}

[System.Serializable]
public struct BulletPrefab
{
    public BulletType type;
    public GameObject prefab;
}