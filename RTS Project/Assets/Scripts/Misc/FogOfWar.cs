using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;
public class FogOfWar : MonoBehaviour
{
    // als de player al een stuk heeft weggehaald moet je het niet mee tellen.
    //alles buiten de camera hoeft niet meegetelt te worden tijdens de updates.
    public PlayerTestMovement RefPlayerMovement;
    public GameObject FogOfWarPlane;
    public List<SoldierUnit> soldierUnits;
    public Transform Player;
    public LayerMask FogLayer;
    public float radius = 5f;
    public Vector3 FogToDestroy;
    private float radiusSqr { get { return radius * radius; } }
    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;
    private List<bool> visitedVertices;
    
    [Range(0.5f, 1f)]
    public float Transparency;

    private Coroutine rayCastRoutine;

    private Vector3 prevPlayerPos;
    private float distanceTravelled;
    
    [Space]
    [Range(0.5f, 1.0f)]
    public float raycastDistThreshold = 1f;
    public float timeUntilDisperse;

    // Start is called before the first frame update
    void Start()
    {
        Profiler.BeginSample("Initializing");
        Initialize();
        Profiler.EndSample();
    }

    // Update is called once per frame
    void Update()
    {
        // Add soldier units to list once found
        //SoldierUnit soldierObject = FindObjectOfType<SoldierUnit>();
        //if (soldierObject == null)
        //{
        //    Debug.LogError("No Soldier Found");
        //}
        //else
        //{
        //    // Add them to the list
        //    soldierUnits.Add(soldierObject);
        //}

        // TODO: Gather each soldier and make them move instead to see the Fog of War changing

        Profiler.BeginSample("Player Moving");
        if (RefPlayerMovement.PlayerMoving)
        {
            if (rayCastRoutine == null)
            {
                rayCastRoutine = StartCoroutine(DoRayCast());
            }
        }
        else
        {
            // Stop de routine als deze loop
            if (rayCastRoutine != null)
            {
                StopCoroutine(rayCastRoutine);
                rayCastRoutine = null;
            }

        }

        Profiler.EndSample();
    }

    // Enumerator om raycasts te sturen
    public IEnumerator DoRayCast()
    {
        while (true)
        {
            // Bereken de afstand die de speler heeft afgelegd
            distanceTravelled += Vector3.Distance(Player.position, prevPlayerPos);
            prevPlayerPos = Player.position;

            // voer de raycasts uit alleen wanneer de speler een bepaald aantal afstand heeft gelegd
            if (distanceTravelled >= raycastDistThreshold)
            {
                PerformRayCast();
                distanceTravelled = 0f;
            }

            yield return null;
        }
    }

    private void PerformRayCast()
    {
        Profiler.BeginSample("Started Performing RayCasts");
        Vector3 cameraPosition = Camera.main.transform.position;
        Transform fogTransform = FogOfWarPlane.transform;

        Ray camR = new Ray(cameraPosition, Player.position - cameraPosition);
        RaycastHit Camhit;

        bool colorsChanged = false;

        Profiler.BeginSample("Player Moving");
        if (RefPlayerMovement.PlayerMoving)
        {
            if (Physics.Raycast(camR, out Camhit, 100, FogLayer, QueryTriggerInteraction.Collide))
            {
                Profiler.BeginSample("RayCasting");
                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 v = fogTransform.TransformPoint(vertices[i]);
                    float dist = Vector3.SqrMagnitude(v - Camhit.point);
                    if (dist < radiusSqr)
                    {
                        float alpha = MathF.Min(colors[i].a, dist / radiusSqr);
                        if (colors[i].a != alpha)
                        {
                            colors[i].a = alpha;
                            colorsChanged = true;
                        }
                        visitedVertices[i] = true;
                        FogToDestroy = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
                    }
                    else if (visitedVertices[i]) // return fog to transparency
                    {
                        float newAlpha = Mathf.Lerp(colors[i].a, Transparency, (timeUntilDisperse * Time.deltaTime));
                        if (colors[i].a != newAlpha)
                        {
                            colors[i].a = newAlpha;
                            colorsChanged = true;
                        }
                    }
                }
                Profiler.EndSample();
                if (colorsChanged)
                {
                    UpdateColor();
                }
            }
        }

        Profiler.EndSample();
    }

    void Initialize()
    {
        //maakt mesh aan.
        mesh = FogOfWarPlane.GetComponent<MeshFilter>().mesh;
        //kijkt hoeveel vertices er zijn.
        vertices = mesh.vertices;
        //radiusSqr = radius * radius;
        colors = new Color[vertices.Length];
        visitedVertices = new List<bool>(new bool[vertices.Length]);
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.black;
        }
        UpdateColor();
    }

    void UpdateColor()
    {
        Profiler.BeginSample("Updating color");
        mesh.colors = colors;
        Profiler.EndSample();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Player.position, radius);
    }
}
