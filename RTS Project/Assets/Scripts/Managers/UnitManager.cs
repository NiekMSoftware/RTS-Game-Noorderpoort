using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    //gamemanager maken:
    //- 2 list met alle units per team.
    //- unit moet zelf doorgeven wanneer hij in de lijst moet.
    //- uit de lijst de enemies halen en dan hieronder gebruiken.
    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();

    public void AddUnit(Unit unit)
    {
        if(unit.typeUnit == Unit.TypeUnit.Enemy)
        {
            enemyUnits.Add(unit);
        }
        else if(unit.typeUnit == Unit.TypeUnit.Human)
        {
            playerUnits.Add(unit);
        }
        else
        {
            Debug.Log("no TypeUnit");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
