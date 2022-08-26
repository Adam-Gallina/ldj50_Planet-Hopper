using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponModBase
{
    public string modName = "Default Mod";
    //public int inputCount;
    [Range(1, 10)] public int outputCount = 1;
    [SerializeField] protected float damageMod = 1;
    [SerializeField] protected float speedMod = 1;

    // Serialized for debug
    [SerializeField] protected WeaponModBase[] outputs;

    public WeaponModBase()
    {
        outputs = new WeaponModBase[outputCount];
    }

    protected virtual ProjectileStats ModBullet(ProjectileStats bullet)
    {
        bullet.damage *= damageMod;
        bullet.speed *= speedMod;

        return bullet;
    }

    public virtual List<ProjectileStats> GetBullets(ProjectileStats input)
    {
        List<ProjectileStats> bullets = new List<ProjectileStats>();

        for (int i = 0; i < outputCount; i++)
        {
            if (outputs[i] != null)
                bullets.AddRange(outputs[i].GetBullets(ModBullet(input)));
            else
                bullets.Add(ModBullet(input));
        }

        return bullets;
    }
}
