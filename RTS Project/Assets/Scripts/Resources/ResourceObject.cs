using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    [SerializeField] public ItemSlot slot;
    private ResourceObjectManager resourceManager;
    private void Awake()
    {
        resourceManager = FindObjectOfType<ResourceObjectManager>();
        StartCoroutine(AmountChanger());
    }

    private IEnumerator AmountChanger()
    {
        yield return new WaitForSeconds(1f);
        slot.amount = (int)Math.Round(slot.amount * transform.localScale.x, 0);
        yield return null;
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
    private void OnDestroy()
    {
        resourceManager.resources.Remove(gameObject);
        resourceManager.occupiedResources.Remove(gameObject);
    }
}
