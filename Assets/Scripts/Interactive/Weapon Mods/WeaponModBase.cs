using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModBase : MonoBehaviour
{
    public string modName = "Default Mod";
    //public int inputCount;
    [Range(1, 10)] public int outputCount = 1;
    [SerializeField] protected float damageMod = 1;
    [SerializeField] protected float speedMod = 1;

    protected List<WeaponModBase> outputs;

    private void Awake()
    {
        outputs = new List<WeaponModBase>();
        for (int i = 0; i < outputCount; i++)
            outputs.Add(null);
    }

    public bool SetOutput(int index, WeaponModBase mod)
    {
        if (index < 0 || index >= outputCount)
            return false;

        outputs[index] = mod;
        return true;
    }

    public bool RemoveOutput(int index)
    {
        if (index < 0 || index >= outputCount)
            return false;

        outputs[index] = null;
        return true;
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
