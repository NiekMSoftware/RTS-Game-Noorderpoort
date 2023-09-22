using UnityEngine;
using UnityEngine.AI;

public class BuildingBase : MonoBehaviour
{
    [SerializeField] protected float buildingHp = 50f;
    [SerializeField] protected int maxWorkers = 5;
    protected int currentWorkers = 0;
    [SerializeField] public int maxStorage = 20;
    [SerializeField] public ItemSlot currentStorage;
    [SerializeField] protected ItemData resourceItem;

    private void Awake()
    {
        ItemSlot slot = new();
        slot.SetData(resourceItem);
        slot.SetAmount(0);
        currentStorage = slot;
    }
    protected void ManageStorage()
    {
        if (currentStorage.GetAmount() < maxStorage)
        {
            //human go work
        }

        //Human human;
        //human = GetComponent<Human>();

        //human.healthUnit;
    }

    public void AddItemToStorage(ItemData itemData)
    {
        print(currentStorage.GetAmount());
        if (itemData == resourceItem)
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

    public void RemoveItemFromStorage(ItemData itemData)
    {
        if (itemData == resourceItem)
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
}