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
        if (Input.anyKeyDown)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Input.GetMouseButtonDown(0))//left mouse button
            {
                if (selectedBuilding != null)
                {
                    selectedBuilding.DeselectBuilding();
                }

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, building))
                {
                    selectedBuilding = hit.transform.GetComponent<BuildingBase>();
                    selectedBuilding.SelectBuilding();
                }

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectable))
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
                }
                else
                {
                    DeselectAllUnits();
                }
            }
            else if (Input.GetMouseButtonDown(1))//right mouse button
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, building))
                {
                    Debug.Log("Selected building");
                    BuildingSelected(hit.transform.gameObject);
                    buildingToAttack = hit.transform.gameObject;

                    foreach (GameObject selectedUnit in selectedUnits)
                    {
                        SoldierUnit soldier = selectedUnit.GetComponent<SoldierUnit>();
                        foreach (var unit in selectedUnits)
                        {
                            unit.GetComponent<Unit>().SendUnitToLocation(hit.point);
                            buildingPosition = hit.point;
                        }
                    }
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
                {
                    Instantiate(markerPrefab, hit.point, Quaternion.identity);
                    foreach (var unit in selectedUnits)
                    {
                        unit.GetComponent<Unit>().SendUnitToLocation(hit.point);
                    }
                }
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, Enemy))
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
            else if (Input.GetMouseButtonDown(2))//middle mouse
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectable) && hit.collider.GetComponent<Worker>())
                {
                    hit.collider.gameObject.GetComponent<Worker>().UnAssignWorker();
                }
            }
        }

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
        //Vector2 boxStart = new Vector2(selectionBox.left, selectionBox.top);
        //Vector2 boxEnd = new Vector2(selectionBox.right, selectionBox.bottom);
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