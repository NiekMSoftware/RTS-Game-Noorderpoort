using UnityEngine;

public class PlayerWorkerSpawner : MonoBehaviour
{
    [SerializeField] private int amountToSpawn;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector2 spawnScale;
    [SerializeField] private Vector2 spawnOffset;
    [SerializeField] private Terrain terrain;

    private Worker[] workers;

    private void Awake()
    {
        terrain = FindObjectOfType<Terrain>();
    }

    private void Start()
    {
        workers = new Worker[amountToSpawn];

        for (int i = 0; i < amountToSpawn; i++)
        {
            Vector3 position = transform.position;
            position.x += Random.Range(spawnScale.x, spawnScale.y) + spawnOffset.x;
            position.z += Random.Range(spawnScale.x, spawnScale.y) + spawnOffset.y;
            position.y = terrain.SampleHeight(position) + (prefab.transform.lossyScale.y);
            Worker spawnedAI = Instantiate(prefab, position, Quaternion.identity).GetComponent<Worker>();
            workers[i] = spawnedAI;
        }
    }

    public Worker[] GetWorkers() => workers;

    private void OnDrawGizmos()
    {
        Vector3 scale = new(spawnScale.x, 1, spawnScale.y);
        scale *= 2;
        Vector3 position = transform.position;
        position.x += spawnOffset.x;
        position.z += spawnOffset.y;
        if (terrain)
            position.y = terrain.SampleHeight(position);
        Gizmos.DrawWireCube(position, scale);
    }
}
