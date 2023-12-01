using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class SpecialUnit : Unit
{
    [Header("Health")]
    public int health = 1;
    public bool SpecialUnitHealth = false;
    public int GoDownHealth = 1;
    public SpecialUnitManager refSpecialUnitManager;

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (GoDownHealth <= 0)
        {
            Debug.Log("I'm Down");
            refSpecialUnitManager.GoDown = true;
            SpecialUnitHealth = true;
            Debug.Log("I'm a Unit now");
            refSpecialUnitManager.GoDown = false;
            //placeholder voor despawnen.
            unitHealth--;
            if (unitHealth <= 0)
            {
                UnitDeath();
            }
            unitHealth = health;
        }
    }

    public void UnitDeath()
    {
        print("Death");
        refSpecialUnitManager.RemoveThis(gameObject);       
    }    
    //Special creatures have a lot of health. They randomly spawn (in the fog of war) every X minutes.
    //They can be captured by “killing” it.
    //You can then use it as a unit. it can die just like any other unit.
    //The creature has abilities that are triggered via the UI.

}
