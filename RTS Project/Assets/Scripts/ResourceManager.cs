using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public ItemSlot[] itemSlots;

    public ItemSlot[] GetAllResources() => itemSlots;
}