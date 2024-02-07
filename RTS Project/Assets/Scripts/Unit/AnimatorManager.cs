using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AnimatorManager : MonoBehaviour
{
    public Animator animator;

    
    private List<Unit> units = new();
    private Worker worker;
    private Soldier soldier;
    public BuildingBase jobType;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (animator != null)
        {
            

        }

        //GetUnits();
    }

    /*private void GetUnits()
    {
        Unit temp = FindObjectOfType<Unit>();

        if (units.Count == 0)
        {
            units.Add(temp);
        }
    }*/

    /*private void CurrentState()
    {
        switch (worker.currentState)
        {
            case Worker.State.Assigning:
                animator.SetTrigger("Idle");
                break;
            case Worker.State.Depositing:
                animator.SetTrigger("Idle");
                break;
            case Worker.State.Gathering:
                animator.SetTrigger("Idle");
                break;
            case Worker.State.Idling:
                animator.SetTrigger("Idle");
                break;
            case Worker.State.Moving:
                animator.SetTrigger("Idle");
                break;
        }

        switch (soldier.currentState)
        {
            case Soldier.States.Attacking:
                animator.SetTrigger("Idle");
                break;
            case Soldier.States.Idle:
                animator.SetTrigger("Idle");
                break;
        }
    }*/

    /*private void WorkerType()
    {
       
    }*/
}
