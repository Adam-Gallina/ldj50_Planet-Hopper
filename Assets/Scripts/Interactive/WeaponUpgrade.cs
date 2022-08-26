using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUpgrade : Resource
{
    [SerializeField] private WeaponModBase weaponMod;

    protected override void OnCollect()
    {
        PlayerController.Instance.manager.AddWeaponMod(weaponMod);
    }
}
