using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private LayerMask selectable;
    [SerializeField] private LayerMask ground;
    [SerializeField] private List<GameObject> unitsSelected = new();

    [SerializeField] private GameObject marker;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        marker.SetActive(false);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectable))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (!unitsSelected.Contains(hit.collider.gameObject))
                        {
                            unitsSelected.Add(hit.collider.gameObject);
                            hit.collider.GetComponent<Unit>().SetSelectionObject(true);
                        }
                    }
                    else
                    {
                        DeselectAll();
                        unitsSelected.Add(hit.collider.gameObject);
                        hit.collider.GetComponent<Unit>().SetSelectionObject(true);
                    }
                }
                else
                {
                    DeselectAll();
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
                {
                    print("hi");
                    marker.transform.position = hit.collider.transform.position;
                    marker.SetActive(true);
                }
            }
        }
    }

    private void DeselectAll()
    {
        foreach (GameObject unit in unitsSelected)
        {
            unit.GetComponent<Unit>().SetSelectionObject(false);
        }
        unitsSelected.Clear();
    }
}