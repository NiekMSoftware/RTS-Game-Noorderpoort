using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    [SerializeField] private ItemSlot slot;
    private ResourceSpawnManager resourceSpawnManager;
    private void Awake()
    {
        resourceSpawnManager = FindObjectOfType<ResourceSpawnManager>();
    }
    private void Update()
    {
        if (slot.amount <= 0)
        {

        }
    }

    public void RemoveItemFromResource()
    {
        if (slot.amount > 0)
        {
            slot.IncreaseAmount(-1);
            
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
