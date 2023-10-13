using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBase : MonoBehaviour
{
    [SerializeField] protected float buildingHp = 50f;
    [SerializeField] protected int maxWorkers = 5;
    [SerializeField] private ItemSlot[] currentStorage;
    [SerializeField] private List<Worker> workers = new();
    [SerializeField] private string jobName;
    [SerializeField] private Jobs jobs;
    [SerializeField] private States currentState;
    private List<Material> savedMaterials = new();
    private GameObject particleObject;
    private List<GameObject> resourceAreas = new();
    private int scanRange = 200;

    private Material buildingMaterial;

    public enum Jobs { Wood, Stone, Metal }

    public enum States
    {
        Building,
        Normal
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
    public void Init(Material _material, GameObject _particleObject)
    {
        buildingMaterial = _material;
        particleObject = _particleObject;
    }

    public IEnumerator Build(float buildTime)
    {
        currentState = States.Building;

        SaveObjectMaterials();
        ApplyObjectMaterials();
        //ChangeObjectMaterial(buildingMaterial);
        yield return new WaitForSeconds(buildTime);

        currentState = States.Normal;
        ParticleSystem particle = Instantiate(particleObject, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        particle.Play();
        FindClosestResourceManager(transform, currentStorage[0].data);
        yield return new WaitForSeconds(particle.main.duration);

        yield return null;
    }

    public ItemSlot GetStorage(ItemData itemdata)
    {
        foreach (ItemSlot slot in currentStorage)
        {
            if (slot.GetData() == itemdata) return slot;
        }

        return null;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.gameObject.transform.position, 20);
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

    public void AddWorkerToBuilding(Worker worker)
    {
        if (workers.Contains(worker))
        {
            return;
        }
        else if (worker.GetCurrentBuilding() != null)
        {
            return;
        }
        else if (workers.Count < maxWorkers)
        {
            worker.InitializeWorker(gameObject, jobs, FindClosestResourceManager(this.transform, currentStorage[0].data));
            workers.Add(worker);
        }
    }

    protected void RemoveWorkerFromBuilding(Worker worker)
    {
        workers.Remove(worker);
    }

    public States GetCurrentState() => currentState;

    private void ChangeObjectMaterial(Material material)
    {
        if (gameObject.TryGetComponent(out MeshRenderer mr))
        {
            if (mr.material)
            {
                mr.material = material;
            }
        }
        else
        {
            foreach (var mr2 in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                Material[] materials = mr2.materials;

                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = material;
                }

                mr2.materials = materials;
            }
        }
    }

    private void ApplyObjectMaterials()
    {
        if (gameObject.TryGetComponent(out MeshRenderer mr))
        {
            if (mr.material)
            {
                mr.material = savedMaterials[0];
            }
        }
        else
        {
            foreach (var mr2 in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                Material[] materials = mr2.materials;

                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = savedMaterials[i];
                }

                mr2.materials = materials;
            }
        }
    }

    private void SaveObjectMaterials()
    {
        if (gameObject.TryGetComponent(out MeshRenderer mr))
        {
            if (mr.material)
            {
                savedMaterials[0] = mr.material;
            }
        }
        else
        {
            foreach (var mr2 in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                Material[] materials = mr2.materials;

                for (int i = 0; i < materials.Length; i++)
                {
                    //print(materials[i].name + " " + i + mr2.transform.GetSiblingIndex());
                    savedMaterials.Add(materials[i]);
                }
            }
        }
    }
}