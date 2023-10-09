using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpecialUnitManager : MonoBehaviour
{
    [Header("Timer")]
    public GameObject specialUnitObject;
    private GameObject currentUnitObject;
    public float spawnTime;
    public float spawnDelay;
    public List<GameObject> UnitList = new();
    public bool GoDown = false;
    public bool stopSpawning = false;

    public SpecialUnit specialUnit;

    void Start()
    {
        InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
    }

    public void SpawnObject()
    {
        currentUnitObject = Instantiate(specialUnitObject, transform.position, transform.rotation);
        SpecialUnit currentSpecialUnit = currentUnitObject.GetComponent<SpecialUnit>();
        currentSpecialUnit.refSpecialUnitManager = this;
        UnitList.Add(currentUnitObject);

        if (UnitList.Count == 5)
        {
            stopSpawning = true;
        }      
        
        if (stopSpawning)
        {
            print("StopSpawning!!!!!!!");
            foreach(GameObject unit in UnitList)
            {
                print(unit.name);
                currentSpecialUnit = unit.GetComponent<SpecialUnit>();
                currentSpecialUnit.health--;                              
            }            
        }
        /*if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("stopspawing false");
            stopSpawning = false;
        }*/
    }
}
