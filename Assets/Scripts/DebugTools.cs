using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerController.Instance.manager.AddResource(ResourceType.Metal, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerController.Instance.manager.AddResource(ResourceType.Electronics, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayerController.Instance.manager.AddResource(ResourceType.Uranium, 1);
        }

        if (Input.GetKeyDown(KeyCode.U))
            PlayerController.Instance.manager.upgrades[UpgradeType.damage].level -= 1;
        if (Input.GetKeyDown(KeyCode.I))
            PlayerController.Instance.manager.upgrades[UpgradeType.miningSpeed].level -= 1;
        if (Input.GetKeyDown(KeyCode.O))
        {
            PlayerController.Instance.manager.upgrades[UpgradeType.shieldHealth].level -= 1;
            PlayerController.Instance.manager.upgrades[UpgradeType.shieldRate].level -= 1;
        }
        if (Input.GetKeyDown(KeyCode.P))
            PlayerController.Instance.manager.upgrades[UpgradeType.shipSpeed].level -= 1;

        if (Input.GetKeyDown(KeyCode.R))
            LevelGeneration.Instance.GenerateLevel();

        if (Input.GetKeyDown(KeyCode.Backspace))
            GameController.Instance.NextLevel();

        if (Input.GetKeyDown(KeyCode.Minus))
            EnemySpawner.Instance.SpawnWave();

        if (Input.GetKeyDown(KeyCode.LeftBracket))
            Mothership.Instance.ShowMothership();
        if (Input.GetKeyDown(KeyCode.RightBracket))
            Mothership.Instance.HideMothership();

        if (Input.GetKeyDown(KeyCode.Comma))
            PlayerController.Instance.manager.miningPenalty.level += Input.GetKey(KeyCode.LeftShift) ? 1 : -1;
        if (Input.GetKeyDown(KeyCode.Period))
            PlayerController.Instance.manager.drivePenalty.level += Input.GetKey(KeyCode.LeftShift) ? 1 : -1;
        if (Input.GetKeyDown(KeyCode.Slash))
            PlayerController.Instance.manager.shieldPenalty.level += Input.GetKey(KeyCode.LeftShift) ? 1 : -1;
    }
}
