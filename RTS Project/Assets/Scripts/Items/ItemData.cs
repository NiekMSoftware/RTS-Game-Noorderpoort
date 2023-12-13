using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "Item")]
[System.Serializable]
public class ItemData : ScriptableObject, INetworkSerializable
{
    [SerializeField] private string itemName;

    // Implement INetworkSerializable
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemName);
    }
    public string ItemName => itemName;
}