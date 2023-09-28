using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BuildingBase : MonoBehaviour
{
    [SerializeField] protected float buildingHp = 50f;
    [SerializeField] protected int maxWorkers = 5;
    [SerializeField] private ItemSlot[] currentStorage;
    [SerializeField] private List<Worker> workers = new();
    [SerializeField] private string jobName;
    [SerializeField] private GameObject[] resourceTargets;
    [SerializeField] private Jobs jobs;
    [SerializeField] private States currentState;
    [SerializeField] private Material buildingMaterial;

    private List<Material> savedMaterials = new();

    public enum Jobs { Wood, Stone, Metal }

    public enum States
    {
        Building,
        Normal
    }

    public IEnumerator Build(float buildTime)
    {
        currentState = States.Building;

        SaveObjectMaterials();
        ChangeObjectMaterial(gameObject, buildingMaterial);
        yield return new WaitForSeconds(buildTime);

        ApplyObjectMaterials();
        currentState = States.Normal;

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
            worker.InitializeWorker(gameObject, jobs, resourceTargets);
            workers.Add(worker);
        }
    }

    public GameObject[] GetResources() => resourceTargets;

    protected void RemoveWorkerFromBuilding(Worker worker)
    {
        workers.Remove(worker);
    }

    public States GetCurrentState() => currentState;

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
                for (int i = 0; i < mr2.materials.Length; i++)
                {
                    savedMaterials.Add(mr2.materials[i]);
                }
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
                for (int i = 0; i < mr2.materials.Length; i++)
                {
                    mr2.materials[i] = savedMaterials[i];
                }
            }
        }
    }

    private void ChangeObjectMaterial(GameObject go, Material material)
    {
        if (go.TryGetComponent(out MeshRenderer mr))
        {
            if (mr.material)
            {
                mr.material = material;
            }
        }
        else
        {
            foreach (var mr2 in go.GetComponentsInChildren<MeshRenderer>())
            {
                List<Material> materials = new();

                for (int i = 0; i < mr2.materials.Length; i++)
                {
                    materials.Add(material);
                }

                mr2.SetMaterials(materials);
            }
        }
    }
}