using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Barrack : BuildingBase
{
    private int unitCount;

    [Header("Unit List")] [Tooltip("Change this to the Soldier")]
    [SerializeField] private GameObject unitToSpawnEnemy;
    [SerializeField] private GameObject unitToSpawnFriendly;

    private const int CAPACITY = 5;

    [Header("Barrack Properties")] 
    [SerializeField] private float queue = 0f; // queue to keep track of
    [SerializeField] private float maxTimeUntilNext = 5.0f; // max time var, this will also be used on Invoke
    [SerializeField] private float rangeOfSpawn = 10f;

    [Space]
    [SerializeField] private BarrackEntrance entrance;
    [SerializeField] private Transform exit;

    [Space] 
    [SerializeField] private Terrain terrain;

    [Header("Unit Selection")] public SelectionManager selectionManager;

    private SoldierUnit spawnedUnit;

    private Unit.TypeUnit typeUnit;

    protected override void Awake()
    {
        base.Awake();
        terrain = FindObjectOfType<Terrain>();
    }

    private void Start()
    {
        entrance.Setup(this);

        selectionManager = FindObjectOfType<SelectionManager>();
        
        // set both the entrance and exit to the height of the terrain
        float entranceHeight = terrain.SampleHeight(entrance.transform.position);
        float exitHeight = terrain.SampleHeight(exit.position);

        // set the y-coordinates
        entrance.transform.position = new Vector3(entrance.transform.position.x, entranceHeight, entrance.transform.position.z);
        exit.position = new Vector3(exit.position.x, exitHeight, exit.position.z);
    }

    protected override void Update()
    {
        base.Update();

        if (unitCount > 0)
        {
            StartCoroutine(ProcessSpawning());
        }
    }

    public void AddUnitToBarrack(GameObject AIUnit, Unit.TypeUnit typeUnit)
    {
        if (AIUnit == null)
        {
            List<GameObject> selectedUnit = selectionManager.selectedUnits;

            // Send the selectedUnits to the entrance
            foreach (var unit in selectedUnit)
            {
                if (unit.TryGetComponent(out NavMeshAgent agent))
                {
                    agent.destination = entrance.transform.position;
                }
                else
                {
                    Debug.LogError("Failed to gather Agent Component from unit");
                }
            }
        }
        else
        {
            this.typeUnit = typeUnit;

            if (AIUnit.TryGetComponent(out Worker worker))
            {
                if (worker.TryGetComponent(out NavMeshAgent agent))
                {
                    agent.SetDestination(entrance.transform.position);
                }
            }
        }
    }

    public SoldierUnit GetSpawnedSoldier() => spawnedUnit;

    private IEnumerator ProcessSpawning()
    {
        // Turn on the queue
        queue += Time.deltaTime;

        // Check if the list isn't empty, if so break the method
        if (unitCount != 0)
        {
            // Check if the queue hasn't surpassed the max Time
            if (queue >= maxTimeUntilNext)
            {
                // Spawn in Soldier
                SpawnSoldier();

                // Reset queue
                queue = 0.0f;
            }
        }
        else
            yield break;
    }

    private void SpawnSoldier()
    {
        /* DEBUGGING */

        NavMeshHit hit;
        if (NavMesh.SamplePosition(exit.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            // Instantiate the GameObject
            GameObject unitToSpawn = null;

            switch (typeUnit)
            {
                case Unit.TypeUnit.Enemy:
                    unitToSpawn = unitToSpawnEnemy;
                    break;

                case Unit.TypeUnit.Human:
                    unitToSpawn = unitToSpawnFriendly;
                    break;
            }

            GameObject soldierGO = Instantiate(unitToSpawn, hit.position, Quaternion.identity);

            soldierGO.GetComponent<Unit>().typeUnit = typeUnit;
            spawnedUnit = soldierGO.GetComponent<SoldierUnit>();

            // Remove unit out of the list
            unitCount--;

            // Generate a random position near the barrack
            Vector3 randomPosition = exit.position + new Vector3(Random.Range(-rangeOfSpawn, rangeOfSpawn),
                                        0 , Random.Range(-rangeOfSpawn, rangeOfSpawn));

            // Find the closest point on the Navmesh to the random pos
            NavMeshHit randomHit;
            if (NavMesh.SamplePosition(randomPosition, out randomHit, rangeOfSpawn, NavMesh.AllAreas))
            {
                // Set the soldier's destination to the randomPos
                NavMeshAgent agent = soldierGO.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.destination = randomHit.position;
                }
                else
                {
                    Debug.LogError("Failed to get Agent component from soldier!");
                }
            }
            else
            {
                Debug.LogError("Failed to find a pos on the NavMesh near the RandomPos");
            }
        }
        else
        {
            Debug.LogError("Failed to find a position on the NavMesh close to exit.position.");
        }
    }

    public void AIEnteredEntrance(Unit unit)
    {
        // Destroy the unit
        Destroy(unit.gameObject);

        // Add a new item to the list!
        unitCount++;
    }
}
