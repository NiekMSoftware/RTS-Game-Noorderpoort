using Unity.Netcode;
using UnityEngine;

public class ResourceItemManager : NetworkBehaviour
{
    public Type type;

    NetworkVariable<ItemSlot> itemSlotVar =  new NetworkVariable<ItemSlot>(null ,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public ItemSlot[] itemSlots;

    public enum Type
    {
        Player,
        AI
    }

    public void GetSlotByItemData(ItemData data)
    {
        foreach (var item in itemSlots)
        {
            if (item.data == data)
            {
                itemSlotVar.Value = item;
                return;
            }
        }
        itemSlotVar.Value = null;
        return;
    }
}
