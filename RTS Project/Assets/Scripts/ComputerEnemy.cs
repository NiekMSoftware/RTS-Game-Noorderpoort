using System.Collections.Generic;
using UnityEngine;

public class ComputerEnemy : MonoBehaviour
{
    [SerializeField] private float amountOfWorkersAtStart;
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private ResourceItemManager resources;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private float scanRange;
    [SerializeField] private GameObject building;

    [SerializeField] private List<Worker> workers;
    [SerializeField] private List<GameObject> buildings;

    private List<GameObject> resourceAreas = new();

    private void Start()
    {
        for (int i = 0; i < amountOfWorkersAtStart; i++)
        {
            Worker worker = Instantiate(workerPrefab, transform.position, Quaternion.identity).GetComponent<Worker>();
            workers.Add(worker);
        }

        //int randomWorker = Random.Range(0, workers.Count);
        //Worker currentWorker = workers[randomWorker];
        //currentWorker.

        GameObject closestResource = FindClosestResourceManager(ResourceType.resourceType.Wood);
        print(closestResource.name);
        GameObject spawnedBuilding = Instantiate(building, closestResource.transform.position, Quaternion.identity);
        buildings.Add(spawnedBuilding);
    }

    private void Update()
    {

    }

    public GameObject FindClosestResourceManager(ResourceType.resourceType itemdata)
    {
        Transform resourceAreaSpawner = FindObjectOfType<ResourceAreaSpawner>().transform;

        List<Transform> resourceSorters = new();
        foreach (Transform transform in resourceAreaSpawner)
        {
            resourceSorters.Add(transform);
        }

        List<Transform> resourceSpawners = new();

        foreach (Transform transform in resourceSorters)
        {
            print("resource sorter : " + transform.name);
            resourceSpawners.Add(transform);
        }

        List<Transform> resources = new();

        foreach (Transform transform in resourceSpawners)
        {
            resources.Add(transform);
            print("Individual spawner : " + transform.name);
        }

        return null;

        foreach (Transform resourceType in FindAnyObjectByType<ResourceAreaSpawner>().GetComponentInChildren<Transform>())
        {
            print(resourceType.GetComponentsInChildren<Transform>().Length);

            foreach (Transform resource in resourceType.GetComponentsInChildren<Transform>())
            {
                print("loop through resource");
                if (!resourceAreas.Contains(resource.gameObject))
                {
                    print("add resource area");
                    resourceAreas.Add(resource.gameObject);
                }
            }
        }

        GameObject closestResource = null;
        float closestDistance = scanRange;
        Vector3 currentPosition = transform.position;

        if (resourceAreas != null)
        {
            print("Found resource areas");
            foreach (GameObject resource in resourceAreas)
            {
                print("Loop through resources");
                if (resource != null)
                {
                    print("Resource not null");
                    Vector3 resourcePosition = resource.transform.position;
                    float distanceToResource = Vector3.Distance(currentPosition, resourcePosition);

                    if (distanceToResource <= scanRange && distanceToResource < closestDistance)
                    {
                        print("Resource in range");
                        if (Vector3.Distance(currentPosition, resourcePosition) < scanRange)
                        {
                            print("Resource close enough");
                            closestDistance = distanceToResource;
                            closestResource = resource;
                        }
                    }
                }
            }
        }
        else
        {
            print("No resource in range");
        }
        return closestResource;
    }
}