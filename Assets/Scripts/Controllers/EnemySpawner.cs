using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [SerializeField] protected EnemySpawnType[] availableEnemies;

    [Header("Spawn settings")]
    [SerializeField] protected int baseEnemyThreshold;
    [SerializeField] protected float levelMod;
    [SerializeField] protected float waveMod;
    [SerializeField] protected float minSpawnDistFromPlayer;
    [SerializeField] protected float maxSpawnDistFromPlayer;
    [SerializeField] protected float percentEnemiesBeforeSpawn;
    [HideInInspector] public bool canSpawn = false;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private float maxSpawnedEnemies;

    private int spawnsOnPlanet = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (canSpawn && (spawnedEnemies.Count <= maxSpawnedEnemies * percentEnemiesBeforeSpawn || spawnedEnemies.Count == 1))
            SpawnWave();
        else
        {
            for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
            {
                if (!spawnedEnemies[i])
                    spawnedEnemies.RemoveAt(i);
            }
        }
    }

    public void SpawnWave()
    {
        int enemyThreshold = (int)((baseEnemyThreshold + (int)((GameController.Instance.completedLevels) * levelMod)) * GameController.DifficultyMod);

        List<EnemySpawnType> eligible = new List<EnemySpawnType>();
        foreach (EnemySpawnType e in availableEnemies)
        {
            if (e.minThreshold <= enemyThreshold)
            {
                eligible.Add(e);
            }
        }

        enemyThreshold += (int)(spawnsOnPlanet * waveMod * GameController.DifficultyMod);

        while (enemyThreshold > 0)
        {
            EnemySpawnType e = eligible[Random.Range(0, eligible.Count)];
            spawnedEnemies.Add(SpawnEnemy(e));

            enemyThreshold -= e.cost;
        }

        maxSpawnedEnemies = spawnedEnemies.Count;
        spawnsOnPlanet++;
    }

    private GameObject SpawnEnemy(EnemySpawnType targetEnemy)
    {
        Vector2 randPoint = Random.insideUnitCircle * Random.Range(minSpawnDistFromPlayer, maxSpawnDistFromPlayer);
        float scale = Random.Range(targetEnemy.minScale, targetEnemy.maxScale);

        GameObject newEnemy = Instantiate(targetEnemy.prefab);
        newEnemy.transform.position = PlayerController.Instance.transform.position + new Vector3(randPoint.x, 0, randPoint.y);
        newEnemy.transform.localScale = new Vector3(scale, scale, scale);

        return newEnemy;
    }
}

[System.Serializable]
public struct EnemySpawnType
{
    public GameObject prefab;
    public int minThreshold;
    public int cost;

    public float minScale;
    public float maxScale;
}