using Unity.Netcode;
using UnityEngine;

public class ResourceItemManager : NetworkBehaviour
{
    public Type type;

    public ItemSlot[] itemSlots;

    public NetworkVariable<ItemSlot> itemSlotVar = new NetworkVariable<ItemSlot>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //public NetworkVariable<ItemData> dataVar = new NetworkVariable<ItemData>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public enum Type
    {
        Player,
        AI
    }

    [ServerRpc (RequireOwnership = false)]
    public void GetSlotByItemDataServerRpc(ItemData _data)
    {
        print(_data);
        //itemSlotVar.Value.data = _data;
        print(itemSlotVar.Value.data);
        //dataVar.Value = _data;
        foreach (var item in itemSlots)
        {
            if (item.data == _data)
            { 
                itemSlotVar.Value = item;
                print(itemSlotVar);
                return;
            }
        }
    }
}
