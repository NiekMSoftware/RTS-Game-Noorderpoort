using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;

[RequireComponent(typeof(DetectEnemies))]
[RequireComponent (typeof(NavMeshAgent))]
public class Soldier : Unit
{
    [SerializeField] private bool autoAttackTargets;
    [SerializeField] private float acceptDistance = 3f;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float turnInterval;
    [SerializeField] private float turnSpeed;

    private DetectEnemies detectEnemies;

    [SerializeField] private States currentState;

    [SerializeField] private GameObject target;

    private float attackTimer;
    private float turnTimer;

    private float amountTurned;

    private enum States
    {
        Idle,
        Patrolling,
        Attacking,
    }

    private void Awake()
    {
        detectEnemies = GetComponent<DetectEnemies>();
    }

    protected override void Start()
    {
        base.Start();

        attackTimer = attackSpeed;
    }

    protected override void Update()
    {
        base.Update();

        HandleStates();

        HandleAction();

        CheckMove();
    }

    private void CheckMove()
    {
        if (target == null) return;
        if (myAgent == null) return;

        if (Vector3.Distance(target.transform.position, transform.position) > acceptDistance)
            myAgent.SetDestination(target.transform.position);
        else
            myAgent.SetDestination(transform.position);
    }

    private void HandleAction()
    {
        switch (currentState)
        {
            case States.Idle:
                turnTimer -= Time.deltaTime;

                if (turnTimer <= 0)
                {
                    if (amountTurned >= 360)
                    {
                        amountTurned = 0;
                        turnTimer = turnInterval;
                    }

                    amountTurned += Time.deltaTime * turnSpeed;
                    transform.Rotate(Time.deltaTime * turnSpeed * Vector3.up);
                }
                break;

            case States.Attacking:
                attackTimer -= Time.deltaTime;

                if (attackTimer <= 0)
                {
                    attackTimer = attackSpeed;
                    DealDamage();
                }
                break;
        }
    }

    private void DealDamage()
    {
        if (target.TryGetComponent(out BuildingBase building))
        {
            building.buildingHp -= unitDamage;
        }
        else if (target.TryGetComponent(out Unit unit))
        {
            unit.UnitHealth -= unitDamage;
        }
    }

    private void HandleStates()
    {
        currentState = States.Idle;

        if (detectEnemies.visibleTargets.Count > 0)
        {
            //In future, return closest target

            List<GameObject> possibleTargets = detectEnemies.visibleTargets;

            GameObject finalTarget = null;

            foreach (GameObject target in possibleTargets)
            {
                if (target == null) continue;
                if (target == gameObject) continue;

                if (target.TryGetComponent(out BuildingBase building))
                {
                    if (building.GetOccupancyType() == BuildingBase.OccupancyType.Player && typeUnit == TypeUnit.Human)
                        continue;

                    if (building.GetOccupancyType() == BuildingBase.OccupancyType.Enemy && typeUnit == TypeUnit.Enemy)
                        continue;
                }
                else if (target.TryGetComponent(out Unit unit))
                {
                    if (unit.typeUnit == typeUnit) continue;
                }

                finalTarget = target;
            }

            target = finalTarget;
        }

        if (target == null) return;

        if (Vector3.Distance(target.transform.position, transform.position) <= acceptDistance)
        {
            currentState = States.Attacking;
        }
    }
}
