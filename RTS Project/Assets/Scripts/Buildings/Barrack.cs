using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Barrack : MonoBehaviour
{
    // TODO: Clear out all pending Unit's in barrack and release them
        //  (Release them, is clearing them out of the list 1 by 1 until 0 remains)
        
    [Header("Unit List")]
    [SerializeField] private List<GameObject> unitList = new List<GameObject>(CAPACITY);
    public List<GameObject> UnitList {
        get {
            return unitList;
        }
        set {
            unitList = value;
        }
    }
    
    [Tooltip("Change this to the Soldier")]
    public GameObject unitToSpawn;
    private const int CAPACITY = 5;
    private int currentIndex = 0;

    private bool canSpawn = false;
    
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

        Time.timeScale = 1f;
    }

    private void Update() {
        Counter();

        if (queue >= maxTimeUntilNext)
            canSpawn = true;
        
        if(canSpawn)
            SpawnEnemy();
    }
    
    public void AddUnitToBarrack() {
        List<GameObject> selectedUnit = selectionManager.selectedUnits;
        foreach (var unit in selectedUnit) {
            unitAgent = unit.GetComponent<NavMeshAgent>();
            unitAgent.SetDestination(barrackDoor2.position);
        }
    }

    private void SpawnEnemy() {
        print("spawning soldier");
        if (currentIndex < unitList.Count) {
            Instantiate(unitList[currentIndex], barrackDoor1);
            currentIndex++;
        }
    }

    private float Counter() => queue += Time.deltaTime;
}
