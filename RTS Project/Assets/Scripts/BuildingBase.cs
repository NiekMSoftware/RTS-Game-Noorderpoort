using UnityEngine;
using UnityEngine.AI;

public class BuildingBase : MonoBehaviour
{
    [SerializeField] protected float buildingHp = 50f;
    [SerializeField] protected int maxWorkers = 5;
    protected int currentWorkers = 0;
    [SerializeField] public ItemSlot[] currentStorage;
    public ItemSlot GetStorage(ItemData itemdata)
    {
        foreach (ItemSlot slot in currentStorage)
        {
            if (slot.GetData() == itemdata) return slot;
        }

        return null;
    }

    public void AddItemToStorage(ItemData itemData)
    {
        foreach (ItemSlot slot in currentStorage)
        {
            if (slot.GetData() == itemData)
            {
                if (slot.GetAmount() < slot.GetMaxAmount())
                {
                    slot.IncreaseAmount(1);
                }
                else
                {
                    Debug.LogError("Storage full");
                }
            }
        }
    }

    public void RemoveItemFromStorage(ItemData itemData)
    {
        foreach (ItemSlot slot in currentStorage)
        {
            if (slot.GetData() == itemData)
            {
                if (slot.GetAmount() > 0)
                {
                    slot.IncreaseAmount(-1);
                }
                else
                {
                    Debug.LogError("Storage empty");
                }
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