using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBuildingBase : BuildingBase
{
    [SerializeField] protected int maxWorkers = 5;
    [SerializeField] private ItemSlot[] currentStorage;
    [SerializeField] private List<Worker> workers = new();
    [SerializeField] private string jobName;
    [SerializeField] private Jobs jobs;
    [SerializeField] private int scanRange = 200;

    private ResourceItemManager resourceItemManager;

    private List<GameObject> resourceAreas = new();

    public void SetResourceItemManagerByType(ResourceItemManager.Type type)
    {
        foreach (var item in FindObjectsOfType<ResourceItemManager>())
        {
            if (item.type == type)
            {
                resourceItemManager = item;
            }
        }
    }

    public override IEnumerator Build(float buildTime)
    {
        StartCoroutine(base.Build(buildTime));

        FindClosestResourceManager(transform, currentStorage[0].data);

        yield return null;
    }

    public GameObject FindClosestResourceManager(Transform buildingBase, ItemData itemdata)
    {
        foreach (Transform resourceType in FindAnyObjectByType<ResourceAreaSpawner>().GetComponentInChildren<Transform>())
        {
            //wanneer broken probeer dit  foreach (Transform resource in resourceType.GetComponentsInChildren<Transform>())
            foreach (Transform resource in resourceType.GetComponentInChildren<Transform>())
            {
                if (!resourceAreas.Contains(resource.gameObject))
                {
                    resourceAreas.Add(resource.gameObject);
                }
            }
        }

        GameObject closestResource = null;
        float closestDistance = scanRange;
        Vector3 currentPosition = buildingBase.transform.position;

        if (resourceAreas != null)
        {
            foreach (GameObject resource in resourceAreas)
            {
                if (resource != null)
                {
                    Vector3 resourcePosition = resource.transform.position;
                    float distanceToResource = Vector3.Distance(currentPosition, resourcePosition);

                    if (distanceToResource <= scanRange && distanceToResource < closestDistance)
                    {
                        if (Vector3.Distance(currentPosition, resourcePosition) < scanRange)
                        {
                            closestDistance = distanceToResource;
                            closestResource = resource;
                        }
                    }
                }
            }
        }
        else
        {
            print("No resource in range");
        }
        return closestResource;
    }

    public override void Init(Material _material, GameObject _particleObject)
    {
        base.Init(_material, _particleObject);
        SetResourceItemManagerByType(ResourceItemManager.Type.Player);
    }

    public override void SelectBuilding()
    {
        base.SelectBuilding();

        foreach (var worker in workers)
        {
            worker.Select();
        }
    }

    public override void DeselectBuilding()
    {
        base.DeselectBuilding();

        foreach (var worker in workers)
        {
            worker.Deselect();
        }
    }

    public ItemSlot GetStorage(ItemData itemdata)
    {
        foreach (ItemSlot slot in currentStorage)
        {
            if (slot.GetData() == itemdata) return slot;
        }

        return null;
    }

    public ItemData GetItemData()
    {
        return currentStorage[0].GetData();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, scanRange);
    }

    public void AddItemToStorage(ItemData itemData)
    {
        foreach (ItemSlot slot in currentStorage)
        {
            if (slot.GetData() == itemData)
            {
                if (slot.GetAmount() < slot.GetMaxAmount())
                {
                    slot.IncreaseAmount(1);
                }
                else
                {
                    Debug.LogError("Storage full");
                }
            }
        }
    }

    public void RemoveItemFromStorage(ItemData itemData)
    {
        foreach (ItemSlot slot in currentStorage)
        {
            if (slot.GetData() == itemData)
            {
                if (slot.GetAmount() > 0)
                {
                    slot.IncreaseAmount(-1);
                }
                else
                {
                    Debug.LogError("Storage empty");
                }
            }
        }
    }

    public bool AddWorkerToBuilding(Worker worker)
    {
        print("addworkertobuilding");
        if (workers.Contains(worker))
        {
            return false;
        }
        else if (worker.GetCurrentBuilding() != null)
        {
            return false;
        }
        else if (workers.Count < maxWorkers)
        {
            worker.InitializeWorker(gameObject, jobs, FindClosestResourceManager(transform, currentStorage[0].data),
                resourceItemManager);
            workers.Add(worker);
            return true;
        }

        return false;
    }

    public void RemoveWorkerFromBuilding(Worker worker)
    {
        workers.Remove(worker);
    }

    public List<Worker> GetWorkers()
    {
        return workers;
    }

    public int GetMaxWorkers()
    {
        return maxWorkers;
    }

    public override void DestroyBuilding()
    {
        foreach (var worker in workers)
        {
            RemoveWorkerFromBuilding(worker);
        }

        base.DestroyBuilding();
    }
}
