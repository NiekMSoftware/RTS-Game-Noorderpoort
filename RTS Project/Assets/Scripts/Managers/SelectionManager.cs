using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private LayerMask selectable;
    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask building;
    [SerializeField] private LayerMask Enemy;

    public List<GameObject> selectedUnits = new();

    [SerializeField] private GameObject markerPrefab;

    [SerializeField] RectTransform boxVisual;
    public GameObject selectedEnemy;

    private BuildingBase selectedBuilding;
    private GameObject buildingToAttack;

    private Marker marker;

    Rect selectionBox;

    Vector2 boxStartPosition;
    Vector2 boxEndPosition;

    private Camera mainCamera;

    public Vector3 buildingPosition;
    public Vector3 enemyPosition;

    private void Start()
    {
        mainCamera = Camera.main;

        boxStartPosition = Vector2.zero;
        boxEndPosition = Vector2.zero;
        DrawBoxVisual();
    }

    private void Update()
    {
        HandleSelection();

        BoxSelection();
    }

    private void BoxSelection()
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
            UnitSelection();

            boxStartPosition = Vector2.zero;
            boxEndPosition = Vector2.zero;
            DrawBoxVisual();
        }
    }

    private void HandleSelection()
    {
        if (!Input.anyKeyDown) return;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))//left mouse button
        {
            bool hasSelectedSuccessfully = SelectUnits(ray);

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
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectable) && hit.collider.GetComponent<Worker>())
        {
            hit.collider.gameObject.GetComponent<Worker>().UnAssignWorker();
        }
    }

    private void AttackEnemies(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, Enemy))
        {
            selectedEnemy = hit.collider.gameObject;

            foreach (GameObject selectedUnit in selectedUnits)
            {
                SoldierUnit soldier = selectedUnit.GetComponent<SoldierUnit>();
                foreach (var unit in selectedUnits)
                {
                    unit.GetComponent<Unit>().SendUnitToLocation(hit.point);
                    print(hit);
                    enemyPosition = hit.point;
                }
            }
        }
    }

    private void MoveUnits(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
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

    private bool SelectUnits(Ray ray)
    {
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
                DeselectAllUnits();
                selectedUnits.Add(hit.collider.gameObject);
                hit.collider.GetComponent<Unit>().SetSelectionObject(true);
            }

            return true;
        }

        return false;
    }

    private void SelectBuilding(Ray ray)
    {
        if (selectedBuilding != null)
        {
            selectedBuilding.DeselectBuilding();
        }

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, building))
        {
            print("Selected building");
            if (hit.transform.TryGetComponent(out selectedBuilding))
                selectedBuilding.SelectBuilding();

            BuildingSelected(hit.transform.gameObject);
            buildingToAttack = hit.transform.gameObject;
            print(buildingToAttack);

            print(selectedUnits.Count);

            foreach (GameObject selectedUnit in selectedUnits)
            {
                print("moving");
                selectedUnit.GetComponent<Unit>().SendUnitToLocation(hit.point);
                buildingPosition = hit.point;
            }
        }
    }

    private void BuildingSelected(GameObject hit)
    {
        if (selectedUnits.Count > 0)
        {
            foreach (GameObject unit in selectedUnits)
            {
                // Change when worker is integrated into unit
                // selectedBuilding.GetComponent<BuildingBase>().AddWorkerToBuilding(unit.GetComponent<Worker>());

                // Perhaps make it so we can use an if / else if - statement
                // What this will do is add more accessibility
                // Perhaps make this a SWITCH-statement if absolutely necessarily
                if (selectedBuilding.TryGetComponent<ResourceBuildingBase>(out ResourceBuildingBase buildingBase))
                {
                    print("Assigning unit to Worker");
                    buildingBase.AddWorkerToBuilding(unit.GetComponent<Worker>());
                }
                else
                {
                    print("Assigning Unit to soldier");
                    selectedBuilding.GetComponent<Barrack>().AddUnitToBarrack(null);
                }

                selectedBuilding.GetComponent<BuildingBase>().StartAnimateOutline();
            }
        }
    }

    private void DeselectAllUnits()
    {
        foreach (GameObject unit in selectedUnits)
        {
            unit.GetComponent<Unit>().SetSelectionObject(false);
        }
        selectedUnits.Clear();
    }

    void DrawBoxVisual()
    {
        Vector2 boxStart = boxStartPosition;
        Vector2 boxEnd = boxEndPosition;

        Vector2 boxCenter = (boxStart + boxEnd) / 2;
        boxVisual.position = boxCenter;

        Vector2 boxSize = new(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));

        boxVisual.sizeDelta = boxSize;

    }

    void CalculateSelection()
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

    public GameObject GetBuildingToAttack() => buildingToAttack;
}