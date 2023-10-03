using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceItemManager : MonoBehaviour
{
    public ItemSlot[] itemSlots;

    public ItemSlot[] GetAllResources() => itemSlots;
}
