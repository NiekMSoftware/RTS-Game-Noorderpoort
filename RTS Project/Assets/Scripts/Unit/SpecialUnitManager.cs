using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpecialUnitManager : MonoBehaviour
{
    [Header("Timer")]
    public GameObject specialUnitObject;
    public float spawnTime;
    public float spawnDelay;
    [SerializeField] private List<GameObject> UnitList = new();
    public bool GoDown = false;
    public bool stopSpawning = false;

    public SpecialUnit specialUnit;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
    }

    public void SpawnObject()
    {
        while(UnitList.Count >= 0)
        {
            Instantiate(specialUnitObject, transform.position, transform.rotation);
            UnitList.Add(specialUnitObject);

            if (UnitList.Count == 5)
            {
                stopSpawning = true;
            }
        }
        
        if (stopSpawning)
        {
            CancelInvoke("SpawnObject");
            specialUnit.health--;
            UnitList.Clear();

            stopSpawning = false;
        }
    }
}
