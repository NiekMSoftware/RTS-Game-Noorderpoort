using System.Collections.Generic;
using UnityEngine;

public class ComputerEnemy : MonoBehaviour
{
    [SerializeField] private float amountOfWorkersAtStart;
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private ResourceItemManager resources;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private Buildings[] resourcesAndBuildings;
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
    [SerializeField] private PointManager pointManager;
    [SerializeField] private float minResourcePoints;

    [SerializeField] private List<Worker> workers;
    [SerializeField] private List<Worker> availableWorkers;
    [SerializeField] private List<BuildingBase> buildings;
    [SerializeField] private AIStates state;

    private List<GameObject> resourceAreas = new();

    private float choiseTimer;

    private float playerOffenseDefenseScore;
    private float AIOffenseDefenseScore;

    public enum BuildingType
    {
        Resource,
        Defensive,
        Offensive
    }

    [System.Serializable]
    public class Buildings
    {
        public GameObject building;
        public BuildingType buildingType;
        public bool isStarter;
        public ItemData itemData;
    }

    public enum AIStates
    {
        Start,
        Exploring,
        Defending,
        PreparingAttack,
        Attacking
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
        switch (state)
        {
            case AIStates.Start:
                if (pointManager.GetPointsByType(PointManager.Type.AI).resourcePoints >= minResourcePoints)
                {
                    state = AIStates.Exploring;
                }
                break;

            case AIStates.Exploring:
                //Add exploring code

                UpdateScores();

                //if the players army is beter than the ai's, defend
                //otherwise prepare attack
                if (playerOffenseDefenseScore > AIOffenseDefenseScore)
                {
                    print("Player better, defending");
                    state = AIStates.Defending;
                }
                else
                {
                    print("AI better, attacking");
                    state = AIStates.PreparingAttack;
                }
                break;

            case AIStates.Defending:
                UpdateScores();

                if (playerOffenseDefenseScore < AIOffenseDefenseScore)
                {
                    state = AIStates.Attacking;
                }
                break;

            case AIStates.PreparingAttack:
                UpdateScores();

                if (playerOffenseDefenseScore < AIOffenseDefenseScore)
                {
                    state = AIStates.Attacking;
                }
                break;

            case AIStates.Attacking:
                //Select all soldiers and attack player
                break;
        }
    }

    private void UpdateScores()
    {
        playerOffenseDefenseScore = pointManager.GetPointsByType(PointManager.Type.Player).offensivePoints + pointManager.GetPointsByType(PointManager.Type.Player).defensivePoints;
        AIOffenseDefenseScore = pointManager.GetPointsByType(PointManager.Type.AI).offensivePoints + pointManager.GetPointsByType(PointManager.Type.AI).defensivePoints;
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
            BuildingBase spawnedBuilding = Instantiate(resourcesAndBuildings[GetResourceIndexByItemdata(woodItem)].building
                , originalPos, Quaternion.identity).GetComponent<BuildingBase>();

            if (Physics.Raycast(spawnedBuilding.transform.position + new Vector3(0, 1, 0), -Vector3.up, out RaycastHit hit, groundLayer))
            {
                spawnedBuilding.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                spawnedBuilding.SetResourceItemManagerByType(ResourceItemManager.Type.AI);
                pointManager.AddPoints(spawnedBuilding.GetPoints().pointsToReceive, spawnedBuilding.GetPoints().type, PointManager.Type.AI);
                buildings.Add(spawnedBuilding);
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