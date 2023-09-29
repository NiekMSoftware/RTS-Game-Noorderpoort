using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Worker : Unit
{
    private GameObject[] resourceTargets;
    [SerializeField] private GameObject resourceTarget;
    [SerializeField] private GameObject workerHouse;
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] protected ItemSlot currentStorage;
    [SerializeField] protected int maxStorage = 3;
    [SerializeField] protected float workerHp = 5f;
    [SerializeField] private ItemData resourceItem;

    private bool canGather = true;
    private bool canDeposit = true;
    private float transferRange = 2.5f;
    private float gatherTime = 1f;
    private enum State { Moving, Idling, Gathering, Depositing, Assigning }
    private State currentState = State.Assigning;
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
    public void InitializeWorker(GameObject _workerHouse, BuildingBase.Jobs _jobName, GameObject[] _resourceTargets)
    {

        workerHouse = _workerHouse;
        resourceTargets = _resourceTargets;
        jobName = _jobName.ToString();
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

    //public GameObject FindClosestResource()
    //{
    //    GameObject closestResource = null;
    //    float closestDistance = scanRange;
    //    Vector3 currentPosition = transform.position;

    //    resourceTargets = buildingBase.GetResources();

    //    if (resourceTargets != null)
    //    {
    //        foreach (GameObject resource in resourceTargets)
    //        {
    //            Vector3 resourcePosition = resource.transform.position;
    //            float distanceToResource = Vector3.Distance(currentPosition, resourcePosition);

    //            if (distanceToResource <= scanRange && distanceToResource < closestDistance)
    //            {
    //                closestDistance = distanceToResource;
    //                closestResource = resource;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        print("No resource in range");
    //    }


    //    return closestResource;
    //}
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
                    buildingBase = workerHouse.GetComponent<BuildingBase>();
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
                        print("123");
                        resourceTarget = resourceManager.FindClosestResource(buildingBase.transform, resourceItem);
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

                if (Vector3.Distance(transform.position, workerHouse.transform.position) <= transferRange)
                {
                    myAgent.isStopped = true;
                }
                else
                {
                    myAgent.SetDestination(workerHouse.transform.position);
                }

                if (currentStorage.GetAmount() < maxStorage &&
                    buildingBase.GetStorage(resourceItem).GetAmount() < buildingBase.GetStorage(resourceItem).GetMaxAmount()
                    && resourceTarget)
                {
                    currentState = State.Moving;
                }
                
                if (!resourceTarget && resourceManager.resources.Count > resourceManager.occupiedResources.Count)
                {
                    resourceTarget = resourceManager.FindClosestResource(buildingBase.transform, resourceItem);
                    currentState = State.Moving;
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
