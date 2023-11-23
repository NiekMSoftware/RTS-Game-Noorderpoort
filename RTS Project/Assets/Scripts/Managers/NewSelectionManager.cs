using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSelectionManager : MonoBehaviour
{
    [SerializeField] private List<Unit> selectedUnits = new();
    [SerializeField] private RectTransform selectionBoxVisual;
    [SerializeField] private GameObject markerPrefab;

    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask buildingLayer;

    private Rect selectionBox;
    private Vector2 boxStartPosition;
    private Vector2 boxEndPosition;

    private Camera mainCamera;

    private GameObject buildingToAttack;

    private Marker marker;

    private void Awake()
    {
        mainCamera = Camera.main;

        boxStartPosition = Vector2.zero;
        boxEndPosition = Vector2.zero;
        DrawBoxVisual();
    }

    public void Update()
    {
        HandleSelection();

        HandleBoxSelect();
    }

    #region Handle Unit/Building Selection

    private void HandleSelection()
    {
        if (!Input.anyKeyDown) return;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))//left mouse button
        {
            bool hasSelectedSuccessfully = SelectSingleUnit(ray);

            SelectBuilding(ray);

            if (!hasSelectedSuccessfully)
            {
                DeselectAllUnits();
            }
        }
        else if (Input.GetMouseButtonDown(1))//right mouse button
        {
            MoveUnits(ray);
        }
        else if (Input.GetMouseButtonDown(2))//middle mouse
        {
            UnassignWorkerTest(ray);
        }
    }

    private void UnassignWorkerTest(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unitLayer) && hit.collider.GetComponent<Worker>())
        {
            hit.collider.gameObject.GetComponent<Worker>().UnAssignWorker();
        }
    }

    private void SelectBuilding(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, buildingLayer))
        {
            print("Selected Building");
            if (!hit.transform.TryGetComponent(out BuildingBase building)) return;

            if (selectedUnits.Count > 0)
            {
                foreach (Unit unit in selectedUnits)
                {
                    switch (unit)
                    {
                        case Unit soldier when unit is SoldierUnit:
                            //Send soldier to enemy building
                            if (building.GetOccupancyType() == BuildingBase.OccupancyType.Enemy)
                            {
                                //Change to buildingbase when soldierunit is changed
                                buildingToAttack = building.gameObject;

                                soldier.SendUnitToLocation(building.transform.position);
                            }
                            break;

                        case Worker worker when unit is Worker:
                            switch (building)
                            {
                                case Barrack barrack when building is Barrack:
                                    //Send worker to barrack
                                    barrack.AddUnitToBarrack(worker);
                                    break;

                                case ResourceBuildingBase workerBuilding when building is ResourceBuildingBase:
                                    //Send worker to building
                                    workerBuilding.AddWorkerToBuilding(worker);
                                    break;
                            }
                            break;
                    }
                }
            }
            else
            {
                //Select building
                building.SelectBuilding();
            }
        }
    }

    private void MoveUnits(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            if (selectedUnits.Count <= 0) return;

            if (marker)
            {
                Destroy(marker.gameObject);
            }

            marker = Instantiate(markerPrefab, hit.point, Quaternion.identity).GetComponent<Marker>();
            foreach (var unitObject in selectedUnits)
            {
                Unit unit = unitObject.GetComponent<Unit>();
                unit.SendUnitToLocation(hit.point);
                marker.SetUnit(unit);
            }
        }
    }

    private bool SelectSingleUnit(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unitLayer))
        {
            Unit unit = hit.transform.GetComponent<Unit>();

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (!selectedUnits.Contains(unit))
                {
                    AddUnit(unit);
                }
            }
            else
            {
                DeselectAllUnits();
                AddUnit(unit);
            }

            return true;
        }

        return false;
    }

    private void DeselectAllUnits()
    {
        foreach (Unit unit in selectedUnits)
        {
            unit.SetSelectionObject(false);
        }

        selectedUnits.Clear();
    }

    private void AddUnit(Unit unit)
    {
        selectedUnits.Add(unit);
        unit.SetSelectionObject(true);
    }

    #endregion

    #region Box Selection

    private void HandleBoxSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            boxStartPosition = Input.mousePosition;
            selectionBox = new Rect();
        }

        if (Input.GetMouseButton(0))
        {
            boxEndPosition = Input.mousePosition;
            DrawBoxVisual();
            CalculateSelection();
        }

        if (Input.GetMouseButtonUp(0))
        {
            BoxSelectUnits();

            boxStartPosition = Vector2.zero;
            boxEndPosition = Vector2.zero;
            DrawBoxVisual();
        }
    }

    private void BoxSelectUnits()
    {
        Unit[] allUnits = FindObjectsOfType<Unit>();

        foreach (var unit in allUnits)
        {
            if (selectionBox.Contains(mainCamera.WorldToScreenPoint(unit.transform.position)))
            {
                if (!selectedUnits.Contains(unit))
                {
                    AddUnit(unit);
                }
            }
        }
    }

    private void DrawBoxVisual()
    {
        Vector2 boxStart = boxStartPosition;
        Vector2 boxEnd = boxEndPosition;

        Vector2 boxCenter = (boxStart + boxEnd) / 2;
        selectionBoxVisual.position = boxCenter;

        Vector2 boxSize = new(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));

        selectionBoxVisual.sizeDelta = boxSize;
    }

    private void CalculateSelection()
    {
        if (Input.mousePosition.x < boxStartPosition.x)
        {
            selectionBox.xMin = Input.mousePosition.x;
            selectionBox.xMax = boxStartPosition.x;
        }
        else
        {
            selectionBox.xMax = Input.mousePosition.x;
            selectionBox.xMin = boxStartPosition.x;
        }

        if (Input.mousePosition.y < boxStartPosition.y)
        {
            selectionBox.yMin = Input.mousePosition.y;
            selectionBox.yMax = boxStartPosition.y;
        }
        else
        {
            selectionBox.yMax = Input.mousePosition.y;
            selectionBox.yMin = boxStartPosition.y;
        }
    }

    #endregion

    #region Public Getters

    public GameObject GetBuildingToAttack() => buildingToAttack;

    #endregion
}
