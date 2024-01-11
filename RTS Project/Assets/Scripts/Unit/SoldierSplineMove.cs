using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;
using UnityEngine.Splines;
using UnityEngine.UIElements;

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
                    print("hi");
                }

                splineContainer = Instantiate(splineContainerPrefab).GetComponent<SplineContainer>();

                spline = new();
            }

            if (Input.GetMouseButton(0))
            {
                BezierKnot knot = new();
                knot.Position = hit.point;
                knot.Rotation = Quaternion.identity;
                knot.TangentIn = Vector3.forward;
                knot.TangentOut = Vector3.forward;
                spline.Add(knot);
            }

            if (Input.GetMouseButtonUp(0))
            {
                splineContainer.Spline = spline;

                //List<float> distanceBetweenPreviousList = new();
                //List<BezierKnot> knotsToRemove = new();

                List<float> distanceBetweenNodesList = new();
                List<BezierKnot> nodes = new();

                print("Length before : " + spline.Knots.ToList().Count);

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
                    if (distanceBetweenNodesList[i] < 3f)
                    {
                        spline.Remove(nodes[i]);
                    }
                }

                print("Length after : " + spline.Knots.ToList().Count);

                List<Unit> units = FindObjectsOfType<Unit>().ToList();
                List<NavMeshAgent> unitAgents = new();

                foreach (Unit unit in units)
                {
                    unitAgents.Add(unit.GetComponent<NavMeshAgent>());
                }

                float splineLength = spline.GetLength();
                float distanceBetweenNodes = 3;
                float jumpAmount = distanceBetweenNodes / splineLength;
                int amountOfNodes = (int)(splineLength / distanceBetweenNodes);
                float currentPos = 0;

                for (int i = 0; i < amountOfNodes; i++)
                {
                    if (unitAgents[i] == null) return;

                    print("hi");
                    unitAgents[i].SetDestination(spline.EvaluatePosition(currentPos));
                    currentPos += jumpAmount;
                }

                print("move soldiers");
            }
        }
    }
}
