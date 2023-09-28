using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private LayerMask selectable;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask building;
    [SerializeField] private List<GameObject> selectedUnits = new();
    [SerializeField] private GameObject marker;

    [SerializeField] RectTransform boxVisual;
    private GameObject selectedBuilding;

    Rect selectionBox;

    Vector2 startPosition;
    Vector2 endPosition;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        marker.SetActive(false);

        startPosition = Vector2.zero;
        endPosition = Vector2.zero;
        DrawBoxVisual();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out RaycastHit hit2, Mathf.Infinity, building))
                {
                    selectedBuilding = hit2.collider.gameObject;
                    BuildingSelected();
                }
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectable))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (!selectedUnits.Contains(hit.collider.gameObject))
                        {
                            selectedUnits.Add(hit.collider.gameObject);
                            hit.collider.GetComponent<Unit>().SetSelectionObject(true);
                        }
                    }
                    else
                    {
                        DeselectAll();
                        selectedUnits.Add(hit.collider.gameObject);
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
                    marker.transform.position = hit.point;
                    marker.SetActive(true);
                    foreach (var unit in selectedUnits)
                    {
                        unit.GetComponent<Unit>().SendUnitToLocation(marker.transform.position);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            selectionBox = new Rect();
        }

        if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;
            DrawBoxVisual();
            DrawSelection();
        }

        if (Input.GetMouseButtonUp(0))
        {
            UnitSelection();

            startPosition = Vector2.zero;
            endPosition = Vector2.zero;
            DrawBoxVisual();
        }
    }
    private void BuildingSelected()
    {
        if (selectedUnits.Count > 0)
        {
            foreach (GameObject unit in selectedUnits)
            {
                // Change when worker is integrated into unit
                selectedBuilding.GetComponent<BuildingBase>().AddWorkerToBuilding(unit.GetComponent<Worker>());
            }
        }
    }
    private void DeselectAll()
    {
        foreach (GameObject unit in selectedUnits)
        {
            unit.GetComponent<Unit>().SetSelectionObject(false);
        }
        selectedUnits.Clear();
    }

    void DrawBoxVisual()
    {
        Vector2 boxStart = startPosition;
        Vector2 boxEnd = endPosition;

        Vector2 boxCenter = (boxStart + boxEnd) / 2;
        boxVisual.position = boxCenter;

        Vector2 boxSize = new(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));

        boxVisual.sizeDelta = boxSize;
    }

    void DrawSelection()
    {
        if (Input.mousePosition.x < startPosition.x)
        {
            selectionBox.xMin = Input.mousePosition.x;
            selectionBox.xMax = startPosition.x;
        }
        else
        {
            selectionBox.xMax = Input.mousePosition.x;
            selectionBox.xMin = startPosition.x;
        }

        if (Input.mousePosition.y < startPosition.y)
        {
            selectionBox.yMin = Input.mousePosition.y;
            selectionBox.yMax = startPosition.y;
        }
        else
        {
            selectionBox.yMax = Input.mousePosition.y;
            selectionBox.yMin = startPosition.y;
        }
    }

    void UnitSelection()
    {
        Unit[] allUnits = FindObjectsOfType<Unit>();

        foreach (var unit in allUnits)
        {
            if (selectionBox.Contains(mainCamera.WorldToScreenPoint(unit.transform.position)))
            {
                if (!selectedUnits.Contains(unit.gameObject))
                {
                    selectedUnits.Add(unit.gameObject);
                    unit.GetComponent<Unit>().SetSelectionObject(true);
                }
            }
        }
    }
}