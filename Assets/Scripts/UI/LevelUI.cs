using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Menu { pause, death, win, none, upgrades }
public class LevelUI : MonoBehaviour
{
    public static LevelUI Instance;

    [Header("Resources")]
    [SerializeField] private Text metalCount;
    [SerializeField] private Text electronicsCount;
    [SerializeField] private Text uraniumCount;

    [Header("Upgrades")]
    [SerializeField] private Text upgradeCost;
    [SerializeField] private Text damageUpgradeButton;
    [SerializeField] private Text miningUpgradeButton;
    [SerializeField] private Text speedUpgradeButton;
    [SerializeField] private Text shieldUpgradeButton;

    [Header("Repairs")]
    [SerializeField] private Text drillRepair;
    [SerializeField] private Image drillRepairBtn;
    [SerializeField] private Text driveRepair;
    [SerializeField] private Image driveRepairBtn;
    [SerializeField] private Text shieldRepair;
    [SerializeField] private Image shieldRepairBtn;
    [SerializeField] private Color[] btnColors;
    private string[] repairLevelText = { "Broken", "Danger", "Warning", "Ok" };

    [Header("Healthbar")]
    [SerializeField] private Slider healthIndicator;
    [SerializeField] private Slider shieldIndicator;

    [Header("Warping")]
    [SerializeField] private Slider warpIndicator;
    [SerializeField] private Button warpButton;

    [Header("Timer")]
    [SerializeField] private Slider timer;

    [Header("Popups")]
    [SerializeField] private GameObject popupBackground;
    [SerializeField] private GameObject upgradesMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deathMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject statsMenu;
    [HideInInspector] public Menu currMenu = Menu.none;

    [Header("Stats")]
    [SerializeField] private Text planetText;
    [SerializeField] private Text resourcesText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text enemyText;

    [Header("Mining Indicator")]
    [SerializeField] private GameObject miningIndicator;
    [SerializeField] private Slider miningPercent;
    private Resource miningTarget;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        SetInventory(PlayerController.Instance.manager.inventory[ResourceType.Metal],
                     PlayerController.Instance.manager.inventory[ResourceType.Electronics],
                     PlayerController.Instance.manager.inventory[ResourceType.Uranium]);

        SetUpgrades(PlayerController.Instance.manager.upgradeCost,
                    PlayerController.Instance.manager.upgrades[UpgradeType.damage],
                    PlayerController.Instance.manager.upgrades[UpgradeType.miningSpeed],
                    PlayerController.Instance.manager.upgrades[UpgradeType.shipSpeed],
                    PlayerController.Instance.manager.upgrades[UpgradeType.shield]);

        SetRepairs(PlayerController.Instance.manager.miningLevel,
                   PlayerController.Instance.manager.driveLevel,
                   PlayerController.Instance.manager.shieldLevel);

        SetHealth(PlayerController.Instance.currHealth / PlayerController.Instance.maxHealth,
                  PlayerController.Instance.GetComponentInChildren<PlayerShield>().currHealth / PlayerController.Instance.GetComponentInChildren<PlayerShield>().maxHealth);

        SetWarp((float)PlayerController.Instance.manager.inventory[ResourceType.Uranium] / (PlayerController.Instance.uraniumPerWarp + (PlayerController.Instance.manager.DrivePenalty)),
                PlayerController.Instance.CheckWarp(false));

        SetTimer(Mothership.Instance.elapsedTime / Mothership.Instance.totalTime);

