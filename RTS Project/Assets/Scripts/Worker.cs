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
    private State currentState = State.Moving;
    [SerializeField] protected ItemSlot currentStorage;
    [SerializeField] protected int maxStorage = 3;
    [SerializeField] protected float workerHp = 5f;
    [SerializeField] private ItemData resourceItem;
    private bool canGather = true;
    private bool canDeposit = true;
    private enum State{Moving, Idle, Gathering, Depositing}
    private BuildingBase buildingBase;

    private void Start()
    {
        buildingBase = workerHouse.GetComponent<BuildingBase>();

        ItemSlot itemSlot = new();
        itemSlot.SetData(resourceItem);
        itemSlot.SetAmount(0);
        currentStorage = itemSlot;
        myAgent = GetComponent<NavMeshAgent>();
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
            myAgent.isStopped = false;
            AddItemToWorkerStorage(resourceItem);
            yield return new WaitForSeconds(1f);
        }
        resourceTarget = null;
        currentState = State.Moving;

        yield return null;
    }

    private IEnumerator DepositResources()
    {
        while (currentStorage.GetAmount() > 0)
        {
            myAgent.isStopped = false;
            if (buildingBase.currentStorage.GetAmount() < buildingBase.maxStorage)
            {
                RemoveItemFromWorkerStorage(resourceItem);
                buildingBase.AddItemToStorage(resourceItem);
            }
            yield return new WaitForSeconds(1f);
        }
        currentState = State.Idle;
        canDeposit = true;

        yield return null;
    }
    private void Update()
    {
        print(currentStorage.GetAmount());

        switch (currentState)
        {
            case State.Moving:
                canGather = true;
                StopAllCoroutines();
                if (resourceTarget == null)
                {
                    resourceTarget = FindClosestResource();
                }
                if (resourceTarget != null)
                {
                    myAgent.isStopped = false;
                    myAgent.SetDestination(resourceTarget.position);
                }
                if (resourceTarget != null && Vector3.Distance(transform.position, resourceTarget.position) <= 2.5f && currentStorage.GetAmount() < 3)
                {
                    currentState = State.Gathering;
                }
                if (currentStorage.GetAmount() == 3)
                {
                    myAgent.SetDestination(workerHouse.transform.position);
                }
                if (Vector3.Distance(transform.position, workerHouse.transform.position) <= 2.5f && currentStorage.GetAmount() > 0 )
                {
                    currentState = State.Depositing;
                }
                break;

            case State.Idle:
                myAgent.isStopped = true;
                if (currentStorage.GetAmount() == 0)
                {
                    currentState = State.Moving;
                }
                if (resourceTarget != null )
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
                if (Vector3.Distance(transform.position, workerHouse.transform.position) <= 2.5f)
                {
                    myAgent.isStopped = true;
                    if (canDeposit)
                    {
                        canDeposit = false;
                        StartCoroutine(DepositResources());
                    }
                }
                else
                {
                    currentState = State.Idle;
                }
                break;

            default:
                print("Defaulting");
                break;
        }
    }
}
