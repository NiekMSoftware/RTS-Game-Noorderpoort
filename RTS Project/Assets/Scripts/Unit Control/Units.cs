using UnityEngine;

public class Units : MonoBehaviour
{
    //Zet AI in een lijst wanner opgestart wordt.
    private void Start()
    {       
        SelectUnits.Instance.unitList.Add(this.gameObject);      
    }

    //Haalt AI uit de lijst.
    void OnDestroy()
    {
        SelectUnits.Instance.unitList.Remove(this.gameObject);
    }
}
