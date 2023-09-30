using UnityEngine;

public class ResourceAreaSpawner : MonoBehaviour
{
    [SerializeField] private Vector2Int scale;
    [SerializeField] private SpawnableResouce[] spawnableResources;

    [System.Serializable]
    class SpawnableResouce
    {
        public GameObject prefabToSpawn;
        public Texture2D texture;
        public float amountToSpawn;
    }

    private void Start()
    {
        foreach (var resource in spawnableResources)
        {
            int amountSpawned = 0;

            while (amountSpawned < resource.amountToSpawn)
            {
                int randomX = Random.Range(0, scale.x);
                int randomY = Random.Range(0, scale.y);

                if (resource.texture.GetPixel(randomX, randomY).a >= 0.9f)
                {
                    amountSpawned++;
                    Instantiate(resource.prefabToSpawn, new Vector3(randomX, 0, randomY), Quaternion.identity);
                }
            }
        }
    }
}