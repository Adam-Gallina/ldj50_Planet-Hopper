using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UpgradeType { damage, miningSpeed, shipSpeed, shieldHealth, shieldRate }
public enum RepairType { drill, drive, shield, primaryGun, specialWeapon }
public class PlayerManager : MonoBehaviour
{
    public Dictionary<ResourceType, int> inventory = new Dictionary<ResourceType, int>();

    [Header("Upgrades")]
    [SerializeField] private int maxUpgrade;
    public int upgradeCost;
    [SerializeField] private int upgradeCostMod;
    public Upgrade damageMod;
    public Upgrade miningSpeedMod;
    public Upgrade shipSpeedMod;
    public Upgrade shieldHealthMod;
    public Upgrade shieldRateMod;
    public Dictionary<UpgradeType, Upgrade> upgrades = new Dictionary<UpgradeType, Upgrade>();

    [Header("Repairs")]
    public int maxRepair = 3;
    public Upgrade miningPenalty;
    public Upgrade drivePenalty;
    public Upgrade shieldPenalty;
    public Dictionary<RepairType, Upgrade> repairs = new Dictionary<RepairType, Upgrade>();

    //[Header("Weapon Mods")]
    public List<WeaponModBase> weaponMods = new List<WeaponModBase>();

    private void Awake()
    {
        inventory.Add(ResourceType.Metal, 0);
        inventory.Add(ResourceType.Electronics, 0);
        inventory.Add(ResourceType.Uranium, 0);

        upgrades.Add(UpgradeType.damage, damageMod);
        upgrades.Add(UpgradeType.miningSpeed, miningSpeedMod);
        upgrades.Add(UpgradeType.shipSpeed, shipSpeedMod);
        upgrades.Add(UpgradeType.shieldHealth, shieldHealthMod);
        upgrades.Add(UpgradeType.shieldRate, shieldRateMod);

        repairs.Add(RepairType.drill, miningPenalty);
        repairs.Add(RepairType.drive, drivePenalty);
        repairs.Add(RepairType.shield, shieldPenalty);
    }

    private void Update()
    {
        LevelUI.Instance.SetInventory(inventory[ResourceType.Metal],
                                      inventory[ResourceType.Electronics],
                                      inventory[ResourceType.Uranium]);

        LevelUI.Instance.SetUpgrades(upgradeCost,
                                     upgrades[UpgradeType.damage].level,
                                     upgrades[UpgradeType.miningSpeed].level,
                                     upgrades[UpgradeType.shipSpeed].level,
                                     upgrades[UpgradeType.shieldRate].level);

        LevelUI.Instance.SetRepairs(repairs[RepairType.drill].level,
                                    repairs[RepairType.drive].level,
                                    repairs[RepairType.shield].level);
    }

    public void AddResource(ResourceType resource, int amount)
    {
        inventory[resource] += amount;
    }

    #region Upgrades
    public bool UpgradeShip(UpgradeType upgrade, bool ignoreCost=false)
    {
        if (!ignoreCost && inventory[ResourceType.Electronics] < upgradeCost)
            return false;

        if (upgrades[upgrade] >= maxUpgrade)
            return false;

        upgrades[upgrade].level++;

        if (!ignoreCost)
        {
            inventory[ResourceType.Electronics] -= upgradeCost;
            upgradeCost += upgradeCostMod;
        }

        return true;
    }
    #endregion

    #region Repairs
    public void CheckForBreak()
    {
        int RandomBreak() { return Random.Range(0, 1000) < 300 + 700 * (-Mathf.Pow(GameController.Instance.completedLevels, 0.2f) + 1.99f) ? 0 : 1; }

        foreach (RepairType r in repairs.Keys)
        {
            if (repairs[r].level < maxRepair)
                repairs[r].level += RandomBreak();
        }
    }

    public bool RepairShip(RepairType repair)
    {
        if (inventory[ResourceType.Metal] < 5)
            return false;

        if (repairs[repair].level <= 0)
            return false;

        repairs[repair].level -= 1;

        inventory[ResourceType.Metal] -= 5;

        return true;
    }
    #endregion

    #region Weapon Mods
    public void AddWeaponMod(WeaponModBase newMod)
    {
        weaponMods.Add(newMod);

        Debug.Log($"Collected {newMod.modName} ({weaponMods.Count}");
    }
    #endregion
}


[System.Serializable]
public class Upgrade
{
    [HideInInspector] public int level;
    [SerializeField] protected float mod;
    [HideInInspector] public virtual float Mod { get { return level * mod; } }

    public static implicit operator float(Upgrade obj)
    {
        return obj.Mod;
    }
}