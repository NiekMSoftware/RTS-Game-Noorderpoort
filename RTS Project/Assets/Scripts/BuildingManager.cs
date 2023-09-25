using System.Collections.Generic;
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
                ChangeObjectMaterial(pendingObject, incorrectPlaceMaterial);
            }
            else
            {
                ChangeObjectMaterial(pendingObject, correctPlaceMaterial);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ResetObject();
            }

            if (Input.GetMouseButtonDown(0) && !GridManager.Instance.GetOccupanyPendingObject())
            {
                PlaceObject();
            }
        }
    }

    public GameObject GetPendingObject() => pendingObject;

    private void ResetObject()
    {
        Destroy(pendingObject);
        pendingObject = null;
        currentIndex = -1;
        pos = Vector3.zero;
    }

    private void PlaceObject()
    {
        Instantiate(objects[currentIndex].model, pos, transform.rotation);
        ResetObject();
        GridManager.Instance.CheckOccupancy();
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

        ChangeObjectMaterial(pendingObject, correctPlaceMaterial);

        currentIndex = index;
    }

    private void ChangeObjectMaterial(GameObject go, Material material)
    {
        if (go.TryGetComponent(out MeshRenderer mr))
        {
            if (mr.material)
            {
                mr.material = material;
            }
        }
        else
        {
            foreach (var mr2 in go.GetComponentsInChildren<MeshRenderer>())
            {
                List<Material> materials = new();

                for (int i = 0; i < mr2.materials.Length; i++)
                {
                    materials.Add(material);
                }

                mr2.SetMaterials(materials);
            }
        }
    }
}
