using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void StartLevel()
    {
        LevelGeneration.Instance.GenerateLevel();

        PlayerController.Instance.SetVelocity(new Vector3(0, 0, 0), 0);
        PlayerController.Instance.transform.position = Vector3.zero;
        CameraController.Instance.SetTarget(PlayerController.Instance.transform);

        if (EnemySpawner.Instance)
            EnemySpawner.Instance.canSpawn = true;
    }
}
