using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public List<GameObject> assignedResources = new List<GameObject>();
    public ItemSlot[] itemSlots;

    public ItemSlot[] GetAllResources() => itemSlots;


}