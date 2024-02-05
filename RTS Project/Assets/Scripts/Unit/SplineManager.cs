using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(LineRenderer))]
public class SplineManager : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void UpdateLineRenderer(Spline spline)
    {
        lineRenderer.positionCount = spline.Knots.Count();

        List<BezierKnot> knots = spline.Knots.ToList();

        for (int i = 0; i < spline.Knots.Count(); i++)
        {
            lineRenderer.SetPosition(i, knots[i].Position);
        }
    }
}