        UpdateMiningIndicator();
    }

    private void SetInventory(int metal, int electronics, int uranium)
    {
        metalCount.text = "Metal: " + metal;
        electronicsCount.text = "Electronics: " + electronics;
        uraniumCount.text = "Uranium: " + uranium;
    }

    private void SetUpgrades(int cost, int damageLevel, int miningLevel, int speedLevel, int shieldLevel)
    {
        upgradeCost.text = "Cost: " + cost + " electronics";
        damageUpgradeButton.text = "Damage: Level " + (damageLevel + 1);
        miningUpgradeButton.text = "Mining Speed: Level " + (miningLevel + 1);
        speedUpgradeButton.text = "Ship Speed: Level " + (speedLevel + 1);
        shieldUpgradeButton.text = "Shield: Level " + (shieldLevel + 1);
    }

    private void SetRepairs(int drillLevel, int driveLevel, int shieldLevel)
    {
        drillRepair.text = "Mining Drill: " + repairLevelText[drillLevel];
        drillRepairBtn.color = btnColors[drillLevel];

        driveRepair.text = "Warp Drive: " + repairLevelText[driveLevel];
        driveRepairBtn.color = btnColors[driveLevel];

        shieldRepair.text = "Shields: " + repairLevelText[shieldLevel];
        shieldRepairBtn.color = btnColors[shieldLevel];
    }

    private void SetHealth(float healthPercent, float shieldPercent)
    {
        healthIndicator.value = healthPercent;
        shieldIndicator.value = shieldPercent;
    }

    private void SetWarp(float warpPercent, bool canWarp)
    {
        warpIndicator.value = warpPercent;
        warpButton.interactable = canWarp;
    }

    private void SetTimer(float timePercent)
    {
        timer.value = timePercent;
    }

    public void SetMiningIndicator(Resource target)
    {
        miningTarget = target;
    }

    private void UpdateMiningIndicator()
    {
        if (miningTarget)
        {
            miningIndicator.SetActive(true);

            miningIndicator.transform.position = Camera.main.WorldToScreenPoint(miningTarget.transform.position);
            miningPercent.value = miningTarget.currHealth / miningTarget.maxHealth;
        }
        else
        {
            miningIndicator.SetActive(false);
        }
    }

    public void UpgradeShip(int upgrade)
    {
        PlayerController.Instance.manager.UpgradeShip((UpgradeType)upgrade);
    }

    public void RepairShip(int repair)
    {
        PlayerController.Instance.manager.RepairShip((RepairType)repair);
    }

    public void WarpPlayer()
    {
        if (PlayerController.Instance.CheckWarp(true))
            GameController.Instance.NextLevel();
    }

    public bool ToggleMenu(Menu menu)
    {
        if (currMenu != Menu.none && currMenu != menu)
            return false;

        switch (menu)
        {
            case Menu.upgrades:
                GameController.Instance.TogglePause(false);
                upgradesMenu.SetActive(!upgradesMenu.activeSelf);
                break;
            case Menu.pause:
                pauseMenu.SetActive(!pauseMenu.activeSelf);
                break;
            case Menu.death:
                deathMenu.SetActive(!deathMenu.activeSelf);
                statsMenu.SetActive(!statsMenu.activeSelf);
                break;
            case Menu.win:
                winMenu.SetActive(!winMenu.activeSelf);
                statsMenu.SetActive(!statsMenu.activeSelf);
                break;
        }

        if (currMenu == menu)
            currMenu = Menu.none;
        else
            currMenu = menu;

        popupBackground.SetActive(currMenu != Menu.none);
        return true;
    }

    public bool SetMenu(Menu menu, bool visible)
    {
        if (currMenu != Menu.none && currMenu != menu)
            return false;

        switch (menu)
        {
            case Menu.upgrades:
                upgradesMenu.SetActive(visible);
                GameController.Instance.SetPause(visible, false);
                break;
            case Menu.pause:
                pauseMenu.SetActive(visible);
                break;
            case Menu.death:
                deathMenu.SetActive(visible);
                statsMenu.SetActive(visible);
                break;
            case Menu.win:
                winMenu.SetActive(visible);
                statsMenu.SetActive(visible);
                break;
        }

        if (!visible)
            currMenu = Menu.none;
        else
            currMenu = menu;

        popupBackground.SetActive(currMenu != Menu.none);
        return true;
    }

    public void UpdateStats(int planets, int resources, float time, int enemies)
    {
        planetText.text = planets.ToString() + "\nPlanets Visited";
        resourcesText.text = resources.ToString() + "\nResources Collected";

        int seconds = (int)time;
        int minutes = seconds / 60;
        seconds %= 60;
        timeText.text = minutes.ToString() + ":" + seconds.ToString() + "\nTime Survived";
        enemyText.text = enemies.ToString() + "\nEnemies Destroyed";
    }

    public void CloseUpgrades()
    {
        SetMenu(Menu.upgrades, false);
    }

    public void Resume()
    {
        GameController.Instance.TogglePause();
    }

    public void Retry()
    {
        GameController.Instance.Restart();
    }

    public void ReturnToMain()
    {
        GameController.Instance.ReturnToMain();
    }
}
