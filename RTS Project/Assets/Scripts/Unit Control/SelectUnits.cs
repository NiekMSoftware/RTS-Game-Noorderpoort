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
        unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
        unitToAdd.GetComponent<AIMovement>().enabled = true;
    }

    public void ShiftClickSelect(GameObject unitToAdd)
    {
        if(!unitsSelected.Contains(unitToAdd))
        {
            unitsSelected.Add(unitToAdd);
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
            unitToAdd.GetComponent<AIMovement>().enabled = true;
        }
        else
        {
            unitToAdd.GetComponent<AIMovement>().enabled = false;
            unitToAdd.transform.GetChild(0).gameObject.SetActive(false);
            unitsSelected.Remove(unitToAdd);
        }
    }

    public void DragSelect(GameObject unitToAdd)
    {
        if (!unitsSelected.Contains(unitToAdd))
        {
            unitsSelected.Add(unitToAdd);
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
            unitToAdd.GetComponent<AIMovement>().enabled = true;
        }
    }

    public void DeSelect(GameObject unitToAdd)
    {

    }

    public void DeSelectAll()
    {
        foreach (var unit in unitsSelected)
        {
            unit.GetComponent<AIMovement>().enabled = false;
            unit.transform.GetChild(0).gameObject.SetActive(false);
        }
        unitsSelected.Clear();
    }

    public void Deselect(GameObject unitToDeselect)
    {

    }
}
