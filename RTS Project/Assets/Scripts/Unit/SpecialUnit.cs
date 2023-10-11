using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class SpecialUnit : Unit
{
    [Header("FUCKING HELL KUT DING WERK")]
    public int health = 1;
    public SpecialUnitManager refSpecialUnitManager;

    private void Start()
    {
        
    }

    private void Update()
    {
        Death();
        unitHealth = health;
    }

    protected override void Death()
    {
        print("Death");
        if(unitHealth <= 0)
        {
            print("Destroyed GO");
            refSpecialUnitManager.UnitList.Remove(gameObject);
            Destroy(gameObject);
        }
    }
    //Special creatures have a lot of health. They randomly spawn (in the fog of war) every X minutes.
    //They can be captured by “killing” it and feeding it until it’s tamed.
    //You can then use it as a unit. it can die just like any other unit.
    //The creature has abilities that are triggered via the UI.

}
