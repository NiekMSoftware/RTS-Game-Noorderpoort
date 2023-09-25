using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [SerializeField] Material correctPlaceMaterial;
    [SerializeField] Material incorrectPlaceMaterial;
    [SerializeField] PlaceableObject[] objects;
    private GameObject pendingObject;
    private int currentIndex = -1;

    private Vector3 pos;

    private RaycastHit hit;
    [SerializeField] private LayerMask layerMask;

    [System.Serializable]
    class PlaceableObject
    {
        public GameObject model;
        public float yHeight;
    }

    void Update()
    {
        if (pendingObject != null)
        {
            pendingObject.transform.position = pos;

            if (GridManager.Instance.GetOccupanyPendingObject())
            {
                if (pendingObject.GetComponent<MeshRenderer>().material)
                {
                    pendingObject.GetComponent<MeshRenderer>().material = correctPlaceMaterial;
                }
                else
                {
                    pendingObject.GetComponentInChildren<MeshRenderer>().material = correctPlaceMaterial;
                }
            }
            else
            {
                if (pendingObject.GetComponent<MeshRenderer>().material)
                {
                    pendingObject.GetComponent<MeshRenderer>().material = incorrectPlaceMaterial;
                }
                else
                {
                    pendingObject.GetComponentInChildren<MeshRenderer>().material = incorrectPlaceMaterial;
                }
            }



            if (Input.GetMouseButtonDown(0) && !GridManager.Instance.GetOccupanyPendingObject())
            {
                PlaceObject();
            }
        }
    }

    public GameObject GetPendingObject() => pendingObject;

    private void PlaceObject()
    {
        pendingObject = null;
        currentIndex = -1;
    }

    private void FixedUpdate()
    {
        if (currentIndex < 0) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            pos = new Vector3(GridManager.Instance.GetClosestPointOnGrid(hit.point).x, objects[currentIndex].yHeight, GridManager.Instance.GetClosestPointOnGrid(hit.point).z);
        }
    }

    public void SelectObject(int index)
    {
        pendingObject = Instantiate(objects[index].model, pos, transform.rotation);
        currentIndex = index;
    }
}
