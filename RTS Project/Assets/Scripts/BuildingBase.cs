using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBase : MonoBehaviour
{
    [SerializeField] protected float buildingHp = 50f;
    [SerializeField] protected int maxWorkers = 5;
    [SerializeField] protected int currentWorkers = 5;
    [SerializeField] protected int maxStorage = 20;
    [SerializeField] protected Item[] currentStorage;
    [SerializeField] protected Item itemType;

    protected void ManageStorage()
    {
        //if (currentStorage != maxStorage)
        //{
        //    //human go work
        //}

        //Human human;
        //human = GetComponent<Human>();

        //human.healthUnit;
    }
    //protected void AddItemToStorage()
    //{
    //    currentStorage++;
    //}
    //protected void RemoveItemFromStorage()
    //{
    //    currentStorage--;
    //}
    protected void AddHumanToBuilding()
    {
        currentWorkers++;
    }
    protected void RemoveHumanFromBuilding()
    {
        currentWorkers--;
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
