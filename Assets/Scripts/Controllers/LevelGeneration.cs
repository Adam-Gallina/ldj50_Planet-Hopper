using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    public static LevelGeneration Instance;

    [SerializeField] private float spawnSize = 125;

    [Header("Wall Generation")]
    [SerializeField] private MeshFilter outerWall;
    [SerializeField] private float noiseAmp;

    [Header("Resource Generation")]
    [SerializeField] private ObjectGeneration metal;
    [SerializeField] private ObjectGeneration electronics;
    [SerializeField] private ObjectGeneration uranium;
    [SerializeField] private float distBetweenResources;

    [Header("Aesthetics Generation")]
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private float treeChance;
    [SerializeField] private float treeAmp;
    //[SerializeField] private ObjectGeneration rock;
    [SerializeField] private ObjectGeneration mountain;
    [SerializeField] private float distBetweenEnvironment;
    [SerializeField] private float distBetweenMountains;

    private void Awake()
    {
        Instance = this;
    }

    public void GenerateLevel()
    {
        GenerateWall();

        GameObject mountainBlock = Instantiate(mountain.prefab, transform, false);
        PlaceObject(mountain, distBetweenMountains);
        Destroy(mountainBlock);
        //PlaceObject(rock, distBetweenEnvironment);
        PlaceObject(metal, distBetweenResources);
        PlaceObject(electronics, distBetweenResources);
        PlaceObject(uranium, distBetweenResources);

        GenerateTrees();
    }

    private void GenerateWall()
    {
        Vector3[] vertices = outerWall.mesh.vertices;

        float perlinMod = Random.Range(-1000f, 1000f);

        for (int i = 0; i < vertices.Length; i++)
        {
            float val = Mathf.PerlinNoise(perlinMod + vertices[i].x, perlinMod + vertices[i].y);
            val = (val * 2 - 1) * noiseAmp; // (-1, 1)

            vertices[i].x += val;
            vertices[i].y += val;
        }

        outerWall.mesh.vertices = vertices;
        outerWall.GetComponent<MeshCollider>().sharedMesh = outerWall.mesh;
    }

    private void PlaceObject(ObjectGeneration obj, float buffer)
    {
        int count = Random.Range(obj.minCount, obj.maxCount);
        int tries = 10;
        while (count > 0)
        {
            Vector3 pos = new Vector3(Random.Range(-spawnSize, spawnSize), 0, Random.Range(-spawnSize, spawnSize));
            pos += Constants.ToGround;

            // Check if inside wall
            bool inside = true;
            foreach (Vector3 dir in new Vector3[] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left })
            {
                if (!Physics.Raycast(pos - Constants.ToGround, dir, spawnSize, 1 << Constants.EnvironmentLayer | 1 << Constants.GroundLayer))
                {
                    inside = false;
                    break;
                }
            }
            if (!inside)
                continue;

            // Check if overlapping walls or other resources
            if (Physics.OverlapSphere(pos, buffer, 1 << Constants.ResourceLayer | 1 << Constants.EnvironmentLayer).Length > 0)
                if (tries-- > 0)
                    continue;

            GetRandomizedObject(obj.prefab, pos, obj.minScale, obj.maxScale);
            count--;
            tries = 10;
        }
    }

    private void GenerateTrees()
    {
        float perlinMod1 = Random.Range(-2000f, 2000f), perlinMod2 = Random.Range(-2000f, 2000f);
        bool spawnedTree = false;

        for (int x = -(int)spawnSize; x <= spawnSize; x++)
        {
            for (int z = -(int)spawnSize; z <= spawnSize; z++)
            {
                if (Mathf.PerlinNoise(x + perlinMod1, z + perlinMod1) > treeChance)
                {
                    Vector3 pos = new Vector3(x, 0, z);
                    pos += Constants.ToGround;

                    float val = Mathf.PerlinNoise(x + perlinMod2, z + perlinMod2);
                    val = (val * 2 - 1) * treeAmp;
                    pos.x += val;
                    pos.z += val;

                    // Check if inside wall
                    bool inside = true;
                    foreach (Vector3 dir in new Vector3[] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left })
                    {
                        if (!Physics.Raycast(pos - Constants.ToGround, dir, spawnSize, 1 << Constants.EnvironmentLayer | 1 << Constants.GroundLayer))
                        {
                            inside = false;
                            break;
                        }
                    }
                    if (!inside)
                    {
                        continue;
                    }

                    if (Physics.OverlapSphere(pos, distBetweenEnvironment, 1 << Constants.ResourceLayer | 1 << Constants.EnvironmentLayer).Length > 0)
                    {
                        continue;
                    }

                    GetRandomizedObject(treePrefab, pos, .75f, 1.5f);
                    spawnedTree = true;
                }
            }
        }

        if (!spawnedTree)
            GenerateTrees();
    }

    private GameObject GetRandomizedObject(GameObject obj, Vector3 pos, float minScale, float maxScale)
    {
        GameObject newObj = Instantiate(obj, pos, Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up), transform);
        float scale = Random.Range(minScale, maxScale);
        newObj.transform.localScale = new Vector3(scale, scale, scale);
        return newObj;
    }
}

[System.Serializable]
public struct ObjectGeneration
{
    public GameObject prefab;
    public int minCount;
    public int maxCount;
    public float minScale;
    public float maxScale;
}
