using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Burst.CompilerServices;
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
    private Dictionary<int, List<SoldierUnit>> vertexSoldiers = new();

    // private int to keep track of the soldiers
    private int foundSoldiers = 0;

    [Space] 
    [SerializeField] private float elapsed;
    [SerializeField] private float maxUntilNext;

    [Header("Fog of War Properties")]
    [SerializeField] private float fogHeight;
    [SerializeField] private Material fogMaterial;

    [Space] 
    [SerializeField] private GameObject parentObject;
    [SerializeField] private int resolution = 0;
    [SerializeField] private float resolutionFactor = 0;
    [SerializeField] private int sectionSize = 100;
    public List<GameObject> fogObjects = new();

    private Transform soldierPosition;

    [Header("Fog Detection")] 
    [SerializeField] 
    [Range(1f, 20f)] private float radius;

    [Header("DEBUG PROPERTIES")] 
    [SerializeField] private GameObject parentLines;
    private Vector3 lastHitPoint;
    private float lastRadius;

    void Start()
    {
        // Get the terrain's size and position
        Terrain terrain = Terrain.activeTerrain;
        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 terrainPos = terrain.transform.position;

        // Calculate the number of sections in each dimension
        int sectionsX = (int)Mathf.Ceil(terrainSize.x / sectionSize);
        int sectionsZ = (int)Mathf.Ceil(terrainSize.z / sectionSize);

        // Create a fog plane for each section
        for (int i = 0; i < sectionsX; i++)
        {
            for (int j = 0; j < sectionsZ; j++)
            {
                // Calculate the position and size of this section
                Vector3 sectionPos = terrainPos + new Vector3(i * sectionSize, 0, j * sectionSize);
                Vector3 sectionsSize = new Vector3(
                    Mathf.Min(this.sectionSize, terrainSize.x - i * this.sectionSize), terrainSize.y, Mathf.Min(this.sectionSize, terrainSize.z - j * this.sectionSize));

                // Create the fog plane for this section
                CreateFogPlane(sectionPos, sectionsSize, resolution);
            }
        }

        StartCoroutine(InitializeFogObjects());
    }

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

    private void UpdateFog(SoldierUnit soldier, Vector3 oldPos, Vector3 newPos)
    {
        foreach (var fogObject in fogObjects)
        {
            // Get the fog object's MeshFilter
            MeshFilter meshFilter = fogObject.GetComponent<MeshFilter>();

            // Get the fog mesh
            Mesh mesh = meshFilter.mesh;

            // Get the current colors of the fog mesh
            Color[] colors = mesh.colors;

            // Calculate the maximum distance a vertex can be from the soldier to be affected
            float maxDistance = radius * radius;

            // Iterate over each vertex
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                Vector3 worldPos = fogObject.transform.TransformPoint(mesh.vertices[i]);

                // Calculate the squared distance from the old and new soldier positions to this vertex
                float oldDistance = (worldPos - oldPos).sqrMagnitude;
                float newDistance = (worldPos - newPos).sqrMagnitude;

                // Check if this vertex was or is within the soldier's radius
                if (oldDistance <= maxDistance || newDistance <= maxDistance)
                {
                    // Update the list of soldiers affecting this vertex
                    if (!vertexSoldiers.ContainsKey(i))
                    {
                        vertexSoldiers[i] = new List<SoldierUnit>();
                    }

                    if (oldDistance <= maxDistance)
                    {
                        vertexSoldiers[i].Remove(soldier);
                    }

                    if (newDistance <= maxDistance)
                    {
                        vertexSoldiers[i].Add(soldier);
                    }

                    // Update the color of this vertex based on whether any soldiers are affecting it
                    colors[i] = vertexSoldiers[i].Count > 0 ? Color.clear : Color.red;
                }
            }

            // Update the colors of the fog mesh
            mesh.colors = colors;
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
                if (previousPositions.ContainsKey(soldier))
                {
                    UpdateFog(soldier, previousPositions[soldier], soldier.transform.position);
                }
                else
                {
                    previousPositions[soldier] = soldier.transform.position;
                }

                // after waiting send out a ray from the camera to the position of the soldier
                Vector3 direction = soldier.transform.position - Camera.main.transform.position;
                Ray ray = new Ray(Camera.main.transform.position, direction);
                RaycastHit hit;

                // check if it his
                if (Physics.Raycast(ray, out hit))
                {
                    // Create a new LineRenderer for this hit
                    GameObject lineRendererObject = new GameObject("LineRenderer");

                    // set parent obj
                    lineRendererObject.transform.SetParent(parentLines.transform);

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
                    // Update the position
                    previousPositions[soldier] = soldier.transform.position;

                    // after waiting send out a ray from the camera to the position of the soldier
                    Vector3 direction = soldier.transform.position - Camera.main.transform.position;
                    Ray ray = new Ray(Camera.main.transform.position, direction);
                    RaycastHit hit;

                    // check if it his
                    if (Physics.Raycast(ray, out hit))
                    {
                        // Create a new LineRenderer for this hit
                        GameObject lineRendererObject = new GameObject("LineRenderer");

                        // set parent
                        lineRendererObject.transform.SetParent(parentLines.transform);

                        LineRenderer lineRenderer = lineRendererObject.AddComponent<LineRenderer>();

                        lineRenderer.startWidth = 0.1f;
                        lineRenderer.endWidth = 0.1f;

                        // Draw a line from the camera to the hit point
                        lineRenderer.SetPosition(0, Camera.main.transform.position);
                        lineRenderer.SetPosition(1, hit.point);

                        lineRenderers[soldier] = lineRenderer;

                        Destroy(lineRenderer, 1f);
                    }
                }
            }
        }

        yield return null;
    }

    #region IEnumerators

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

