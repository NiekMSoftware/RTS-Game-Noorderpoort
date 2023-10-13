using System.Collections.Generic;
using UnityEngine;

public class ComputerEnemy : MonoBehaviour
{
    [SerializeField] private float amountOfWorkersAtStart;
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private ResourceItemManager resources;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private float scanRange;
    [SerializeField] private ResourceAndBuilding[] resourcesAndBuildings;
    [SerializeField] private ItemData woodItem;
    [SerializeField] private Terrain terrain;
    [SerializeField] private LayerMask resourceLayer;
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("Higher is more accurate but also less performent")]
    [SerializeField] private float buildingPlaceAccuracy = 50;

    [SerializeField] private List<Worker> workers;
    [SerializeField] private List<GameObject> buildings;

    private List<GameObject> resourceAreas = new();

    [System.Serializable]
    public class ResourceAndBuilding
    {
        public GameObject building;
        public ItemData itemData;
    }

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

        PlaceBuilding(resourcesAndBuildings[GetResourceIndexByItemdata(woodItem)].itemData);
    }

    private void Update()
    {

    }

    public void PlaceBuilding(ItemData itemData)
    {
        ResourceObjectManager closestResource = FindClosestResourceManager(itemData);

        Vector3 direction = transform.position - closestResource.transform.position;
        Debug.DrawRay(closestResource.transform.position, direction * 1000, Color.green, 30);

        Vector3 originalPos = closestResource.transform.position;

        while (Physics.CheckSphere(originalPos, 5f, resourceLayer))
        {
            originalPos += direction / buildingPlaceAccuracy;
        }

        originalPos.y = terrain.SampleHeight(originalPos) +
            resourcesAndBuildings[GetResourceIndexByItemdata(woodItem)].building.transform.localScale.y;

        GameObject spawnedBuilding = Instantiate(resourcesAndBuildings[GetResourceIndexByItemdata(woodItem)].building
            , originalPos, Quaternion.identity);

        if (Physics.Raycast(spawnedBuilding.transform.position + new Vector3(0, 1, 0), -Vector3.up, out RaycastHit hit, groundLayer))
        {
            spawnedBuilding.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            buildings.Add(spawnedBuilding);

            for (int i = 0; i < 2; i++)
            {
                int randomWorker = Random.Range(0, workers.Count);
                spawnedBuilding.GetComponent<BuildingBase>().AddWorkerToBuilding(workers[randomWorker]);
            }
        }
    }

    public ResourceObjectManager FindClosestResourceManager(ItemData itemdata)
    {
        ResourceObjectManager[] resourceManagers = FindObjectsOfType<ResourceObjectManager>();
        List<ResourceObjectManager> validResourceManagers = new();

        foreach (var resourceManager in resourceManagers)
        {
            if (resourceManager.resources.Count > 0)
            {
                if (resourceManager.resources[0].GetComponent<ResourceObject>().slot.data == itemdata)
                {
                    validResourceManagers.Add(resourceManager);
                }
            }
        }

        ResourceObjectManager closestResource = null;
        float closestDistance = scanRange;

        foreach (var resourceManager in validResourceManagers)
        {
            float distanceToResourceManager = Vector3.Distance(transform.position, resourceManager.transform.position);

            if (distanceToResourceManager <= scanRange && distanceToResourceManager <= closestDistance)
            {
                closestDistance = distanceToResourceManager;
                closestResource = resourceManager;
            }
        }

        return closestResource;
    }

    public int GetResourceIndexByItemdata(ItemData itemData)
    {
        int index = 0;

        foreach (var item in resourcesAndBuildings)
        {
            if (item.itemData == itemData)
            {
                print("found itemdata");
                return index;
            }
            index++;
        }

        return -1;
    }
}