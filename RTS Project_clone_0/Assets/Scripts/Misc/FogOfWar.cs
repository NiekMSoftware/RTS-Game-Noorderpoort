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
    public Transform Player;
    public LayerMask FogLayer;
    public float radius = 5f;
    public Vector3 FogToDestroy;
    private float radiusSqr { get { return radius * radius; } }

    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;

    private List<bool> visitedVertices;

    [Range(0, 1)]
    public float Transparency;


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
        Profiler.BeginSample("Player Moving");
        if(RefPlayerMovement.PlayerMoving)
        {
            DoRaycast();
        }
        Profiler.EndSample();
    }

    public void DoRaycast()
    {
        Profiler.BeginSample("Doing RayCast");

        Vector3 cameraPosition = Camera.main.transform.position;

        Ray camR = new Ray(cameraPosition, Player.position - cameraPosition);
        RaycastHit Camhit;
        

        if (RefPlayerMovement.PlayerMoving)
        {
            //kijkt wanneer de RayCast met de FogLayer collides.
            if(Physics.Raycast(camR, out Camhit, 100, FogLayer,QueryTriggerInteraction.Collide))
            {          
                for(int i= 0;i<vertices.Length;i++)
                {             
                    Vector3 v = FogOfWarPlane.transform.TransformPoint(vertices[i]);
                    float dist = Vector3.SqrMagnitude(v - Camhit.point);
                    if(dist < radiusSqr)
                    {
                        float alpha = MathF.Min(colors[i].a, dist/radiusSqr);
                        colors[i].a = alpha;
                        visitedVertices[i] = true;
                        FogToDestroy = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
                    }
                    //if vertices are outside of radius then make them half see through. 
                    else if (visitedVertices[i])
                    {
                        // If vertices are outside of the radius, make them half see-through.
                        //colors[i].a = 0.5f;
                        colors[i].a = Mathf.Lerp(colors[i].a, Transparency, Time.deltaTime);
                    }
                }
                UpdateColor();
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

        for (int i=0; i<colors.Length; i++)
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
