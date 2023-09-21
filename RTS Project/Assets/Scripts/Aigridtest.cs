using UnityEngine;
using UnityEngine.AI;

public class Aigridtest : MonoBehaviour
{
    [SerializeField] private Vector3Int pos;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(pos);
    }
}