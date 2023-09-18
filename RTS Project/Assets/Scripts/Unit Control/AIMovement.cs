using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    Camera myCamera;
    NavMeshAgent myAgent;
    public LayerMask ground;

    void Start()
    {
        myCamera = Camera.main;
        myAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
       if(Input.GetMouseButtonDown(1)) 
       {
            RaycastHit hit;
            Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground)) 
            {
                myAgent.SetDestination(hit.point);
            }
       }     
    }


    /* GameObject AI;
     [SerializeField]
     private Transform Target;

     //AI movement
     void Awake()
     {
         //zoekt target object.
         Target = GameObject.Find("Target").transform;
         //Zoekt GameObject met AI tag.
         AI = GameObject.FindGameObjectWithTag("AI");
     }

     void Update()
     {
         //Maakt AI bewegen naar target.
         GetComponent<NavMeshAgent>().destination = Target.transform.position;
     } */


}
