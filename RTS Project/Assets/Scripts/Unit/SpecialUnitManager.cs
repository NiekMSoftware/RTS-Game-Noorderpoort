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
    public Terrain terrain;
    public int MinPostionX;
    public int MaxPostionX;
    public int MinPostionZ;
    public int MaxPostionZ;

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
        Vector3 RPosition = new Vector3(Random.Range(MinPostionX, MaxPostionX), 0, Random.Range(MinPostionZ, MaxPostionZ));
        Vector3 randomSpawnPosition = new Vector3(RPosition.x, terrain.SampleHeight(RPosition), RPosition.z);
        currentUnitObject = Instantiate(specialUnitObject, randomSpawnPosition, Quaternion.identity);
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
            foreach (GameObject unit in UnitList)
            {
                if (unit != null)
                {
                    print(unit.name);
                    currentSpecialUnit = unit.GetComponent<SpecialUnit>();
                    currentSpecialUnit.GoDownHealth--; 
                    stopSpawning = false;
                }
            }                                
        }
    }
}