using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ComputerEnemy : MonoBehaviour
{
    [SerializeField] private float amountOfWorkersAtStart;
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private ResourceItemManager resources;
    [SerializeField] private BuildingManager buildingManager;
    [SerializeField] private Buildings[] buildings;
    [SerializeField] private ItemData woodItem;
    [SerializeField] private ItemData stoneItem;
    [SerializeField] private ItemData metalItem;
    [SerializeField] private Terrain terrain;
    [SerializeField] private LayerMask occupanyLayer;
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("Higher is more accurate but also less performent")]
    [SerializeField] private float buildingPlaceAccuracy = 50;
    [SerializeField] private float amountOfSecondsPerChoise = 3;
    [SerializeField] private ResourceItemManager resourceItemManager;
    [SerializeField] private PointManager pointManager;
    [SerializeField] private float minResourcePoints;

    [SerializeField] private List<Worker> workers;
    [SerializeField] private List<Worker> availableWorkers;
    [SerializeField] private List<BuildingBase> placedBuildings;
    [SerializeField] private AIStates state;

    private List<GameObject> resourceAreas = new();

    private float choiseTimer;

    private PointManager.Points aiPoints;
    private PointManager.Points playerPoints;

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
        Check,
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

        PlaceResourceBuilding(buildings[GetResourceIndexByItemdata(woodItem)].itemData);
        AssignWorker(GetBuildingIndexByType(woodItem), availableWorkers[0]);

        aiPoints = pointManager.GetPointsByType(PointManager.Type.AI);
        playerPoints = pointManager.GetPointsByType(PointManager.Type.Player);
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
                foreach (var building in buildings)
                {
                    //if building is a starter building and isn't built yet, build it.
                    if (building.isStarter && !placedBuildings.Contains(building.building.GetComponent<BuildingBase>()))
                    {
                        PlaceBuilding(building.building);
                    }
                }

                if (aiPoints.resourceScore >= minResourcePoints)
                {
                    state = AIStates.Exploring;
                }
                break;

            case AIStates.Exploring:
                //Add exploring code

                state = AIStates.Check;

                break;

            case AIStates.Check:
                //if the players army is beter than the ai's, defend
                //otherwise prepare attack
                if (playerPoints.warScore > aiPoints.warScore)
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
                //Build defending buildings and create defending troops

                if (playerPoints.defensiveScore > aiPoints.defensiveScore)
                {
                    state = AIStates.Check;
                }
                break;

            case AIStates.PreparingAttack:
                //Build barracks and create offensive troops

                if (aiPoints.offensiveScore < playerPoints.offensiveScore)
                {
                    state = AIStates.Check;
                }
                break;

            case AIStates.Attacking:
                //Select all soldiers and attack player
                break;
        }
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

    private void PlaceBuilding(GameObject building)
    {
        //Make the building be placed as close as possible to the main building/computerenemy's location

        Vector3 direction = transform.position - new Vector3(0, Random.Range(0, 360), 0);
        Debug.DrawRay(transform.position, direction * 1000, Color.green, 30);

        Vector3 originalPos = building.transform.position;

        while (Physics.CheckSphere(originalPos, 5f, occupanyLayer))
        {
            originalPos += direction / buildingPlaceAccuracy;
        }

        originalPos = Vector3Int.FloorToInt(originalPos);

        originalPos.y = terrain.SampleHeight(originalPos) +
            buildings[GetResourceIndexByItemdata(woodItem)].building.transform.localScale.y;

        if (HasEnoughResources(buildings[GetResourceIndexByItemdata(woodItem)].building.GetComponent<BuildingBase>()))
        {
            BuildingBase spawnedBuilding = Instantiate(building, originalPos, Quaternion.identity).GetComponent<BuildingBase>();

            if (Physics.Raycast(spawnedBuilding.transform.position + new Vector3(0, 1, 0), -Vector3.up, out RaycastHit hit, groundLayer))
            {
                spawnedBuilding.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                spawnedBuilding.SetResourceItemManagerByType(ResourceItemManager.Type.AI);
                pointManager.AddPoints(spawnedBuilding.GetPoints().pointsToReceive, spawnedBuilding.GetPoints().type, PointManager.Type.AI);
                placedBuildings.Add(spawnedBuilding);
            }
        }
        else
        {
            print("Not enough resources for : " + buildings[GetResourceIndexByItemdata(woodItem)].building.name);
        }
    }

    private void PlaceResourceBuilding(ItemData itemData)
    {
        ResourceObjectManager closestResource = FindClosestResourceManager(itemData);

        Vector3 direction = transform.position - closestResource.transform.position;
        Debug.DrawRay(closestResource.transform.position, direction * 1000, Color.green, 30);

        Vector3 originalPos = closestResource.transform.position;

        while (Physics.CheckSphere(originalPos, 5f, occupanyLayer))
        {
            originalPos += direction / buildingPlaceAccuracy;
        }

        originalPos = Vector3Int.FloorToInt(originalPos);

        originalPos.y = terrain.SampleHeight(originalPos) +
            buildings[GetResourceIndexByItemdata(woodItem)].building.transform.localScale.y;

        if (HasEnoughResources(buildings[GetResourceIndexByItemdata(woodItem)].building.GetComponent<BuildingBase>()))
        {
            BuildingBase spawnedBuilding = Instantiate(buildings[GetResourceIndexByItemdata(woodItem)].building
                , originalPos, Quaternion.identity).GetComponent<BuildingBase>();

            if (Physics.Raycast(spawnedBuilding.transform.position + new Vector3(0, 1, 0), -Vector3.up, out RaycastHit hit, groundLayer))
            {
                spawnedBuilding.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                spawnedBuilding.SetResourceItemManagerByType(ResourceItemManager.Type.AI);
                pointManager.AddPoints(spawnedBuilding.GetPoints().pointsToReceive, spawnedBuilding.GetPoints().type, PointManager.Type.AI);
                placedBuildings.Add(spawnedBuilding);
            }
        }
        else
        {
            print("Not enough resources for : " + buildings[GetResourceIndexByItemdata(woodItem)].building.name);
        }
    }

    private BuildingBase GetBuildingIndexByType(ItemData itemData)
    {
        foreach (var building in placedBuildings)
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

        foreach (var building in buildings)
        {
            if (building.itemData == itemData)
            {
                return index;
            }
            index++;
        }

        return -1;
    }
}