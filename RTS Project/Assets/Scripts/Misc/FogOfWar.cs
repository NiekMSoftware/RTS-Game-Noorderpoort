using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class FogOfWar : MonoBehaviour
{
    // TODO: Refactor the cast performing so it will shoot a ray, then do an overlap sphere on said ground to check the radius
    // if there are things in that radius show them, else not (this goes for all units).

    [Header("Unit Properties")]
    public List<SoldierUnit> soldiers;
    public List<Unit> enemyUnits;

    // Dictionaries to store values
    private Dictionary<SoldierUnit, Vector3> previousPositions = new();
    Dictionary<SoldierUnit, LineRenderer> lineRenderers = new ();

    // private int to keep track of the soldiers
    private int foundSoldiers = 0;

    [Space] 
    [SerializeField] private float elapsed;
    [SerializeField] private float maxUntilNext;

    [Header("Fog of War Properties")]
    [SerializeField] private Transform fogTransform;

    private Transform soldierPosition;

    void Update()
    {
        // Gather all local soldiers
        UpdateSoldierList();

        // Gather each soldier's pos in the List
        StartCoroutine(nameof(FindPositions));

        // If there are soldiers found, send the rays
        if (foundSoldiers != 0)
        {
            StartCoroutine(SendRays(soldierPosition));
        }
    }

    void LateUpdate()
    {
        // Update the end position of each LineRenderer to the current position of its soldier
        foreach (var soldier in lineRenderers.Keys)
        {
            LineRenderer lineRenderer = lineRenderers[soldier];

            if (lineRenderer != null)
            {
                lineRenderers[soldier].SetPosition(1, soldier.transform.position);

            }
        }
    }

    IEnumerator SendRays(Transform positions)
    {
        // iterate through each item in list, then check if they moved
        foreach (var soldier in soldiers)
        {
            // if the previousPos isn't stored, store it.
            if (!previousPositions.ContainsKey(soldier))
            {
                previousPositions[soldier] = soldier.transform.position;

                // after waiting send out a ray from the camera to the position of the soldier
                Vector3 direction = soldier.transform.position - Camera.main.transform.position;
                Ray ray = new Ray(Camera.main.transform.position, direction);
                RaycastHit hit;

                // check if it his
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Ray hit object: " + hit.collider.gameObject.name);

                    // Create a new LineRenderer for this hit
                    GameObject lineRendererObject = new GameObject("LineRenderer");
                    LineRenderer lineRenderer = lineRendererObject.AddComponent<LineRenderer>();

                    lineRenderer.startWidth = 0.1f;
                    lineRenderer.endWidth = 0.1f;

                    // Draw a line from the camera to the hit point
                    lineRenderer.SetPosition(0, Camera.main.transform.position);
                    lineRenderer.SetPosition(1, hit.point);

                    lineRenderers[soldier] = lineRenderer;

                    Destroy(lineRenderer, 3f);
                }
            }
            else
            {
                // If the soldier's position is different from the stored pos, it moved
                if (soldier.transform.position != previousPositions[soldier])
                {
                    Debug.Log($"Soldier: {soldier} has moved!");

                    // Update the position
                    previousPositions[soldier] = soldier.transform.position;

                    // after waiting send out a ray from the camera to the position of the soldier
                    Vector3 direction = soldier.transform.position - Camera.main.transform.position;
                    Ray ray = new Ray(Camera.main.transform.position, direction);
                    RaycastHit hit;

                    // check if it his
                    if (Physics.Raycast(ray, out hit))
                    {
                        Debug.Log("Ray hit object: " + hit.collider.gameObject.name);

                        // Create a new LineRenderer for this hit
                        GameObject lineRendererObject = new GameObject("LineRenderer");
                        LineRenderer lineRenderer = lineRendererObject.AddComponent<LineRenderer>();

                        lineRenderer.startWidth = 0.1f;
                        lineRenderer.endWidth = 0.1f;

                        // Draw a line from the camera to the hit point
                        lineRenderer.SetPosition(0, Camera.main.transform.position);
                        lineRenderer.SetPosition(1, hit.point);

                        lineRenderers[soldier] = lineRenderer;

                        Destroy(lineRenderer, 1f);

                        // check the fog of war pos and remove vertices of it via the ray casts
                            // these rays will be sent from the position of the soldiers to the y pos of the plane
                            // once hit the plane, they release a raycast sphere to create holes in the plane
                    }
                }
            }
        }

        yield return null;
    }
        
    IEnumerator FindPositions()
    {
        while (foundSoldiers == soldiers.Count)
        {
            // if in general the count is equal to 0, break loop
            if (soldiers.Count == 0) yield break;

            foreach (var soldier in soldiers)
            {
                soldierPosition = soldier.transform.GetComponent<Transform>();
            }

            // return null to avoid freezing
            yield return null;
        }
    }

    private void UpdateSoldierList()
    {
        // Count the number of null items
        int nullSoldiers = soldiers.RemoveAll(soldier => soldier == null);
        
        // Decrease the foundSoldiers
        foundSoldiers -= nullSoldiers;

        // Remove any null items from the soldiers list
        soldiers.RemoveAll(soldier => soldier == null);

        // Remove any null items from the enemies list
        enemyUnits.RemoveAll(soldier => soldier == null);

        // find the game object that the soldiers are
        SoldierUnit[] soldierObjects = FindObjectsOfType<SoldierUnit>();

        // iterate over each obj
        foreach (var obj in soldierObjects)
        {
            SoldierUnit soldier = obj.GetComponent<SoldierUnit>();

            // return if true
            if (soldier == null)
                return;

            // Add the soldier accordingly
            switch (soldiers.Contains(soldier))
            {
                case false when obj.Type == Unit.TypeUnit.Human:
                    soldiers.Add(soldier);
                    foundSoldiers++;
                    break;
                case false when obj.Type == Unit.TypeUnit.Enemy && !enemyUnits.Contains(soldier):
                    enemyUnits.Add(soldier);
                    break;
            }
        }
    }
}
