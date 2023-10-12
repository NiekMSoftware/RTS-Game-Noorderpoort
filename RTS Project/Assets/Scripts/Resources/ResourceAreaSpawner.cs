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

    [System.Serializable]
    class SpawnableResource
    {
        public Transform parent;
        public GameObject prefabToSpawn;
        public Texture2D texture;
        public float amountToSpawn;
    }

    private void Start()
    {
        Stopwatch sw = Stopwatch.StartNew();

        foreach (var resource in spawnableResources)
        {
            int amountSpawned = 0;

            for (int i = 0; i < resource.amountToSpawn + 10; i++)
            {
                if (amountSpawned >= resource.amountToSpawn)
                {
                    return;
                }

                int randomX = Random.Range(0, scale.x);
                int randomY = Random.Range(0, scale.y);

                if (resource.texture.GetPixel(randomX, randomY).a >= 0.9f)
                {
                    Vector3 position = new(randomX, terrain.SampleHeight(new Vector3(randomX, 0, randomY)), randomY);

                    if (!Physics.CheckSphere(position, checkRadius, resourceLayer))
                    {
                        amountSpawned++;
                        GameObject spawnedResource = Instantiate(resourceManagerToSpawn, position, Quaternion.identity, resource.parent);
                        spawnedResource.GetComponent<ResourceSpawnManager>().Init(spawnedResource, terrain);
                    }
                }
            }
        }

        sw.Stop();
        print("Spawn resouces took : " + sw.ElapsedMilliseconds + "ms");
    }
}