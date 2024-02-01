using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ComputerEnemy : MonoBehaviour
{
    [SerializeField] private float amountOfWorkersAtStart;
    [SerializeField] private GameObject workerPrefab;
    [SerializeField] private Buildings[] buildings;
    [SerializeField] private Material buildingMaterial;
    [SerializeField] private GameObject buildingParticle;
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
    [SerializeField] private List<SoldierUnit> soldiers;
    [SerializeField] public List<BuildingBase> placedBuildings;
    [SerializeField] private AIStates state;
    [SerializeField] private int pointsToAddAssignWorker;
    [SerializeField] private bool debugMode;
    [SerializeField] private PlayerWorkerSpawner workerSpawner;

    private List<GameObject> resourceAreas = new();

    private float choiseTimer;

    private Points aiPoints;
    private Points playerPoints;

    private bool hasExplored;

    private Vector3 playerMainBuildingPosition;

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
        public float buildTime;
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

    private void Awake()
    {
        aiPoints = pointManager.GetPointsByType(PointManager.EntityType.AI);
        playerPoints = pointManager.GetPointsByType(PointManager.EntityType.Player);
    }

    private void Start()
    {
        workers = workerSpawner.GetWorkers().ToList();

        foreach (var worker in workers)
        {
            worker.typeUnit = Unit.TypeUnit.Enemy;
        }

        availableWorkers = workers;
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

    #region State Machine

    private void MakeChoise()
    {
        //if (hasTrainedUnit) return;

        switch (state)
        {
            case AIStates.Start:
                //Place all starter buildings
                foreach (var building in buildings)
                {
                    if (building.isStarter && !building.hasBeenPlaced)
                    {
                        //if it's a resource building, place a resourcebuilding, otherwise, just place a normal building
                        if (building.buildingType == BuildingType.Resource)
                        {
                            PlaceResourceBuilding(building.itemData);
                        }
                        else
                        {
                            PlaceNonResourceBuilding(building);
                        }
                    }
                }

                //Check if the AI can continue to the exploring state
                if (CheckStartState())
                {
                    state = AIStates.Exploring;
                }
                else
                {
                    //if not, increase points, by placing buildings and assigning workers
                    List<PointManager.ResourcePoint> startingBuildingItems = new();

                    //Get all the starterbuildings itemdata
                    foreach (var building in buildings)
                    {
                        if (building.isStarter)
                        {
                            startingBuildingItems.Add(aiPoints.GetResourcePointByItem(building.itemData));
                        }
                    }

                    float lowestResourcePoints = Mathf.Infinity;
                    PointManager.ResourcePoint lowestResourcePoint = null;

                    //Get the building with the lowest resource points, and that is a starter building
                    foreach (var item in startingBuildingItems)
                    {
                        foreach (var resourcePoint in aiPoints.resourcePoints)
                        {
                            if (resourcePoint.amount < lowestResourcePoints && item == resourcePoint)
                            {
                                lowestResourcePoints = resourcePoint.amount;
                                lowestResourcePoint = resourcePoint;
                            }
                        }
                    }

                    //Add more points
                    //TODO: change this to maybe have a gain points function/state and just say add to 2 more points to whatever.
                    if (resourcesToGather.Count <= 0)
                    {
                        resourcesToGather.Add(new MissingResource(
                            resourceItemManager.GetSlotByItemData(lowestResourcePoint.item).amount + 50,
                            lowestResourcePoint.item));
                        state = AIStates.ResourceGathering;
                    }
                }
                break;

            case AIStates.ResourceGathering:
                if (resourcesToGather.Count > 0)
                {
                    if (resourceItemManager.GetSlotByItemData(resourcesToGather[0].item).amount >= resourcesToGather[0].amount)
                    {
                        resourcesToGather.RemoveAt(0);
                        if (debugMode)
                            print("enough resources");
                        state = AIStates.Check;
                        return;
                    }

                    bool shouldPlaceNewBuilding = false;
                    ItemData itemData = null;

                    if (debugMode)
                        print("not enough resources");
                    foreach (var placedBuilding in placedBuildings)
                    {
                        if (placedBuilding is ResourceBuildingBase placedResourceBuilding)
                        {
                            foreach (var resource in resourcesToGather)
                            {
                                if (resource.item == placedResourceBuilding.GetItemData())
                                {
                                    //Add workers
                                    for (int i = 0; i < placedResourceBuilding.GetMaxWorkers() + 1; i++)
                                    {
                                        if (availableWorkers.Count > 0)
                                        {
                                            if (placedResourceBuilding.GetWorkers().Count <= i)
                                            {
                                                shouldPlaceNewBuilding = false;
                                                Worker worker = GetRandomAvailableWorker();

                                                if (AssignWorker(placedResourceBuilding, worker)) continue;
                                            }
                                            else
                                            {
                                                shouldPlaceNewBuilding = true;
                                                itemData = placedResourceBuilding.GetItemData();
                                                if (debugMode)
                                                    print("building already full of workers " + placedResourceBuilding.GetItemData());
                                                //Place new building at new resource
                                            }
                                        }
                                        else
                                        {
                                            //Place house
                                            if (debugMode)
                                                print("no workers available");
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (shouldPlaceNewBuilding)
                    {
                        if (debugMode)
                            print("place resource building, because not enough");
                        PlaceResourceBuilding(itemData);
                    }

                }
                break;

            case AIStates.Exploring:
                //Add exploring code

                playerMainBuildingPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

                hasExplored = true;

                state = AIStates.Check;

                break;

            case AIStates.Check:
                if (!hasExplored)
                {
                    if (CheckStartState())
                    {
                        state = AIStates.Exploring;
                    }
                    else
                    {
                        state = AIStates.Start;
                    }
                }
                else
                {
                    //if the players army is beter than the ai's, defend
                    //otherwise prepare attack
                    //if (playerPoints.warScore > aiPoints.warScore)
                    if (false)
                    {
                        if (debugMode)
                            print("Player better, defending");
                        state = AIStates.Defending;
                    }
                    else
                    {
                        if (soldiers.Count > 0)
                        {
                            if (debugMode)
                                print("AI has enough soldiers to attack");
                            state = AIStates.Attacking;
                        }
                        else
                        {
                            if (debugMode)
                                print("AI better, preparing attack");
                            state = AIStates.PreparingAttack;
                        }
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
                Barrack barrackBuilding = null;
                foreach (var building in buildings)
                {
                    if (building.buildingType == BuildingType.Offensive)
                    {
                        if (building.building.TryGetComponent(out barrackBuilding))
                        {
                            if (!building.hasBeenPlaced)
                            {
                                PlaceNonResourceBuilding(building);
                            }
                        }
                    }
                }

                BuildingBase placedBarrackBuildingBase = GetPlacedBuildingByType(barrackBuilding);

                if(placedBarrackBuildingBase.TryGetComponent(out Barrack placedBarrack))
                {
                    Worker worker = GetRandomAvailableWorker();

                    if (worker)
                    {
                        placedBarrack.AddUnitToBarrack(worker.gameObject, Unit.TypeUnit.Enemy);
                        availableWorkers.Remove(worker);
                    }

                    if (placedBarrack.GetSpawnedSoldier() != null)
                    {
                        SoldierUnit soldierUnit = placedBarrack.GetSpawnedSoldier();
                        if (soldiers.Contains(soldierUnit)) return;

                        soldierUnit.typeUnit = Unit.TypeUnit.Enemy;

                        soldiers.Add(soldierUnit);
                    }
                }

                state = AIStates.Check;
                break;

            case AIStates.Attacking:
                //Select all soldiers and attack player
                foreach (var soldier in soldiers)
                {
                    NavMeshPath path = new();
                    soldier.GetComponent<NavMeshAgent>().CalculatePath(playerMainBuildingPosition, path);
                    soldier.GetComponent<NavMeshAgent>().SetDestination(playerMainBuildingPosition);
                }

                soldiers.Clear();
                break;
        }
    }
    #endregion

    #region Place Building Logic

    private void PlaceBuilding(Vector3 position, Buildings building)
    {
        //snap position to an int value
        position = Vector3Int.FloorToInt(position);

        //Set Y position to that of the terrain
        position.y = terrain.SampleHeight(position) + (building.building.transform.lossyScale.y / 2);

        if (HasEnoughResources(building.building.GetComponent<BuildingBase>()))
        {
            //Spawn building
            GameObject spawnedBuilding = Instantiate(building.building, position, Quaternion.identity);

            BuildingBase spawnedBuildingBase = spawnedBuilding.GetComponent<BuildingBase>();

            spawnedBuildingBase.Init(buildingMaterial, buildingParticle, spawnedBuilding, building.buildTime, BuildingBase.States.Building);

            spawnedBuilding.TryGetComponent(out ResourceBuildingBase resourceBuildingBase);

            if (Physics.Raycast(spawnedBuilding.transform.position + new Vector3(0, 1, 0), -Vector3.up, out RaycastHit hit, groundLayer))
            {
                //Set rotation to that of the terrain/raycasthit
                spawnedBuilding.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                if (resourceBuildingBase)
                {
                    //Set resource type
                    resourceBuildingBase.SetResourceItemManagerByType(ResourceItemManager.Type.AI);
                }
                //set occupancy type
                spawnedBuildingBase.SetOccupancyType(BuildingBase.OccupancyType.Enemy);

                //add points
                pointManager.AddPoints(spawnedBuildingBase.GetPoints().amount, spawnedBuildingBase.GetPoints().pointType, PointManager.EntityType.AI,
                    pointManager.GetPointsByType(PointManager.EntityType.AI).GetResourcePointByItem(building.itemData));

                placedBuildings.Add(spawnedBuildingBase);
                building.hasBeenPlaced = true;
            }
        }
        else
        {
            BuildingBase buildingBase = building.building.GetComponent<BuildingBase>();
            Recipe recipe = buildingBase.GetRecipes()[0];
            if (debugMode)
                print("Not enough resources for : " + building.building.name);
            resourcesToGather.Add(new MissingResource(recipe.amountNeeded, recipe.data));
            state = AIStates.ResourceGathering;
        }
    }

    private void PlaceNonResourceBuilding(Buildings building)
    {
        //Make the building be placed as close as possible to the main building/computerenemy's location

        Vector3 randomDirection = new(Random.insideUnitCircle.normalized.x, 0, Random.insideUnitCircle.y);
        Debug.DrawRay(transform.position, randomDirection * 1000, Color.blue, 30);

        Vector3 originalPos = transform.position;

        while (Physics.CheckSphere(originalPos, 5f, occupanyLayer))
        {
            originalPos += randomDirection / buildingPlaceAccuracy;
        }

        PlaceBuilding(originalPos, building);
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

        //Get building by resource
        Buildings building = GetResourceBuildingByItemdata(itemData);

        PlaceBuilding(originalPos, building);
    }

    #endregion

    #region Getters

    private bool CheckStartState()
    {
        bool placedAllStarterBuildings = true;

        foreach (var building in buildings)
        {
            //if building is a starter building and isn't built yet, build it.
            if (building.isStarter && !building.hasBeenPlaced)
            {
                placedAllStarterBuildings = false;
            }
        }

        bool enoughPoints = false;

        foreach (var resourcePoint in aiPoints.resourcePoints)
        {
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

        if (enoughPoints && placedAllStarterBuildings)
        {
            return true;
        }

        return false;
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

    private BuildingBase GetPlacedBuildingByType(BuildingBase building)
    {
        foreach (var placedBuilding in placedBuildings)
        {
            if (placedBuilding.GetType() == building.GetType())
            {
                return placedBuilding;
            }
        }

        return null;
    }

    private Worker GetRandomAvailableWorker()
    {
        if (availableWorkers.Count <= 0)
        {
            if (debugMode)
                print("no worker available");
            return null;
        }

        int randomNum = Random.Range(0, availableWorkers.Count);

        if (availableWorkers[randomNum])
        {
            return availableWorkers[randomNum];
        }
        else
        {
            if (debugMode)
                print("no worker available");
            return null;
        }
    }

    private bool AssignWorker(ResourceBuildingBase building, Worker worker)
    {
        if (availableWorkers.Contains(worker))
        {
            if (building.AddWorkerToBuilding(worker))
            {
                if (debugMode)
                    print("assigned worker");
                availableWorkers.Remove(worker);
                pointManager.AddPoints(pointsToAddAssignWorker, building.GetPoints().pointType, PointManager.EntityType.AI,
                    aiPoints.GetResourcePointByItem(building.GetItemData()));
                return true;
            }
        }
        else
        {
            if (debugMode)
                print("worker : " + worker + " not available");
            return false;
        }

        return false;
    }

    public ResourceObjectManager FindClosestResourceManager(ItemData itemdata)
    {
        ResourceObjectManager[] resourceManagers = FindObjectsOfType<ResourceObjectManager>();

        if (resourceManagers.Length <= 0) return null;

        List<ResourceObjectManager> validResourceManagers = new();

        foreach (var resourceManager in resourceManagers)
        {
            //If the resource manager has resources at all
            if (resourceManager.resources.Count > 0)
            {
                //When it is the correct resource item
                if (resourceManager.resources[0].GetComponent<ResourceObject>().slot.data == itemdata)
                {
                    //When the AI did not already place a building here
                    if (!resourceManager.placedBuilding)
                    {
                        validResourceManagers.Add(resourceManager);
                    }
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

        closestResource.placedBuilding = true;
        return closestResource;
    }

    public Buildings GetResourceBuildingByItemdata(ItemData itemData)
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

    #endregion
}
