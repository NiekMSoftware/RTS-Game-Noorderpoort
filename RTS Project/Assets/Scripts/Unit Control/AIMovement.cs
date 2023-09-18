using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMovement : MonoBehaviour
{
    GameObject AI;
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
    } 
     

}
