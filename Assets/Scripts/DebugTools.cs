using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTools : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerController.Instance.AddResource(ResourceType.Metal, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerController.Instance.AddResource(ResourceType.Electronics, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayerController.Instance.AddResource(ResourceType.Uranium, 1);
        }

        if (Input.GetKeyDown(KeyCode.U))
            PlayerController.Instance.upgrades[UpgradeType.damage] -= 1;
        if (Input.GetKeyDown(KeyCode.I))
            PlayerController.Instance.upgrades[UpgradeType.miningSpeed] -= 1;
        if (Input.GetKeyDown(KeyCode.O))
            PlayerController.Instance.upgrades[UpgradeType.shield] -= 1;
        if (Input.GetKeyDown(KeyCode.P))
            PlayerController.Instance.upgrades[UpgradeType.shipSpeed] -= 1;

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
            PlayerController.Instance.drillLevel += Input.GetKey(KeyCode.LeftShift) ? 1 : -1;
        if (Input.GetKeyDown(KeyCode.Period))
            PlayerController.Instance.driveLevel += Input.GetKey(KeyCode.LeftShift) ? 1 : -1;
        if (Input.GetKeyDown(KeyCode.Slash))
            PlayerController.Instance.shieldLevel += Input.GetKey(KeyCode.LeftShift) ? 1 : -1;
    }
}
