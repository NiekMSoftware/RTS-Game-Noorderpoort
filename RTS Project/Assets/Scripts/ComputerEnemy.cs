using System.Collections.Generic;
using UnityEngine;

public class ComputerEnemy : MonoBehaviour
{
    [SerializeField] private float amountOfWorkersAtStart;
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private ResourceItemManager resources;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private ResourceAndBuilding[] resourcesAndBuildings;
    [SerializeField] private ItemData woodItem;
    [SerializeField] private ItemData stoneItem;
    [SerializeField] private ItemData metalItem;
    [SerializeField] private Terrain terrain;
    [SerializeField] private LayerMask resourceLayer;
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("Higher is more accurate but also less performent")]
    [SerializeField] private float buildingPlaceAccuracy = 50;
    [SerializeField] private float amountOfSecondsPerChoise = 3;
    [SerializeField] private ResourceItemManager resourceItemManager;

    [SerializeField] private List<Worker> workers;
    [SerializeField] private List<Worker> availableWorkers;
    [SerializeField] private List<BuildingBase> buildings;

    private List<GameObject> resourceAreas = new();

    private float choiseTimer;

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
            availableWorkers.Add(worker);
        }

        PlaceBuilding(resourcesAndBuildings[GetResourceIndexByItemdata(woodItem)].itemData);
        AssignWorker(GetBuildingIndexByType(woodItem), availableWorkers[0]);
    }

    private void Update()
    {
        choiseTimer -= Time.deltaTime;

        if (choiseTimer <= 0)
        {
            choiseTimer = amountOfSecondsPerChoise;

            MakeChoise();
        }
    }

    private void MakeChoise()
    {

    }

    private bool HasEnoughResources(BuildingBase building)
    {
        bool hasAllResources = false;

        foreach (var item in building.GetRecipes())
        {
            if (item.data == resourceItemManager.GetSlotByItemData(item.data).data)
            {
                if (resourceItemManager.GetSlotByItemData(item.data).amount >= item.amountNeeded)
                {
                    hasAllResources = true;
                }
                else
                {
                    return false;
                }
            }
        }

        if (hasAllResources)
        {
            foreach (var item in building.GetRecipes())
            {
                if (resourceItemManager.GetSlotByItemData(item.data).data == item.data)
                {
                    resourceItemManager.GetSlotByItemData(item.data).amount -= item.amountNeeded;
                }
            }
        }

        return hasAllResources;
    }

    private void PlaceBuilding(ItemData itemData)
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

        if (HasEnoughResources(resourcesAndBuildings[GetResourceIndexByItemdata(woodItem)].building.GetComponent<BuildingBase>()))
        {
            GameObject spawnedBuilding = Instantiate(resourcesAndBuildings[GetResourceIndexByItemdata(woodItem)].building
                , originalPos, Quaternion.identity);

            if (Physics.Raycast(spawnedBuilding.transform.position + new Vector3(0, 1, 0), -Vector3.up, out RaycastHit hit, groundLayer))
            {
                spawnedBuilding.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                buildings.Add(spawnedBuilding.GetComponent<BuildingBase>());
                spawnedBuilding.GetComponent<BuildingBase>().SetResourceItemManagerByType(ResourceItemManager.Type.AI);
            }
        }
        else
        {
            print("Not enough resources for : " + resourcesAndBuildings[GetResourceIndexByItemdata(woodItem)].building.name);
        }
    }

    private BuildingBase GetBuildingIndexByType(ItemData itemData)
    {
        foreach (var building in buildings)
        {
            if (building.GetItemData() == itemData)
            {
                return building;
            }
        }

        return null;
    }

    private void AssignWorker(BuildingBase building, Worker worker)
    {
        if (availableWorkers.Contains(worker))
        {
            building.AddWorkerToBuilding(worker);
        }
        else
        {
            print("worker : " + worker + " not available");
        }
    }

    public ResourceObjectManager FindClosestResourceManager(ItemData itemdata)
    {
        ResourceObjectManager[] resourceManagers = FindObjectsOfType<ResourceObjectManager>();

        if (resourceManagers.Length <= 0) return null;

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
        float closestDistance = Mathf.Infinity;

        foreach (var resourceManager in validResourceManagers)
        {
            float distanceToResourceManager = Vector3.Distance(transform.position, resourceManager.transform.position);

            if (distanceToResourceManager <= closestDistance)
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
                return index;
            }
            index++;
        }

        return -1;
    }
}