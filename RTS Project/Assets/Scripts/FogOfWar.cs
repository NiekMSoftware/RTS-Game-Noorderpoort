using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FogOfWar : MonoBehaviour
{
    public PlayerTestMovement RefPlayerMovement;
    public GameObject FogOfWarPlane;
    public Transform Player;
    public LayerMask FogLayer;
    public float radius = 5f;
    public float MaxAlpha = 1.0f;
    private float radiusSqr;

    private Mesh mesh;
    public Vector3[] vertices;
    public Color[] colors;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if(RefPlayerMovement.PlayerMoving)
        {
            DoRaycast();
        }
    }

    public void DoRaycast()
    {
        Ray r = new Ray(transform.position, Player.position - transform.position);
        RaycastHit hit;
        Debug.Log("RayCast hit");
        
        if(Physics.Raycast(r, out hit, 100, FogLayer,QueryTriggerInteraction.Collide))
        {          
            for(int i= 0;i<vertices.Length;i++)
            {
                print(hit.transform.gameObject);
                Vector3 v = FogOfWarPlane.transform.TransformPoint(vertices[i]);
                float dist = Vector3.SqrMagnitude(v - hit.point);
                if(dist < radiusSqr)
                {
                    float alpha = 0;
                    colors[i].a = alpha;
                }
                UpdateColor();
            }
        }

    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Player.position, radius);      
    }

    void Initialize()
    {
        mesh = FogOfWarPlane.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        radiusSqr = radius * radius;
        colors = new Color[vertices.Length];
        for(int i=0; i<colors.Length; i++)
        {
            colors[i] = Color.black;            
        }
        UpdateColor();
    }

    void UpdateColor()
    {
        Debug.Log("updating color");
        mesh.colors = colors;
    }
}
