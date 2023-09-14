using UnityEngine;
using UnityEngine.AI;

public class Aigridtest : MonoBehaviour
{
    [SerializeField] private Vector2Int pos;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(GridManager.Instance.grid[pos.x, pos.y].pos);
    }
}