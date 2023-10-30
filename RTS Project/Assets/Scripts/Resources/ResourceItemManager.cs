using UnityEngine;

public class ResourceItemManager : MonoBehaviour
{
    public Type type;

    public ItemSlot[] itemSlots;

    public enum Type
    {
        Player,
        AI
    }

    public ItemSlot GetSlotByItemData(ItemData data)
    {
        foreach (var item in itemSlots)
        {
            if (item.data == data)
            {
                return item;
            }
        }

        return null;
    }
}
