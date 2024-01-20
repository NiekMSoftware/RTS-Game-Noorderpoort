using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NewSelectionManager : MonoBehaviour
{
    [SerializeField] private List<Unit> selectedUnits = new();
    [SerializeField] private RectTransform selectionBoxVisual;
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private BuildingSelect buildingSelectUI;

    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask buildingLayer;

    private Rect selectionBox;
    private Vector2 boxStartPosition;
    private Vector2 boxEndPosition;

    private Camera mainCamera;

    private BuildingBase buildingToAttack;
    private Unit enemyToAttack;

    private Marker marker;

    [SerializeField] private BuildingBase selectedBuilding;
    private Unit selectedUnit;

    private UIManager uiManager;

    private bool mayDrawSelectionBox = true;
    private bool mayDeselect = true;

    private void Awake()
    {
        mainCamera = Camera.main;

        boxStartPosition = Vector2.zero;
        boxEndPosition = Vector2.zero;
        DrawBoxVisual();

        uiManager = FindObjectOfType<UIManager>();
    }

    public void Update()
    {
        print(mayDeselect);

        if (selectedUnits.Count != 1 && selectedUnit)
        {
            if (mayDeselect)
            {
                print("deselecting");
                selectedUnit.Deselect();
                selectedUnit = null;
            }
            else
            {
                print("may deselect : false");
            }

            uiManager.SetUnitUI(false, null);
        }

        HandleSelection();

        HandleBoxSelect();

        if (selectedUnits.Count == 1)
        {
            uiManager.SetUnitUI(true, selectedUnits[0]);
            selectedUnit = selectedUnits[0];
            selectedUnit.Select();
        }

        if (selectedUnits.Count > 1)
        {
            bool isSoldier = false;

            foreach (var unit in selectedUnits)
            {
                if (unit is SoldierUnit)
                {
                    isSoldier = true;
                }
            }

            uiManager.SetMultiSoldierUI(isSoldier);
        }
    }

    #region Handle Unit/Building Selection

    private void HandleSelection()
    {
        if (!Input.anyKeyDown) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))//left mouse button
        {
            bool hasSelectedSuccessfully = SelectSingleUnit(ray);

            if (selectedBuilding != null)
                if (!buildingSelectUI.GetIsSelecting())
                    selectedBuilding.DeselectBuilding();

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
    }

    private void SelectBuilding(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, buildingLayer))
        {
            if (!hit.transform.TryGetComponent(out BuildingBase building)) return;
            if (building.GetCurrentState() == BuildingBase.States.Building) return;

            selectedBuilding = building;

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
                                buildingToAttack = building;

                                soldier.SendUnitToLocation(building.transform.position);
                                soldier.SetCurrentAction("Attacking " + building.buildingName);
                            }
                            break;

                        case Worker worker when unit is Worker:
                            switch (building)
                            {
                                case Barrack barrack when building is Barrack:
                                    //Send worker to barrack
                                    barrack.AddUnitToBarrack(worker.gameObject);
                                    worker.SetCurrentAction("Going to train at " + barrack.buildingName);
                                    break;

                                case ResourceBuildingBase workerBuilding when building is ResourceBuildingBase:
                                    //Send worker to building
                                    if (workerBuilding.AddWorkerToBuilding(worker))
                                    {
                                        worker.SetCurrentAction("Going to work at " + workerBuilding.buildingName);
                                    }
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
            foreach (var unit in selectedUnits)
            {
                if (unit.TryGetComponent(out SoldierUnit soldier))
                {
                    soldier.enemyUnit = null;
                    enemyToAttack = null;
                    print("set enemy to null");
                }

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

            switch (unit.typeUnit)
            {
                case Unit.TypeUnit.Human:
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
                    break;

                case Unit.TypeUnit.Enemy:
                    AttackEnemy(hit, unit);
                    break;
            }

            return true;
        }

        return false;
    }

    private void AttackEnemy(RaycastHit hit, Unit enemy)
    {
        enemyToAttack = enemy;

        foreach (var soldier in selectedUnits)
        {
            if (soldier is SoldierUnit)
            {
                soldier.SendUnitToLocation(hit.point);
            }
        }
    }

    public void DeselectAllUnits()
    {
        if (!mayDeselect) return;

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
        if (!mayDrawSelectionBox) return;

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
                    if (unit.typeUnit == Unit.TypeUnit.Human)
                    {
                        AddUnit(unit);
                    }
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

    public BuildingBase GetBuildingToAttack() => buildingToAttack;

    public Unit GetEnemyToAttack() => enemyToAttack;

    public List<Unit> GetSelectedUnits() => selectedUnits;

    public Marker GetMarker() => marker;

    public void SetMayDrawSelectionBox(bool value) => mayDrawSelectionBox = value;

    public void SetMayDeselect(bool value) => mayDeselect = value;

    #endregion
}
