using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpecialUnitManager : MonoBehaviour
{
    [Header("Timer")]
    public GameObject specialUnitObject;
    public GameObject currentUnitObject;
    public float spawnTime;
    public float spawnDelay;
    List<GameObject> UnitList;
    public bool GoDown = false;
    public bool stopSpawning = false;

    public SpecialUnit specialUnit;

    // Start is called before the first frame update
    void Start()
    {
    UnitList = new();
    InvokeRepeating("SpawnObject", spawnTime, spawnDelay);

    }
    public void RemoveThis(GameObject DeletedUnit)
    {
        UnitList.Remove(DeletedUnit);
        DestroyImmediate(DeletedUnit, true);
        print("unit removed");
    }
    public void SpawnObject()
    {
        currentUnitObject = Instantiate(specialUnitObject, transform.position, transform.rotation);
        SpecialUnit currentSpecialUnit = currentUnitObject.GetComponent<SpecialUnit>();
        currentSpecialUnit.refSpecialUnitManager = this;
        UnitList.Add(currentUnitObject);

        if (UnitList.Count >= 6)
        {
            stopSpawning = true;
        }     

        if (stopSpawning)
        {
            print("StopSpawning!!!!!!!");
            foreach(GameObject unit in UnitList)
            {
                if (unit != null) { 
                print(unit.name);
                currentSpecialUnit = unit.GetComponent<SpecialUnit>();
<<<<<<< HEAD
                currentSpecialUnit.health--; 
                stopSpawning = false;
                }
            }                     
=======
                currentSpecialUnit.health--;
            }            
>>>>>>> parent of 4470b2f (can make a spawning loop (still getting weird error))
        }
    }
}
