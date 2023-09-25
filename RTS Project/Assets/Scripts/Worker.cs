using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    private Transform resourceTarget;
    [SerializeField] private GameObject workerHouse;
    private string resourceTag = "StoneResource";
    private float scanRange = 100f;
    NavMeshAgent myAgent;

    [SerializeField] protected ItemSlot currentStorage;
    [SerializeField] protected int maxStorage = 3;
    [SerializeField] protected float workerHp = 5f;
    [SerializeField] private ItemData resourceItem;

    private bool canGather = true;
    private bool canDeposit = true;
    private float transferRange = 2.5f;
    private float gatherTime = 1f;
    private enum State{Moving, Idling, Gathering, Depositing, Assigning}
    private State currentState = State.Assigning;
    private BuildingBase buildingBase;
    private string jobName;

    private void Start()
    {
        ItemSlot itemSlot = new();
        itemSlot.SetData(resourceItem);
        itemSlot.SetAmount(0);
        currentStorage = itemSlot;
        myAgent = GetComponent<NavMeshAgent>();
    }

    public void InitializeWorker(GameObject _workerHouse, string _resourceTag, string _jobName)
    {
        workerHouse = _workerHouse;
        resourceTag = _resourceTag;
        jobName = _jobName;
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

    public Transform FindClosestResource()
    {
        GameObject[] resourceObjects = GameObject.FindGameObjectsWithTag(resourceTag);
        Transform closestResource = null;
        float closestDistance = scanRange;
        Vector3 currentPosition = transform.position;

        foreach (GameObject resource in resourceObjects)
        {
            Vector3 resourcePosition = resource.transform.position;
            float distanceToResource = Vector3.Distance(currentPosition, resourcePosition);

            if (distanceToResource <= scanRange && distanceToResource < closestDistance)
            {
                closestDistance = distanceToResource;
                closestResource = resource.transform;
            }
        }

        return closestResource;
    }
    private IEnumerator GatherResource()
    {
        while (currentStorage.GetAmount() < maxStorage)
        {
            AddItemToWorkerStorage(resourceItem);
            yield return new WaitForSeconds(gatherTime);
        }
        resourceTarget = null;
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
                else
                {
                    myAgent.isStopped = true;
                }
                

                
                break;
            case State.Moving:
                myAgent.isStopped = false;
                if (currentStorage.GetAmount() < maxStorage)
                {
                    if (!resourceTarget)
                    {
                        resourceTarget = FindClosestResource();
                    }
                    else
                    {
                        myAgent.SetDestination(resourceTarget.position);
                    }
                }
                else if (currentStorage.GetAmount() == maxStorage)
                {
                    myAgent.SetDestination(workerHouse.transform.position);
                }

                if (resourceTarget)
                {
                    if (Vector3.Distance(transform.position, resourceTarget.position) <= transferRange && currentStorage.GetAmount() < maxStorage)
                    {
                        currentState = State.Gathering;
                    }
                }

                if (Vector3.Distance(transform.position, workerHouse.transform.position) <= transferRange) 
                {
                    if (currentStorage.GetAmount() > 0 && buildingBase.GetStorage(resourceItem).GetAmount() < buildingBase.GetStorage(resourceItem).GetMaxAmount())
                    {
                        currentState = State.Depositing;
                    }
                }

                if (currentStorage.GetAmount()  >= maxStorage && buildingBase.GetStorage(resourceItem).GetAmount() >= buildingBase.GetStorage(resourceItem).GetMaxAmount())
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

                if (currentStorage.GetAmount() < maxStorage && buildingBase.GetStorage(resourceItem).GetAmount() < buildingBase.GetStorage(resourceItem).GetMaxAmount())
                {
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
                    currentState= State.Moving;
                }
                break;
        }
    }
}
