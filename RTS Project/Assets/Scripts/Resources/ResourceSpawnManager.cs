using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawnManager : MonoBehaviour
{
    private List<GameObject> resourceObjects = new List<GameObject>();
    [SerializeField] private float spawnRange = 100f;
    [SerializeField] private GameObject spawnObject;
    [SerializeField] private int minSpawnAmount;
    [SerializeField] private int maxSpawnAmount;
    [SerializeField] private float minSpawnSize;
    [SerializeField] private float maxSpawnSize;
    [SerializeField] private LayerMask spawnLayerMask;

    [SerializeField] private int minSpawnerAmount = 5;
    [SerializeField] private int maxSpawnerAmount = 10;
    [SerializeField] private float miniSpawnerRange = 10f;

    private void Start()
    {
        int _randomSpawnAmount = Random.Range(minSpawnAmount, maxSpawnAmount);
        for (int i = 0; i < _randomSpawnAmount; i++)
        {
            SpawnResource(transform.position);
            SpawnMoreMiniSpawners();
        }
    }

    private void SpawnResource(Vector3 origin)
    {
        Vector3 _spawnLocation;
        float _rndX = Random.Range(origin.x - spawnRange, origin.x + spawnRange);
        float _rndZ = Random.Range(origin.z - spawnRange, origin.z + spawnRange);
        _spawnLocation = new Vector3(_rndX, origin.y, _rndZ);

        if (Physics.CheckSphere(_spawnLocation, 1, spawnLayerMask))
        {
            return;
        }

        GameObject _spawnedObject = Instantiate(spawnObject, _spawnLocation, Quaternion.Euler(Vector3.up * Random.Range(0, 360)), transform);
        Vector3 _randomSize;
        float _randomSizeNum = Random.Range(spawnObject.transform.localScale.x * minSpawnSize, spawnObject.transform.localScale.x * maxSpawnSize);
        _randomSize = new Vector3(_randomSizeNum, _randomSizeNum, _randomSizeNum);
        _spawnedObject.transform.localScale = _randomSize;
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

        if (Physics.CheckSphere(_spawnLocation, 1, spawnLayerMask))
        {
            return;
        }

        int _randomSpawnAmount = Random.Range(minSpawnerAmount, maxSpawnerAmount);
        for (int i = 0; i < _randomSpawnAmount; i++)
        {
            SpawnResource(_spawnLocation);
        }
    }

    public void SetSpawnObject(GameObject _spawnObject)
    {
        spawnObject = _spawnObject;
    }
}
