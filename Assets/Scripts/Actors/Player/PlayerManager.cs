using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum UpgradeType { damage, miningSpeed, shipSpeed, shield }
public enum RepairType { drill, drive, shield }
public class PlayerManager : MonoBehaviour
{
    public Dictionary<ResourceType, int> inventory = new Dictionary<ResourceType, int>();

    [Header("Upgrades")]
    [SerializeField] private int maxUpgrade;
    public int upgradeCost;
    [SerializeField] private int upgradeCostMod;
    [SerializeField] private float damageMod;
    [HideInInspector] public float DamageMod { get { return upgrades[UpgradeType.damage] * damageMod; } }
    [SerializeField] private float miningSpeedMod;
    [HideInInspector] public float MiningSpeedMod { get { return upgrades[UpgradeType.miningSpeed] * miningSpeedMod; } }
    [SerializeField] private float shipSpeedMod;
    [HideInInspector] public float SpeedMod { get { return upgrades[UpgradeType.shipSpeed] * shipSpeedMod; } }
    [SerializeField] private float shieldHealthMod;
    [HideInInspector] public float ShieldHealthMod { get { return upgrades[UpgradeType.shield] * shieldHealthMod; } }
    [SerializeField] private float shieldRateMod;
    [HideInInspector] public float ShieldRateMod { get { return upgrades[UpgradeType.shield] * shieldRateMod; } }
    public Dictionary<UpgradeType, int> upgrades = new Dictionary<UpgradeType, int>();

    [Header("Repairs")]
    public int maxRepair = 3;
    [HideInInspector] public int miningLevel;
    [SerializeField] private float miningPenalty;
    [HideInInspector] public float MiningPenalty { get { return (maxRepair - miningLevel) * miningPenalty; } }
    [HideInInspector] public int driveLevel;
    [SerializeField] private int drivePenalty;
    [HideInInspector] public int DrivePenalty { get { return (maxRepair - driveLevel) * drivePenalty; } }
    [HideInInspector] public int shieldLevel;
    [SerializeField] private float shieldPenalty;
    [HideInInspector] public float ShieldPenalty { get { return (maxRepair - shieldLevel) * shieldPenalty; } }

    private void Awake()
    {
        inventory.Add(ResourceType.Metal, 0);
        inventory.Add(ResourceType.Electronics, 0);
        inventory.Add(ResourceType.Uranium, 0);

        upgrades.Add(UpgradeType.damage, 0);
        upgrades.Add(UpgradeType.miningSpeed, 0);
        upgrades.Add(UpgradeType.shipSpeed, 0);
        upgrades.Add(UpgradeType.shield, 0);

        miningLevel = driveLevel = shieldLevel = maxRepair;
    }

    public void AddResource(ResourceType resource, int amount)
    {
        inventory[resource] += amount;
    }

    #region Upgrades
    public bool UpgradeShip(UpgradeType upgrade)
    {
        if (inventory[ResourceType.Electronics] < upgradeCost)
            return false;

        if (upgrades[upgrade] == maxUpgrade)
            return false;

        upgrades[upgrade]++;

        inventory[ResourceType.Electronics] -= upgradeCost;
        upgradeCost += upgradeCostMod;

        return true;
    }
    #endregion

    #region Repairs
    public void CheckForBreak()
    {
        int RandomBreak() { return Random.Range(0, 1000) < 300 + 700 * (-Mathf.Pow(GameController.Instance.completedLevels, 0.2f) + 1.99f) ? 0 : 1; }

        if (miningLevel > 0)
            miningLevel -= RandomBreak();
        if (driveLevel > 0)
            driveLevel -= RandomBreak();
        if (shieldLevel > 0)
            shieldLevel -= RandomBreak();
    }

    public bool RepairShip(RepairType repair)
    {
        if (inventory[ResourceType.Metal] < 5)
            return false;

        switch (repair)
        {
            case RepairType.drill:
                if (miningLevel >= maxRepair)
                    return false;
                miningLevel += 1;
                break;
            case RepairType.drive:
                if (driveLevel >= maxRepair)
                    return false;
                driveLevel += 1;
                break;
            case RepairType.shield:
                if (shieldLevel >= maxRepair)
                    return false;
                shieldLevel += 1;
                break;
        }

        inventory[ResourceType.Metal] -= 5;

        return true;
    }
    #endregion

    //moveSpeed + upgrades[UpgradeType.shipSpeed] * shipSpeedMod);

    //nextMine = Time.time + miningSpeed + (maxRepair - drillLevel) * miningPenalty;
    //baseMiningStrength + upgrades[UpgradeType.miningSpeed] * miningSpeedMod;

    //inventory[ResourceType.Uranium] < uraniumPerWarp + (maxRepair - driveLevel) * drivePenalty

    //bulletDamage + upgrades[UpgradeType.damage] * damageMod
}
