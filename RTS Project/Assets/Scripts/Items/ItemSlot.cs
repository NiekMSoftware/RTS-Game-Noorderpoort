using System;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
[GenerateSerializationForType(typeof(ItemSlot))]
public class ItemSlot : INetworkSerializable
{
    // INetworkSerializable
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref amount);
        serializer.SerializeValue(ref maxAmount);
        if (data != null)
        {
            serializer.SerializeValue(ref data);
        }
        else
        {
            // Handle the case when data is null (possibly set default values)
        }
    }
    public ItemData data;
    public int amount;
    public int maxAmount;
    
    public int GetAmount() => amount;
    public int GetMaxAmount() => maxAmount;

    public void IncreaseAmount(int value) => amount += value;

    public void SetAmount(int value) => amount = value;
    public void SetMaxAmount(int value) => maxAmount = value;


    public ItemData GetData() => data;

    public ItemData SetData(ItemData _data) => data = _data;
}