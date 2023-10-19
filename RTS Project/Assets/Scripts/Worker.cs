using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Worker : Unit
{
    [SerializeField] private GameObject resourceTarget;
    [SerializeField] private GameObject workerHouse;
    [SerializeField] private ResourceObjectManager resourceObjectManager;
    [SerializeField] protected ItemSlot currentStorage;
    [SerializeField] protected int maxStorage = 3;

    private ResourceItemManager resourceItemManager;
    private ItemData resourceItem;

    private bool canGather = true;
    private bool canDeposit = true;
    private float transferRange = 2.5f;
    private float gatherTime = 1f;
    public enum State { Moving, Idling, Gathering, Depositing, Assigning }
    public State currentState = State.Assigning;
    private BuildingBase buildingBase;
    private string jobName;

    public bool assigned = false;

    private void Start()
    {
        ItemSlot itemSlot = new();
        itemSlot.SetData(resourceItem);
        itemSlot.SetAmount(0);
        currentStorage = itemSlot;
        myAgent = GetComponent<NavMeshAgent>();
    }
    public BuildingBase GetCurrentBuilding()
    {
        if (workerHouse)
        {
            return workerHouse.GetComponent<BuildingBase>();
        }
        else
        {
            return null;
        }

    }
    public void InitializeWorker(GameObject _workerHouse, BuildingBase.Jobs _jobName,
        GameObject _resourceObjectManager, ResourceItemManager _resourceItemManager)
    {
        print("added new worker!");
        resourceObjectManager = _resourceObjectManager.GetComponent<ResourceObjectManager>();
        workerHouse = _workerHouse;
        jobName = _jobName.ToString();
        buildingBase = workerHouse.GetComponent<BuildingBase>();
        resourceItem = buildingBase.GetItemData();
        resourceItemManager = _resourceItemManager;
    }

    protected void AddItemToWorkerStorage(ItemData itemData)
    {
        if (itemData == resourceItem)
        {
            if (currentStorage.GetAmount() < maxStorage)
            {
                currentStorage.IncreaseAmount(1);
            }
            else
            {
                Debug.LogError("Worker storage full");
            }
        }
    }

    protected void RemoveItemFromWorkerStorage(ItemData itemData)
    {
        if (itemData == resourceItem)
        {
            if (currentStorage.GetAmount() > 0)
            {
                currentStorage.IncreaseAmount(-1);
            }
            else
            {
                Debug.LogError("Worker storage Empty");
            }
        }
    }
    private IEnumerator GatherResource()
    {
        while (currentStorage.GetAmount() < maxStorage && resourceTarget)
        {
            resourceTarget.GetComponent<ResourceObject>().RemoveItemFromResource();
            AddItemToWorkerStorage(resourceItem);
            yield return new WaitForSeconds(gatherTime);
        }
        currentState = State.Moving;
        canGather = true;

        yield return null;
    }

    private IEnumerator DepositResources()
    {
        while (currentStorage.GetAmount() > 0)
        {
            if (buildingBase.GetStorage(resourceItem).GetAmount() < buildingBase.GetStorage(resourceItem).GetMaxAmount())
            {
                RemoveItemFromWorkerStorage(resourceItem);
                buildingBase.AddItemToStorage(resourceItem);
                resourceItemManager.GetSlotByItemData(resourceItem).amount++;
            }
            yield return new WaitForSeconds(1f);
        }
        currentState = State.Moving;
        canDeposit = true;

        yield return null;
    }
    private void Update()
    {
        switch (currentState)
        {
            case State.Assigning:
                if (workerHouse)
                {
                    myAgent.isStopped = false;
                    myAgent.SetDestination(workerHouse.transform.position);

                    if (Vector3.Distance(transform.position, workerHouse.transform.position) <= transferRange)
                    {
                        currentState = State.Moving;
                    }
                }
                break;
            case State.Moving:
                myAgent.isStopped = false;
                if (currentStorage.GetAmount() < maxStorage)
                {
                    if (!resourceTarget)
                    {
                        resourceTarget = resourceObjectManager.FindClosestResource(buildingBase.transform, resourceItem, this);
                    }
                    else
                    {
                        myAgent.SetDestination(resourceTarget.transform.position);
                    }
                }
                else if (currentStorage.GetAmount() == maxStorage)
                {
                    myAgent.SetDestination(workerHouse.transform.position);
                }

                if (resourceTarget)
                {
                    if (Vector3.Distance(transform.position, resourceTarget.transform.position) <= transferRange && currentStorage.GetAmount() < maxStorage)
                    {
                        currentState = State.Gathering;
                    }
                }
                else
                {
                    currentState = State.Idling;
                }

                if (Vector3.Distance(transform.position, workerHouse.transform.position) <= transferRange)
                {
                    if (currentStorage.GetAmount() > 0 && buildingBase.GetStorage(resourceItem).GetAmount() < buildingBase.GetStorage(resourceItem).GetMaxAmount())
                    {
                        currentState = State.Depositing;
                    }
                }

                if (currentStorage.GetAmount() >= maxStorage && buildingBase.GetStorage(resourceItem).GetAmount() >= buildingBase.GetStorage(resourceItem).GetMaxAmount())
                {
                    currentState = State.Idling;
                }
                break;

            case State.Idling:

                // If there are resources, go find resource to get
                if (!resourceTarget && resourceObjectManager.resources.Count > resourceObjectManager.occupiedResources.Count)
                {
                    currentState = State.Moving;
                    resourceTarget = resourceObjectManager.FindClosestResource(buildingBase.transform, resourceItem, this);
                }

                // If inventory isnt full in building and working go to moving
                if (currentStorage.GetAmount() < maxStorage &&
                    buildingBase.GetStorage(resourceItem).GetAmount() < buildingBase.GetStorage(resourceItem).GetMaxAmount()
                    && resourceTarget)
                {
                    currentState = State.Moving;
                }

                // Go back to the workerhouse
                if (Vector3.Distance(transform.position, workerHouse.transform.position) <= transferRange)
                {
                    myAgent.isStopped = true;
                }
                else
                {
                    myAgent.SetDestination(workerHouse.transform.position);
                }
                break;

            case State.Gathering:
                myAgent.isStopped = true;
                if (canGather)
                {
                    canGather = false;
                    StartCoroutine(GatherResource());
                }
                break;

            case State.Depositing:
                myAgent.isStopped = true;
                // kijk uit voor de edge case als een worker vol is en een gebouw vol is
                if (buildingBase.GetStorage(resourceItem).GetAmount() == buildingBase.GetStorage(resourceItem).GetMaxAmount())
                {
                    currentState = State.Moving;
                }

                if (Vector3.Distance(transform.position, workerHouse.transform.position) <= transferRange)
                {
                    if (canDeposit)
                    {
                        canDeposit = false;
                        StartCoroutine(DepositResources());
                    }

                    if (currentStorage.GetAmount() == maxStorage)
                    {
                        currentState = State.Moving;

                    }
                }
                else
                {
                    currentState = State.Moving;
                }
                break;
        }
    }
}
