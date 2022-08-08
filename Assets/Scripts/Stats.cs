using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public static Stats Instance;

    [HideInInspector] public int planetsVisited = 0;
    [HideInInspector] public int resourcesCollected = 0;
    private float startTime = 0;
    private float timeLasted = 0;
    [HideInInspector] public int enemiesDestroyed = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        startTime = Time.time;
    }

    private void Update()
    {
        timeLasted = Time.time - startTime;
        LevelUI.Instance.UpdateStats(planetsVisited, resourcesCollected, timeLasted, enemiesDestroyed);
    }
}