<<<<<<< HEAD
    IEnumerator AddMeshCollider(GameObject fogObject)
    {
        // Wait for the next frame
        yield return null;

        // Now add the MeshCollider
        MeshCollider fogMeshCollider = fogObject.AddComponent<MeshCollider>();
        fogMeshCollider.convex = true;
    }

    IEnumerator InitializeFogObjects()
    {
        yield return new WaitForEndOfFrame();

        var allObjects = FindObjectsOfType<GameObject>();

        int fogOfWarLayer = LayerMask.NameToLayer("FogOfWar");

        foreach (var obj in allObjects)
        {
            if (obj.layer == fogOfWarLayer)
            {
                fogObjects.Add(obj);
=======
                int layerMask = (13 << LayerMask.NameToLayer("FogOfWar")) | (6 << LayerMask.NameToLayer("Clickable"));

                // invert the bit corresponding clickable units
                layerMask = layerMask & ~(6 << LayerMask.NameToLayer("Clickable"));

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 100f);

                    // check if it hit the fog
                    if (hit.collider.transform.IsChildOf(parentObject.transform))
                    {
                        Debug.Log("Ray hit the fog");
                    }
                    else
                    {
                        Debug.LogError("Ray didn't hit child");
                    }
                }
                else
                {
                    Debug.LogError("Ray didn't hit the fog (Oh no...)");

                    Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red, 100f);
                }
>>>>>>> parent of a194326 (Ray collision working?! FINALLY)
            }
        }
    }

    #endregion


    private void CreateFogPlane(Vector3 sectionPos, Vector3 sectionSize, int resolution)
    {
        // Adjust the section size to overlap with neighboring sections
        sectionSize += new Vector3(1, 0, 1);

        // Create a new gameobject for the fog plane
        GameObject fogObject = new GameObject("FogPlane");

        fogObject.transform.SetParent(parentObject.transform);
        fogObject.AddComponent<MeshFilter>();
        fogObject.AddComponent<MeshRenderer>();
        fogObject.AddComponent<MeshCollider>();
        fogObject.layer = LayerMask.NameToLayer("FogOfWar");

        // Create a new mesh for the fog plane
        Mesh fogMesh = new Mesh();

        // Calculate the resolution based on the size of the section
        resolution = (int)(sectionSize.x * sectionSize.z * resolutionFactor);

        // Create a grid of vertices
        Vector3[] vertices = new Vector3[resolution * resolution];

        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                // Calculate the position of this vertex
                float xPos = sectionPos.x + (x / (float)resolution) * sectionSize.x;
                float zPos = sectionPos.z + (z / (float)resolution) * sectionSize.z;

                // Use a raycast to find the height of the terrain at this point
                RaycastHit hit;
                int layerMask = 1 << LayerMask.NameToLayer("Ground"); // only interact with ground layer
                if (Physics.Raycast(new Vector3(xPos, sectionPos.y + sectionSize.y, zPos), Vector3.down, out hit, Mathf.Infinity, layerMask))
                {
                    // Adjust the height of the vertex to be above the terrain
                    vertices[z * resolution + x] = hit.point + Vector3.up * fogHeight;
                }
            }
        }

        // Set the vertices of the fog mesh
        fogMesh.vertices = vertices;

        // Create triangles to connect the vertices
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        for (int z = 0; z < resolution - 1; z++)
        {
            for (int x = 0; x < resolution - 1; x++)
            {
                // Calculate the index of this quad
                int quadIndex = z * (resolution - 1) + x;
                int triangleIndex = quadIndex * 6;

                // Create two triangles to form a quad
                triangles[triangleIndex + 0] = z * resolution + x;
                triangles[triangleIndex + 1] = (z + 1) * resolution + x;
                triangles[triangleIndex + 2] = z * resolution + x + 1;
                triangles[triangleIndex + 3] = z * resolution + x + 1;
                triangles[triangleIndex + 4] = (z + 1) * resolution + x;
                triangles[triangleIndex + 5] = (z + 1) * resolution + x + 1;
            }
        }

        // Set the triangles of the fog mesh
        fogMesh.triangles = triangles;

        // Assign the fog mesh to a MeshFilter component
        MeshFilter meshFilter = fogObject.GetComponent<MeshFilter>();
        meshFilter.mesh = fogMesh;

        // Apply a material to the MeshRenderer
        MeshRenderer meshRenderer = fogObject.GetComponent<MeshRenderer>();
        meshRenderer.material = fogMaterial;
    }

<<<<<<< HEAD
=======
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

>>>>>>> parent of a194326 (Ray collision working?! FINALLY)
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(lastHitPoint, radius);
    }
}
