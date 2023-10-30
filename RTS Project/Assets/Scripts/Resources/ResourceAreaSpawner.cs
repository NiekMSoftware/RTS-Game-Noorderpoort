using System.Diagnostics;
using UnityEngine;

public class ResourceAreaSpawner : MonoBehaviour
{
    [SerializeField] private Vector2Int scale;
    [SerializeField] private GameObject resourceManagerToSpawn;
    [SerializeField] private float checkRadius;
    [SerializeField] private SpawnableResource[] spawnableResources;
    [SerializeField] private LayerMask resourceLayer;
    [SerializeField] private Terrain terrain;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxAngle;
    [SerializeField] private float maxHeight;

    [System.Serializable]
    class SpawnableResource
    {
        public Transform parent;
        public GameObject prefabToSpawn;
        public Texture2D texture;
        public float amountToSpawn;
    }

    private void Awake()
    {
        Stopwatch sw = Stopwatch.StartNew();

        TrySpawnResources();

        sw.Stop();
        print("Spawn resouces took : " + sw.ElapsedMilliseconds + "ms");
    }

    private void TrySpawnResources()
    {
        foreach (var resource in spawnableResources)
        {
            int amountSpawned = 0;
            int loopIndex = 0;

            while (amountSpawned <= resource.amountToSpawn)
            {
                if (loopIndex > 100)
                {
                    break;
                }
                loopIndex++;
                int randomX = Random.Range(0, scale.x);
                int randomZ = Random.Range(0, scale.y);

                if (resource.texture.GetPixel(randomX, randomZ).a >= 0.9f)
                {
                    Vector3 position = new(randomX, terrain.SampleHeight(new Vector3(randomX, 0, randomZ)), randomZ);

                    if (Physics.Raycast(position + new Vector3(0, 1, 0), -Vector3.up, out RaycastHit hit, groundLayer))
                    {
                        Vector3 normalizedNormal = hit.normal.normalized;
                        float rayAngle = Vector3.Angle(Vector3.up, normalizedNormal);

                        if (rayAngle <= maxAngle)
                        {
                            if (position.y <= maxHeight)
                            {
                                if (!Physics.CheckSphere(position, checkRadius, resourceLayer))
                                {
                                    amountSpawned++;
                                    GameObject spawnedResource = Instantiate(resourceManagerToSpawn, position, Quaternion.identity, resource.parent);
                                    spawnedResource.GetComponent<ResourceSpawnManager>().Init(resource.prefabToSpawn, terrain);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}