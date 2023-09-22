using UnityEngine;

public class BuildingBase : MonoBehaviour
{
    [SerializeField] protected float buildingHp = 50f;
    [SerializeField] protected int maxWorkers = 5;
    [SerializeField] protected int currentWorkers = 5;
    [SerializeField] protected int maxStorage = 20;
    [SerializeField] protected ItemSlot currentStorage;

    protected void ManageStorage()
    {
        if (currentStorage.GetAmount() != maxStorage)
        {
            //human go work
        }

        //Human human;
        //human = GetComponent<Human>();

        //human.healthUnit;
    }

    protected void AddItemToStorage(ItemData itemData)
    {
        if (itemData == currentStorage.GetData())
        {
            if (currentStorage.GetAmount() < maxStorage)
            {
                currentStorage.IncreaseAmount(1);
            }
            else
            {
                Debug.LogError("Storage full");
            }
        }
    }
    protected void RemoveItemFromStorage(ItemData itemData)
    {
        if (itemData == currentStorage.GetData())
        {
            if (currentStorage.GetAmount() > 0)
            {
                currentStorage.IncreaseAmount(-1);
            }
            else
            {
                Debug.LogError("Storage Empty");
            }
        }
    }

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