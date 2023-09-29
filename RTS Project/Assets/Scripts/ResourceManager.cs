using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public ItemSlot[] itemSlots;

    public ItemSlot[] GetAllResources() => itemSlots;

    public List<GameObject> resources = new List<GameObject>();
    public List<GameObject> occupiedResources = new List<GameObject>();
    private float scanRange = 100f;

    public GameObject FindClosestResource(Transform buildingBase, ItemData itemdata)
    {
        foreach (Transform resource in transform.GetComponentInChildren<Transform>())
        {
            if (!resources.Contains(resource.gameObject))
            {
                resources.Add(resource.gameObject);
            }
        }

        GameObject closestResource = null;
        float closestDistance = scanRange;
        Vector3 currentPosition = buildingBase.transform.position;

        if (resources != null)
        {
            foreach (GameObject resource in resources)
            {
                if (resource != null && resource.GetComponent<ResourceObject>().slot.data == itemdata)
                {
                    Vector3 resourcePosition = resource.transform.position;
                    float distanceToResource = Vector3.Distance(currentPosition, resourcePosition);

                    if (distanceToResource <= scanRange && !occupiedResources.Contains(resource) && distanceToResource < closestDistance)
                    {
                        closestDistance = distanceToResource;
                        closestResource = resource;
                        occupiedResources.Add(resource);
                        break;
                    }
                }
                else
                {
                    print("Go die");
                }
            }
        }
        else
        {
            print("No resource in range");
        }
        return closestResource;
    }
}