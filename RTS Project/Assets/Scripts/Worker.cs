using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Jobs : MonoBehaviour
{
    private Transform resourceTarget;
    [SerializeField] private GameObject workerHouse;
    private string resourceTag = "StoneResource";
    private float scanRange = 100f;
    NavMeshAgent myAgent;
    private State currentState = State.Idle;
    private bool inventorFull = false;

    public enum State
    {
        Idle,
        Moving,
        Gathering,
    }

    private void Start()
    {
        myAgent = GetComponent<NavMeshAgent>();
    }
    private void IdleState()
    {
        myAgent.isStopped = true;
        resourceTarget = FindClosestResource();
    }

    private void MovingState()
    {
        if (resourceTarget != null)
        {
            myAgent.isStopped = false;
            myAgent.SetDestination(resourceTarget.position);
        }
    }

    private void GatheringState()
    {
        // Gather go brrr
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
        // Play gather animation or perform gathering action here
        yield return new WaitForSeconds(3f);
        myAgent.isStopped = false;
        inventorFull = true;
        resourceTarget = null;
        currentState = State.Moving;
        myAgent.SetDestination(workerHouse.transform.position);
    }
    private void Update()
    {  
        switch (currentState)
        {
            case State.Idle:
                IdleState();
                if (resourceTarget != null )
                {
                    currentState = State.Moving;
                }
                break;

            case State.Moving:
                MovingState();
                if (resourceTarget != null && Vector3.Distance(transform.position, resourceTarget.position) <= 2.5f)
                { 
                    currentState = State.Gathering;
                }
                if (resourceTarget == null && !inventorFull)
                {
                    currentState = State.Idle;
                }
                if (Vector3.Distance(transform.position, workerHouse.transform.position) <= 2.5f)
                {
                    myAgent.isStopped = true;
                    currentState = State.Idle;
                }
                break;

            case State.Gathering:
                GatheringState();
                myAgent.isStopped = true;
                StartCoroutine(GatherResource());

                break;

            default:
                print("switch shitting itself");
                break;

        }
    }
}
