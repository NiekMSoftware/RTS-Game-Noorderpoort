using UnityEngine;

public class GridObject : MonoBehaviour
{
    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        Vector3 oldPos = transform.position;
        transform.position = GridManager.Instance.GetClosestPointOnGrid(transform.position);
        Vector3 newPos = transform.position;
        newPos.y = oldPos.y;
        transform.position = newPos;
    }
}