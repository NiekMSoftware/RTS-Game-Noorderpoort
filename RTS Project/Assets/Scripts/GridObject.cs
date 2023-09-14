using UnityEngine;

public class GridObject : MonoBehaviour
{
    private void Awake()
    {
        Setup();
    }

    private void Setup()
    {
        GridManager.Instance.GetClosestPointOnGrid(Vector3.left);
        //transform.position = GridManager.Instance.grid[]
    }
}