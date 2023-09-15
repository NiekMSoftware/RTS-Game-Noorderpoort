using UnityEngine;

public class GridObject : MonoBehaviour
{
    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        transform.position = GridManager.Instance.GetClosestPointOnGrid(transform.position);
        //transform.position = GridManager.Instance.grid[]
        //transform.position = new Vector3(Mathf.Floor(transform.position.x), Mathf.Floor(transform.position.y), Mathf.Floor(transform.position.z));
    }
}