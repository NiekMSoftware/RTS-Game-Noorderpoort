using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawnManager : MonoBehaviour
{
    private List<GameObject> resourceObjects = new List<GameObject>();
    [SerializeField] private float spawnRange = 100f;
    [SerializeField] private int minSpawnAmount;
    [SerializeField] private int maxSpawnAmount;
    [SerializeField] private float minSpawnSize;
    [SerializeField] private float maxSpawnSize;
    [SerializeField] private LayerMask spawnLayer;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private int minSpawnerAmount = 5;
    [SerializeField] private int maxSpawnerAmount = 10;
    [SerializeField] private float miniSpawnerRange = 10f;

    private GameObject spawnObject;
    private Terrain terrain;

    private void SpawnResources()
    {
        int _randomSpawnAmount = Random.Range(minSpawnAmount, maxSpawnAmount);
        for (int i = 0; i < _randomSpawnAmount; i++)
        {
            SpawnMoreMiniSpawners();
        }
    }

    private void SpawnResource(Vector3 origin)
    {
        Vector3 _spawnLocation;
        float _rndX = Random.Range(origin.x - spawnRange, origin.x + spawnRange);
        float _rndZ = Random.Range(origin.z - spawnRange, origin.z + spawnRange);
        if (terrain)
        {
            _spawnLocation = new Vector3(_rndX, terrain.SampleHeight(new Vector3(_rndX, 0, _rndZ)), _rndZ);
        }
        else
        {
            _spawnLocation = new Vector3(_rndX, 0, _rndZ);
        }

        if (Physics.CheckSphere(_spawnLocation, 1, spawnLayer))
        {
            return;
        }

        Vector3 terrainNormal = Vector3.zero;

        if (Physics.Raycast(_spawnLocation + new Vector3(0, 1, 0), -Vector3.up, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            terrainNormal = hit.normal;
        }

        // Calculate the random Y-axis rotation
        float randomYRotation = Random.Range(0f, 360f);

        // Create a Quaternion for the random Y-axis rotation
        Quaternion randomYRotationQuaternion = Quaternion.Euler(0, randomYRotation, 0);

        // Calculate the initial rotation based on the terrainNormal
        Quaternion initialRotation = Quaternion.FromToRotation(Vector3.up, terrainNormal);

        // Combine the initial rotation with the random Y-axis rotation
        Quaternion finalRotation = initialRotation * randomYRotationQuaternion;

        GameObject _spawnedObject = Instantiate(spawnObject, _spawnLocation, finalRotation, transform);
        _spawnedObject.name = spawnObject.name;

        Vector3 _randomSize;
        float _randomSizeNum = Random.Range(spawnObject.transform.localScale.x * minSpawnSize, spawnObject.transform.localScale.x * maxSpawnSize);
        _randomSize = new Vector3(_randomSizeNum, _randomSizeNum, _randomSizeNum);
        _spawnedObject.transform.localScale = _randomSize;

        GetComponent<ResourceObjectManager>().SetResources(_spawnedObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange * 2, 1, spawnRange * 2));
    }
    public void OnDestroyResource(GameObject _resourceObject)
    {
        resourceObjects.Remove(_resourceObject);
    }
    private void SpawnMoreMiniSpawners()
    {
        Vector3 _spawnLocation;
        float _rndX = Random.Range(transform.position.x - miniSpawnerRange, transform.position.x + miniSpawnerRange);
        float _rndZ = Random.Range(transform.position.z - miniSpawnerRange, transform.position.z + miniSpawnerRange);
        _spawnLocation = new Vector3(_rndX, transform.position.y, _rndZ);

        if (Physics.CheckSphere(_spawnLocation, 1, spawnLayer))
        {
            return;
        }

        int _randomSpawnAmount = Random.Range(minSpawnerAmount, maxSpawnerAmount);

        for (int i = 0; i < _randomSpawnAmount; i++)
        {
            SpawnResource(_spawnLocation);
        }
    }

    public void Init(GameObject spawnObject, Terrain terrain)
    {
        this.spawnObject = spawnObject;
        this.terrain = terrain;

        SpawnResources();
    }
}