using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

public class SoldierSplineMove : MonoBehaviour
{
    [SerializeField] private NewSelectionManager selectionManager;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject splineContainerPrefab;

    private SplineContainer splineContainer;
    private Spline spline;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;

        enabled = false;
    }

    private void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            //Setup
            if (Input.GetMouseButtonDown(0))
            {
                if (splineContainer)
                {
                    Destroy(splineContainer.gameObject);
                }

                splineContainer = Instantiate(splineContainerPrefab).GetComponent<SplineContainer>();

                spline = new();
            }

            if (Input.GetMouseButton(0))
            {
                BezierKnot knot = new();
                knot.Position = hit.point;
                spline.Add(knot);
                splineContainer.gameObject.GetComponent<SplineManager>().UpdateLineRenderer(spline);
            }

            if (Input.GetMouseButtonUp(0))
            {
                selectionManager.SetMayDrawSelectionBox(true);

                splineContainer.Splines.ToList().Add(spline);

                List<float> distanceBetweenNodesList = new();
                List<BezierKnot> nodes = new();

                float knotsLength = spline.Knots.ToList().Count;

                foreach (var knotA in spline.Knots)
                {
                    foreach (var knotB in spline.Knots)
                    {
                        if (knotA.GetHashCode() == knotB.GetHashCode()) continue;

                        float distanceBetweenPrevious = Vector3.Distance(knotA.Position, knotB.Position);
                        distanceBetweenNodesList.Add(distanceBetweenPrevious);

                        nodes.Add(knotA);
                    }
                }

                for (int i = 0; i < nodes.Count; i++)
                {
                    if (distanceBetweenNodesList[i] < 0.5f)
                    {
                        spline.Remove(nodes[i]);
                    }
                }

                splineContainer.gameObject.GetComponent<SplineManager>().UpdateLineRenderer(spline);

                print("knots removed : " + (knotsLength - spline.Knots.ToList().Count));

                List<Unit> units = selectionManager.GetSelectedUnits();
                List<NavMeshAgent> unitAgents = new();

                foreach (Unit unit in units)
                {
                    unitAgents.Add(unit.GetComponent<NavMeshAgent>());
                }

                float splineLength = spline.GetLength();
                float distanceBetweenNodes = 3;
                int amountOfNodes = (int)(splineLength / distanceBetweenNodes);
                float jumpAmount = distanceBetweenNodes / splineLength;
                float currentPos = 0;

                if (amountOfNodes > unitAgents.Count)
                {
                    distanceBetweenNodes = splineLength / unitAgents.Count;
                    jumpAmount = distanceBetweenNodes / splineLength;
                }

                for (int i = 0; i < amountOfNodes; i++)
                {
                    if (i >= unitAgents.Count) return;
                    if (unitAgents[i] == null) return;

                    print(currentPos);
                    NavMeshPath path = new();
                    Vector3 splinePos = spline.EvaluatePosition(currentPos);
                    unitAgents[i].CalculatePath(splinePos, path);
                    unitAgents[i].SetDestination(splinePos);
                    currentPos += jumpAmount;
                }

                selectionManager.SetMayDeselect(true);
                enabled = false;
            }
        }
    }
}
