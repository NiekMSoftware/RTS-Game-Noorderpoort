using System.Collections.Generic;
using UnityEngine;

public class ResourceObjectManager : MonoBehaviour
{
    public List<GameObject> resources = new List<GameObject>();
    public List<GameObject> occupiedResources = new List<GameObject>();
    public bool placedBuilding;

    public void SetResources(GameObject resource)
    {
        resources.Add(resource);
    }

    //private void Awake()
    //{
    //    foreach (Transform resource in transform.GetComponentInChildren<Transform>())
    //    {
    //        if (!resources.Contains(resource.gameObject))
    //        {
    //            resources.Add(resource.gameObject);
    //        }
    //    }
    //}

    public GameObject FindClosestResource(ResourceBuildingBase buildingBase, ItemData itemdata, Worker worker)
    {
        GameObject closestResource = null;
        float scanRange = buildingBase.GetRange();
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
                        if (Vector3.Distance(currentPosition, resourcePosition) < scanRange)
                        {
                            closestDistance = distanceToResource;
                            closestResource = resource;
                        }
                    }
                }
                else
                {
                    //print("Go die");
                }
            }
            if (closestResource != null)
            {
                occupiedResources.Add(closestResource);
            }
            else
            {
                worker.GetComponent<Worker>().currentState = Worker.State.Idling;
            }
        }
        else
        {
            print("No resource in range");
        }
        return closestResource;
    }
}