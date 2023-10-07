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
        transform.position = Vector3Int.CeilToInt(transform.position);
        Vector3 newPos = transform.position;
        newPos.y = oldPos.y;
        transform.position = newPos;
        GridManager.Instance.CheckOccupancy(transform.position);
    }
}