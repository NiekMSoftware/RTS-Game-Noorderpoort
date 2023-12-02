using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Barrack : BuildingBase
{
    private List<GameObject> unitList = new List<GameObject>(CAPACITY);

    public List<GameObject> UnitList
    {
        get { return unitList; }
        set { unitList = value; }
    }

    [Header("Unit List")] [Tooltip("Change this to the Soldier")]
    public GameObject unitToSpawn;

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

    private Unit spawnedUnit;

    protected override void Awake()
    {
        base.Awake();
        terrain = FindObjectOfType<Terrain>().GetComponent<Terrain>();
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

    private void Update()
    {
        if (unitList.Count > 0)
        {
            StartCoroutine(ProcessSpawning());
        }
    }

    public void AddUnitToBarrack(Unit AIUnit)
    {
        if (AIUnit == null)
        {
            List<GameObject> selectedUnit = selectionManager.selectedUnits;
            print("Added Unit to list and sending them to the barrack");

            // Send the selectedUnits to the entrance
            foreach (var unit in selectedUnit)
            {
                NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
                if (agent != null)
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
            if (AIUnit.TryGetComponent(out NavMeshAgent agent))
            {
                print("Entrance position : " + entrance.transform.position);
                agent.destination = entrance.transform.position;
            }
        }
    }

    public Unit GetSpawnedSoldier() => spawnedUnit;

    private IEnumerator ProcessSpawning()
    {
        // Turn on the queue
        queue += Time.deltaTime;

        // Check if the list isn't empty, if so break the method
        if (unitList.Count != 0)
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
            GameObject soldierGO = Instantiate(unitToSpawn, hit.position, Quaternion.identity);
            
            // Remove unit out of the list
            unitList.RemoveAt(0);

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
        unitList.Add(unitToSpawn);
    }
}
