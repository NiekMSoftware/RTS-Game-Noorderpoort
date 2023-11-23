using UnityEngine;

public class PlayerWorkerSpawner : MonoBehaviour
{
    [SerializeField] private int amountToSpawn;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Vector2 spawnScale;
    [SerializeField] private Terrain terrain;

    private void Start()
    {
        for (int i = 0; i < amountToSpawn; i++)
        {
            Vector3 position = transform.position;
            position.x += Random.Range(spawnScale.x, spawnScale.y);
            position.z += Random.Range(spawnScale.x, spawnScale.y);
            position.y = terrain.SampleHeight(position) + (prefab.transform.lossyScale.y);
            GameObject spawnedAI = Instantiate(prefab, position, Quaternion.identity, transform);
            //spawnedAI.GetComponent<NavMeshAgent>().Warp(position);
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 scale = new(spawnScale.x, 1, spawnScale.y);
        scale *= 2;
        Vector3 position = transform.position;
        position.y = terrain.SampleHeight(position);
        Gizmos.DrawWireCube(position, scale);
    }
}
