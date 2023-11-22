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
    public GameObject selectedBuilding;
    public GameObject selectedEnemy;

    private BuildingBase selectedBuilding2;

    Rect selectionBox;

    Vector2 startPosition;
    Vector2 endPosition;

    private Camera mainCamera;

    public Vector3 buildingPosition;
    public Vector3 enemyPosition;

    private void Start()
    {
        mainCamera = Camera.main;

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
                if (selectedBuilding2 != null)
                {
                    selectedBuilding2.DeselectBuilding();
                }

                if (Physics.Raycast(ray, out RaycastHit hit1, Mathf.Infinity, building))
                {
                    selectedBuilding2 = hit1.transform.GetComponent<BuildingBase>();
                    selectedBuilding2.DeselectBuilding();
                    selectedBuilding2.SelectBuilding();
                }

                if (Physics.Raycast(ray, out RaycastHit hit2, Mathf.Infinity, selectable))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (!selectedUnits.Contains(hit2.collider.gameObject))
                        {
                            selectedUnits.Add(hit2.collider.gameObject);
                            hit2.collider.GetComponent<Unit>().SetSelectionObject(true);
                        }
                    }
                    else
                    {
                        DeselectAll();
                        selectedUnits.Add(hit2.collider.gameObject);
                        hit2.collider.GetComponent<Unit>().SetSelectionObject(true);
                    }
                }
                else
                {
                    DeselectAll();
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(ray, out RaycastHit hit2, Mathf.Infinity, building))
                {
                    Debug.Log("Selected building");
                    selectedBuilding = hit2.collider.gameObject;
                    BuildingSelected();

                    foreach (GameObject selectedUnit in selectedUnits)
                    {
                        SoldierUnit soldier = selectedUnit.GetComponent<SoldierUnit>();
                        foreach (var unit in selectedUnits)
                        {
                            unit.GetComponent<Unit>().SendUnitToLocation(hit2.point);
                            buildingPosition = hit2.point;
                        }
                    }
                }
                else if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
                {
                    Instantiate(markerPrefab, hit.point, Quaternion.identity);
                    foreach (var unit in selectedUnits)
                    {
                        unit.GetComponent<Unit>().SendUnitToLocation(hit.point);
                    }
                }
                if (Physics.Raycast(ray, out RaycastHit hitEnemy, Mathf.Infinity, Enemy))
                {
                    selectedEnemy = hitEnemy.collider.gameObject;

                    foreach (GameObject selectedUnit in selectedUnits)
                    {
                        SoldierUnit soldier = selectedUnit.GetComponent<SoldierUnit>();
                        foreach (var unit in selectedUnits)
                        {
                            unit.GetComponent<Unit>().SendUnitToLocation(hitEnemy.point);
                            print(hitEnemy);
                            enemyPosition = hitEnemy.point;
                        }
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