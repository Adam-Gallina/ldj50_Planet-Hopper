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
        maxHealth = baseShieldHealth + manager.ShieldHealthMod;
        shieldRechargeRate = baseShieldRate + manager.ShieldRateMod;
        shieldRechargeDelay = baseShieldDelay + manager.ShieldPenalty;
        if (manager.shieldLevel <= 0)
            shieldRechargeDelay = -1;
    }
}
