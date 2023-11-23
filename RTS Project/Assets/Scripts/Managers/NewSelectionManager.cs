using System.Collections.Generic;
using UnityEngine;

public class NewSelectionManager : MonoBehaviour
{
    [SerializeField] private List<Unit> selectedUnits = new();
    [SerializeField] private RectTransform selectionBoxVisual;

    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask buildingLayer;

    private Rect selectionBox;
    private Vector2 boxStartPosition;
    private Vector2 boxEndPosition;

    private Camera mainCamera;

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

    private void HandleSelection()
    {
        if (!Input.anyKeyDown) return;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))//left mouse button
        {
            //bool hasSelectedSuccessfully = SelectSingleUnit(ray);

            //SelectBuilding(ray);

            //if (!hasSelectedSuccessfully)
            //{
            //    DeselectAllUnits();
            //}
        }
        else if (Input.GetMouseButtonDown(1))//right mouse button
        {
            //MoveUnits(ray);
        }
        else if (Input.GetMouseButtonDown(2))//middle mouse
        {
            //UnassignWorkerTest(ray);
        }
    }

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

    private void SelectSingleUnit(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unitLayer))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {

            }
            else
            {
                //AddUnit(hit)
            }
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

    private void AddUnit(Unit unit)
    {
        selectedUnits.Add(unit);
        unit.SetSelectionObject(true);
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
}
