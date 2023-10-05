using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialUnit : Unit
{
    [SerializeField]
    public bool GoDown = false;
    [SerializeField]
    public GameObject SpawnSpecialUnit;
    [SerializeField]
    public bool stopSpawning = false;
    [SerializeField]
    public float spawnTime;
    
    public float spawnDelay;

    
   public SpecialUnit()
    {
        
    }
    public void InitsStats()
    {
        unitHealth = 10;
    }

    private void Start()
    {
        InitsStats();
        InvokeRepeating("SpawnObject", spawnTime, spawnDelay);
    }

    public void SpawnObject()
    {
        Instantiate(SpawnSpecialUnit, transform.position, transform.rotation);
        if(stopSpawning)
        {
            CancelInvoke("SpawnObject");
        }
    }

    protected override void Death()
    {
        if (unitHealth <= 0) 
        {
            GoDown = true;
            
            //when food is 5 then reset health, GoDown false and make unit.
            //when hp is 0 again then die.
        }


    }
    //Special creatures have a lot of health. They randomly spawn (in the fog of war) every X minutes.
    //They can be captured by “killing” it and feeding it until it’s tamed.
    //You can then use it as a unit. it can die just like any other unit.
    //The creature has abilities that are triggered via the UI.

}
