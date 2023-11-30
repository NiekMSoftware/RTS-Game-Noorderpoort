using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Barrack : BuildingBase
{
    private List<GameObject> unitList = new List<GameObject>(CAPACITY);
    public List<GameObject> UnitList
    {
        get
        {
            return unitList;
        }
        set
        {
            unitList = value;
        }
    }

    [Header("Unit List")]
    [Tooltip("Change this to the Soldier")]
    public GameObject unitToSpawn;
    private const int CAPACITY = 5;
    private int currentIndex = 0;

    private bool canSpawn = false;
    private bool hasSpawned = false;

    [Header("Spawn Pos of Soldier")]
    [SerializeField] private GameObject spawnPosSoldier;

    [Header("Barrack Properties")]
    [SerializeField] private float queue = 0f;  // queue to keep track of
    [SerializeField] private float maxTimeUntilNext = 5f;   // max time var, this will also be used on Invoke

    [SerializeField] private Barrack_Door barrackDoor1;
    [SerializeField] private Transform barrackDoor2;

    [Header("Unit Selection")]
    public SelectionManager selectionManager;
    public NavMeshAgent unitAgent;

    private Unit spawnedUnit;

    private void Start()
    {
        selectionManager = FindObjectOfType<SelectionManager>();
        // unitAgent = GameObject.FindWithTag("AI").GetComponent<NavMeshAgent>();

        barrackDoor1.SetBarrack(this);
    }

    private void Update()
    {
        if (canSpawn)
        {
            Counter();
        }

        if (unitList.Count > 0)
        {
            canSpawn = true;
            SpawnUnit();
        }
    }

    public void AddUnitToBarrack(Unit AIUnit)
    {
        //if (AIUnit == null)
        //{
        //    List<GameObject> selectedUnit = selectionManager.selectedUnits;
        //    foreach (var unit in selectedUnit)
        //    {
        //        unitAgent = unit.GetComponent<NavMeshAgent>();
        //        unitAgent.SetDestination(barrackDoor1.transform.position);
        //    }
        //}
        //else
        //{
        //    NavMeshAgent unitAgent = AIUnit.GetComponent<NavMeshAgent>();
        //    Vector3 worldPos = transform.TransformPoint(barrackDoor1.transform.position);
        //    unitAgent.SetDestination(worldPos);

        //    GameObject gameObject = new();

        //    gameObject.transform.position = worldPos;
        //}

        if (AIUnit != null)
        {
            List<GameObject> selectedUnit = selectionManager.selectedUnits;
            print($"Added units to {selectedUnit}");
        }
    }

    public Unit GetSpawnedSoldier() => spawnedUnit;

    private void SpawnUnit()
    {
        //if (queue >= maxTimeUntilNext)
        //{
        //    if (unitList.Count > 0)
        //    {
        //        if (!hasSpawned) {
        //            //TODO: Assign new gameobject as instantiated
        //            // new gameobject.position = barrackdoor1.position
        //            if (unitList.Count > 0 && unitToSpawn != null && barrackDoor1 != null)
        //            {
        //                print("Spawning Soldier");
        //                spawnedUnit = Instantiate(unitList[0]).GetComponent<Unit>();
                        
        //                spawnedUnit.transform.position = barrackDoor1.transform.position;
        //                spawnedUnit.transform.localScale = new Vector3(1, 1, 1);

        //                unitList.RemoveAt(0);
        //                hasSpawned = true;

        //                //TODO: Move the soldier to a random position near the Barrack
        //            }
        //        }
        //    }

        //    // Check if the unitList is empty
        //    if (unitList.Count == 0)
        //    {
        //        // Turn off everything related
        //        canSpawn = false;
        //        hasSpawned = false;
        //        queue = 0;
        //    }
        //}
    }

    private float Counter() => queue += Time.deltaTime;
}
