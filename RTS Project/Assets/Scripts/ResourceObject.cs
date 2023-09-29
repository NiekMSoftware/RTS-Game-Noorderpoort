using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    [SerializeField] public ItemSlot slot;
    private ResourceManager resourceManager;
    private void Awake()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
    }
    public void RemoveItemFromResource()
    {
        if (slot.amount > 0)
        {
            slot.IncreaseAmount(-1);

        }
        else
        {
            print("destroy");
            resourceManager.resources.Remove(gameObject);
            resourceManager.occupiedResources.Remove(gameObject);
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        resourceManager.resources.Remove(gameObject);
        resourceManager.occupiedResources.Remove(gameObject);
    }
}
