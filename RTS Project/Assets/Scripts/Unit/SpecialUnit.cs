using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialUnit : Unit
{
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
    }

    //Special creatures have a lot of health. They randomly spawn (in the fog of war) every X minutes.
    //They can be captured by “killing” it and feeding it until it’s tamed.
    //You can then use it as a unit. it can die just like any other unit.
    //The creature has abilities that are triggered via the UI.

}
