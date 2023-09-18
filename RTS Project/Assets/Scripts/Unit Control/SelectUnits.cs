using System.Collections.Generic;
using UnityEngine;

public class SelectUnits : MonoBehaviour
{
    //lijst die laat zien hoeveel AI er in de scene zit.
    public List<GameObject> unitList = new List<GameObject>();
    //lijst die laat zien hoeveel AI er Selected is.
    public List<GameObject> unitsSelected = new List<GameObject>();

    private static SelectUnits _instance;
    public static SelectUnits Instance { get { return _instance; } }

     void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void ClickSelect(GameObject unitToAdd)
    {
        DeSelectAll();
        unitsSelected.Add(unitToAdd);
    }

    public void ShiftClickSelect(GameObject unitToAdd)
    {
        if(!unitsSelected.Contains(unitToAdd))
        {
            unitsSelected.Add(unitToAdd);
        }
        else
        {
            unitsSelected.Remove(unitToAdd);
        }
    }

    public void DragClickSelect(GameObject unitToAdd)
    {

    }

    public void DeSelect(GameObject unitToAdd)
    {

    }

    public void DeSelectAll()
    {
        unitsSelected.Clear();
    }

    public void Deselect(GameObject unitToDeselect)
    {

    }
}
