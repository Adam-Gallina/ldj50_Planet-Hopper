using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : Shield
{
    private float baseShieldDelay;
    private float baseShieldRate;
    private float baseShieldHealth;

    private PlayerManager manager;

    protected override void Start()
    {
        base.Start();

        manager = PlayerController.Instance.manager;

        baseShieldDelay = shieldRechargeDelay;
        baseShieldHealth = maxHealth;
        baseShieldRate = shieldRechargeRate;
    }

    protected override void Update()
    {
        base.Update();

        maxHealth = baseShieldHealth + manager.shieldHealthMod;
        shieldRechargeRate = baseShieldRate + manager.shieldRateMod;
        shieldRechargeDelay = baseShieldDelay + manager.shieldPenalty;
        if (manager.shieldPenalty.level <= 0)
            shieldRechargeDelay = -1;
    }
}
