using System.Collections.Generic;
using UnityEngine;

public class ComputerEnemy : MonoBehaviour
{
    [SerializeField] private float amountOfWorkersAtStart;
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private Buildings[] buildings;
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
    [SerializeField] private int pointsToAddAssignWorker;

    private List<GameObject> resourceAreas = new();

    private float choiseTimer;

    private Points aiPoints;
    private Points playerPoints;

    [SerializeField] private List<MissingResource> resourcesToGather;

    [System.Serializable]
    public class MissingResource
    {
        public MissingResource(int amount, ItemData item)
        {
            this.amount = amount;
            this.item = item;
        }

        public int amount;
        public ItemData item;
    }

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
        public bool hasBeenPlaced;
    }

    public enum AIStates
    {
        Start,
        ResourceGathering,
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

        aiPoints = pointManager.GetPointsByType(PointManager.EntityType.AI);
        playerPoints = pointManager.GetPointsByType(PointManager.EntityType.Player);
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
                bool placedAllStarterBuildings = true;

                foreach (var building in buildings)
                {
                    //if building is a starter building and isn't built yet, build it.
                    if (building.isStarter && !building.hasBeenPlaced)
                    {
                        placedAllStarterBuildings = false;
                        if (building.buildingType == BuildingType.Resource)
                        {
                            PlaceResourceBuilding(building.itemData);
                        }
                        else
                        {
                            PlaceBuilding(building);
                        }
                    }
                }

                bool enoughPoints = false;

                foreach (var resourcePoint in aiPoints.resourcePoints)
                {
                    print(resourcePoint.item);
                    print(GetBuildingByType(resourcePoint.item).building);
                    if (GetBuildingByType(resourcePoint.item).isStarter)
                    {
                        if (resourcePoint.amount >= minResourcePoints)
                        {
                            enoughPoints = true;
                        }
                        else
                        {
                            enoughPoints = false;
                        }
                    }
                }

                if (enoughPoints)
                {
                    if (aiPoints.totalResourceScore >= minResourcePoints)
                    {
                        state = AIStates.Exploring;
                    }
                    else
                    {
                        float lowestResourcePoints = Mathf.Infinity;
                        PointManager.ResourcePoint lowestResourcePoint = null;

                        foreach (var resourcePoint in aiPoints.resourcePoints)
                        {
                            if (resourcePoint.amount < lowestResourcePoints)
                            {
                                lowestResourcePoints = resourcePoint.amount;
                                lowestResourcePoint = resourcePoint;
                            }
                        }

                        PlaceResourceBuilding(lowestResourcePoint.item);
                    }
                }
                break;

            case AIStates.ResourceGathering:
                if (resourcesToGather.Count > 0)
                {
                    if (resourceItemManager.GetSlotByItemData(resourcesToGather[0].item).amount >= resourcesToGather[0].amount)
                    {
                        resourcesToGather.RemoveAt(0);
                        print("enough resources");
                        state = AIStates.Check;
                        return;
                    }

                    bool shouldPlaceNewBuilding = false;
                    ItemData itemData = null;

                    print("not enough resources");
                    foreach (var placedBuilding in placedBuildings)
                    {
                        foreach (var resource in resourcesToGather)
                        {
                            if (resource.item == placedBuilding.GetItemData())
                            {
                                //Add workers
                                for (int i = 0; i < placedBuilding.GetMaxWorkers() + 1; i++)
                                {
                                    if (availableWorkers.Count > 0)
                                    {
                                        if (placedBuilding.GetWorkers().Count <= i)
                                        {
                                            AssignWorker(placedBuilding, GetRandomAvailableWorker());
                                            return;
                                        }
                                        else
                                        {
                                            print("building already full of workers" + placedBuilding.GetItemData());
                                            //Place new building at new resource
                                        }
                                    }
                                    else
                                    {
                                        //Place house
                                        print("no workers available");
                                        return;
                                    }
                                }
                            }
                        }
                    }

                }
                break;

            case AIStates.Exploring:
                //Add exploring code

                state = AIStates.Check;

                break;

            case AIStates.Check:
                //if the players army is beter than the ai's, defend
                //otherwise prepare attack

                bool enoughPoints2 = false;

                foreach (var resourcePoint in aiPoints.resourcePoints)
                {
                    if (GetBuildingByType(resourcePoint.item).isStarter)
                    {
                        if (aiPoints.GetResourcePointByItem(resourcePoint.item).amount >= minResourcePoints)
                        {
                            enoughPoints2 = true;
                        }
                        else
                        {
                            enoughPoints2 = false;
                        }
                    }
                }

                if (enoughPoints2)
                {
                    state = AIStates.Exploring;
                }
                else
                {
                    if (playerPoints.warScore > aiPoints.warScore)
                    {
                        print("Player better, defending");
                        state = AIStates.Defending;
                    }
                    else
                    {
                        print("AI better, preparing attack");
                        state = AIStates.PreparingAttack;
                    }
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

    private void PlaceBuilding(Buildings building)
    {
        //Make the building be placed as close as possible to the main building/computerenemy's location

        Vector3 randomDirection = new(Random.insideUnitCircle.normalized.x, 0, Random.insideUnitCircle.y);
        Debug.DrawRay(transform.position, randomDirection * 1000, Color.blue, 30);

        Vector3 originalPos = transform.position;

        while (Physics.CheckSphere(originalPos, 5f, occupanyLayer))
        {
            originalPos += randomDirection / buildingPlaceAccuracy;
        }

        originalPos = Vector3Int.FloorToInt(originalPos);

        originalPos.y = terrain.SampleHeight(originalPos) +
            buildings[GetResourceIndexByItemdata(building.itemData)].building.transform.localScale.y;

        if (HasEnoughResources(buildings[GetResourceIndexByItemdata(building.itemData)].building.GetComponent<BuildingBase>()))
        {
            BuildingBase spawnedBuilding = Instantiate(building.building, originalPos, Quaternion.identity).GetComponent<BuildingBase>();

            if (Physics.Raycast(spawnedBuilding.transform.position + new Vector3(0, 1, 0), -Vector3.up, out RaycastHit hit, groundLayer))
            {
                spawnedBuilding.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                spawnedBuilding.SetResourceItemManagerByType(ResourceItemManager.Type.AI);
                spawnedBuilding.SetOccupancyType(BuildingBase.OccupancyType.Enemy);
                pointManager.AddPoints(spawnedBuilding.GetPoints().amount, spawnedBuilding.GetPoints().pointType, PointManager.EntityType.AI,
                    aiPoints.GetResourcePointByItem(building.itemData));
                placedBuildings.Add(spawnedBuilding);
                building.hasBeenPlaced = true;
            }
        }
        else
        {
            print("Not enough resources for : " + buildings[GetResourceIndexByItemdata(building.itemData)].building.name);
            resourcesToGather.Add(new MissingResource(building.building.GetComponent<BuildingBase>().GetRecipes()[0].amountNeeded,
                building.itemData));
            state = AIStates.ResourceGathering;
        }
    }

    private void PlaceResourceBuilding(ItemData itemData)
    {
        //Find closest resource
        ResourceObjectManager closestResource = FindClosestResourceManager(itemData);

        Vector3 direction = transform.position - closestResource.transform.position;
        Debug.DrawRay(closestResource.transform.position, direction * 1000, Color.green, 30);

        Vector3 originalPos = closestResource.transform.position;

        //try and place it as close as possible to the closest resource without placing it inside anything
        while (Physics.CheckSphere(originalPos, 5f, occupanyLayer))
        {
            originalPos += direction / buildingPlaceAccuracy;
        }

        originalPos = Vector3Int.FloorToInt(originalPos);

        //Get building by resource
        Buildings building = buildings[GetResourceIndexByItemdata(itemData)];

        //Set y height to terrain
        originalPos.y = terrain.SampleHeight(originalPos) + building.building.transform.localScale.y;

        if (HasEnoughResources(building.building.GetComponent<BuildingBase>()))
        {
            BuildingBase spawnedBuilding = Instantiate(building.building, originalPos, Quaternion.identity).GetComponent<BuildingBase>();

            if (Physics.Raycast(spawnedBuilding.transform.position + new Vector3(0, 1, 0), -Vector3.up, out RaycastHit hit, groundLayer))
            {
                spawnedBuilding.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                spawnedBuilding.SetResourceItemManagerByType(ResourceItemManager.Type.AI);
                spawnedBuilding.SetOccupancyType(BuildingBase.OccupancyType.Enemy);
                pointManager.AddPoints(spawnedBuilding.GetPoints().pointsToReceive, spawnedBuilding.GetPoints().type, PointManager.Type.AI, 
                    pointManager.GetPointsByType(PointManager.Type.AI).GetResourcePointByItem(building.building.GetComponent<BuildingBase>().GetRecipes()[0].data));
                placedBuildings.Add(spawnedBuilding);
                building.hasBeenPlaced = true;
            }
        }
        else
        {
            print("Not enough resources for : " + building.building.name);
            resourcesToGather.Add(new MissingResource(building.building.GetComponent<BuildingBase>().GetRecipes()[0].amountNeeded,
                building.building.GetComponent<BuildingBase>().GetRecipes()[0].data));
            state = AIStates.ResourceGathering;
        }
    }

    private BuildingBase GetPlacedBuildingByType(ItemData itemData)
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

    private Buildings GetBuildingByType(ItemData itemData)
    {
        foreach (var building in buildings)
        {
            if (building.itemData == itemData)
            {
                return building;
            }
        }

        return null;
    }

    private Worker GetRandomAvailableWorker()
    {
        int randomNum = Random.Range(0, availableWorkers.Count);

        if (availableWorkers[randomNum])
        {
            return availableWorkers[randomNum];
        }
        else
        {
            print("no worker available");
            return null;
        }
    }

    private void AssignWorker(BuildingBase building, Worker worker)
    {
        if (availableWorkers.Contains(worker))
        {
            if (building.AddWorkerToBuilding(worker))
            {
                print("assigned worker");
                pointManager.AddPoints(pointsToAddAssignWorker, building.GetPoints().pointType, PointManager.EntityType.AI,
                    aiPoints.GetResourcePointByItem(building.GetItemData()));
            }
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
