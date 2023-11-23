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
        //pakt de positie van de Player.
        Ray r = new Ray(transform.position, Player.position - transform.position);
        RaycastHit hit;

        if (RefPlayerMovement.PlayerMoving)
        {
            //kijkt wanneer de RayCast met de FogLayer collides.
            if(Physics.Raycast(r, out hit, 100, FogLayer,QueryTriggerInteraction.Collide))
            {          
                for(int i= 0;i<vertices.Length;i++)
                {             
                    Vector3 v = FogOfWarPlane.transform.TransformPoint(vertices[i]);
                    float dist = Vector3.SqrMagnitude(v - hit.point);
                    if(dist < radiusSqr)
                    {
                        float alpha = MathF.Min(colors[i].a, dist/radiusSqr);
                        colors[i].a = alpha;
                        FogToDestroy = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
                    }
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
        for(int i=0; i<colors.Length; i++)
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
