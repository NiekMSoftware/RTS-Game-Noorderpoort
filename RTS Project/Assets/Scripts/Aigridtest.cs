using UnityEngine;
using UnityEngine.AI;

public class Aigridtest : MonoBehaviour
{
    [SerializeField] private Vector2Int pos;

    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        print(GridManager.Instance.grid[pos.x, pos.y].pos);
        agent.SetDestination(new Vector3(GridManager.Instance.grid[pos.x, pos.y].pos.x, 0, GridManager.Instance.grid[pos.x, pos.y].pos.z));
    }
}