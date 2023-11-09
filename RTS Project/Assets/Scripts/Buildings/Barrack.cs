using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Barrack : MonoBehaviour
{
    private List<GameObject> unitList = new List<GameObject>(CAPACITY);
    public List<GameObject> UnitList {
        get {
            return unitList;
        }
        set {
            unitList = value;
        }
    }
    
    [Header("Unit List")]
    [Tooltip("Change this to the Soldier")]
    public GameObject unitToSpawn;
    private const int CAPACITY = 5;
    private int currentIndex = 0;

    private bool canSpawn = false;

    [Header("Spawn Pos of Soldier")] 
    [SerializeField] private GameObject spawnPosSoldier;
    
    [Header("Barrack Properties")]
    [SerializeField] private float queue = 0f;  // queue to keep track of
    [SerializeField] private float maxTimeUntilNext = 5f;   // max time var, this will also be used on Invoke
    
    [SerializeField] private Transform barrackDoor1;
    [SerializeField] private Transform barrackDoor2;

    [Header("Unit Selection")] 
    public SelectionManager selectionManager;
    public NavMeshAgent unitAgent;

    private void Start() {
        selectionManager = GameObject.FindWithTag("SelectionManager").GetComponent<SelectionManager>();
        // unitAgent = GameObject.FindWithTag("AI").GetComponent<NavMeshAgent>();
    }

    private void Update() {
        if (canSpawn) {
            Counter();
        }

        if (unitList.Count > 0) {
            canSpawn = true;
            SpawnUnit();
        }
    }
    
    public void AddUnitToBarrack() {
        List<GameObject> selectedUnit = selectionManager.selectedUnits;
        foreach (var unit in selectedUnit) {
            unitAgent = unit.GetComponent<NavMeshAgent>();
            unitAgent.SetDestination(barrackDoor2.position);
        }
    }

    private void SpawnUnit() {
        if (queue >= maxTimeUntilNext) {
            if (unitList.Count > 0) {
                //TODO: Assign new gameobject as instantiated
                    // new gameobject.position = barrackdoor1.position
                if (unitList.Count > 0 && unitToSpawn != null && barrackDoor1 != null) {
                    GameObject newSoldier = Instantiate(unitList[0]);
                    newSoldier.transform.position = barrackDoor1.transform.position;
                    newSoldier.transform.localScale = new Vector3(1, 1, 1);
                    
                    //TODO: Move the soldier to a random position near the Barrack
                }

                unitList.RemoveAt(0);
            }
            
            // Check if the unitList is empty
            if (unitList.Count == 0) {
                canSpawn = false;
            }

            queue = 0;
        }
    }

    private float Counter() => queue += Time.deltaTime;
}
