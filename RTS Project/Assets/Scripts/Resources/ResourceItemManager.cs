using Unity.Netcode;
using UnityEngine;

public class ResourceItemManager : NetworkBehaviour
{
    public Type type;

    public ItemSlot[] itemSlots;

    public NetworkVariable<ItemSlot> itemSlotVar = new NetworkVariable<ItemSlot>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<ItemData> data = new NetworkVariable<ItemData>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public enum Type
    {
        Player,
        AI
    }

    [ServerRpc (RequireOwnership = false)]
    public void GetSlotByItemDataServerRpc()
    {
        foreach (var item in itemSlots)
        {
            if (item.data == data.Value)
            {
                itemSlotVar.Value = item;
                return;
            }
        }
        itemSlotVar.Value = null;
    }
}
