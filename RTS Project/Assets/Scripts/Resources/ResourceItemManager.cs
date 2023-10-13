using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceItemManager : MonoBehaviour
{
    public ItemSlot[] itemSlots;

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
