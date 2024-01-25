using Mono.Cecil.Cil;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(DetectEnemies))]
[RequireComponent (typeof(NavMeshAgent))]
public class Soldier : Unit
{
    [SerializeField] private float unitAcceptDistance = 3f;
    [SerializeField] private float buildingAcceptDistance = 10f;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float turnInterval;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float cantDetectTargetsTime = 5f;

    private DetectEnemies detectEnemies;

    [SerializeField] private States currentState;

    [SerializeField] private GameObject target;
    [SerializeField] private BuildingBase secTarget;

    private float attackTimer;
    private float turnTimer;

    private float amountTurned;

    private bool canDetectTargets = true;
    private float cantDetectTargetsTimer;

    private float currentAcceptDistance = 0f;

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
        cantDetectTargetsTimer = cantDetectTargetsTime;
    }

    protected override void Update()
    {
        base.Update();

        CalculateCurrentAcceptDistance();

        UpdateCantDetectTargetsTimer();

        HandleStates();

        HandleAction();

        CheckMove();
    }

    private void UpdateCantDetectTargetsTimer()
    {
        if (canDetectTargets) return;

        if (cantDetectTargetsTimer > 0)
        {
            cantDetectTargetsTimer -= Time.deltaTime;
        }
        else
        {
            cantDetectTargetsTimer = cantDetectTargetsTime;
            canDetectTargets = true;
        }
    }

    private void CalculateCurrentAcceptDistance()
    {
        if (!target) return;

        float acceptDistance = 0f;

        if (target.TryGetComponent(out BuildingBase _))
            acceptDistance = buildingAcceptDistance;
        else if (target.TryGetComponent(out Unit _))
            acceptDistance = unitAcceptDistance;

        currentAcceptDistance = acceptDistance;
    }

    private void CheckMove()
    {
        if (target == null) return;
        if (myAgent == null) return;

        if (Vector3.Distance(target.transform.position, transform.position) > currentAcceptDistance)
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

                string targetName = "target";

                if (target.TryGetComponent(out BuildingBase building))
                    targetName = building.buildingName;
                else if (target.TryGetComponent(out Unit unit))
                    targetName = unit.UnitName;

                SetCurrentAction("Attacking " + targetName);

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

    public void SelectedBuilding(BuildingBase building)
    {
        if (building.GetOccupancyType() == BuildingBase.OccupancyType.Player && typeUnit == TypeUnit.Human)
            return;

        if (building.GetOccupancyType() == BuildingBase.OccupancyType.Enemy && typeUnit == TypeUnit.Enemy)
            return;

        if (target)
        {
            secTarget = building;
        }
        else
        {
            target = building.gameObject;
        }
    }

    public void ResetTargets()
    {
        target = null;
        secTarget = null;
        currentState = States.Idle;
        canDetectTargets = false;
    }

    private void HandleStates()
    {
        currentState = States.Idle;

        if (detectEnemies.visibleTargets.Count > 0 && canDetectTargets)
        {
            //In future, return closest target

            List<GameObject> possibleTargets = detectEnemies.visibleTargets;

            GameObject finalTarget = null;

            foreach (GameObject target in possibleTargets)
            {
                if (target == null) continue;
                if (target == gameObject) continue;

                if (target.TryGetComponent(out Unit unit))
                {
                    if (unit.typeUnit == typeUnit) continue;
                }

                finalTarget = target;
            }

            //make return if possible
            if (target)
            {
                if (target.TryGetComponent(out BuildingBase building))
                    secTarget = building;
            }

            target = finalTarget;
        }

        if (!target)
        {
            if (secTarget)
            {
                target = secTarget.gameObject;
                secTarget = null;
            }
            else
                return;
        }

        if (Vector3.Distance(target.transform.position, transform.position) <= currentAcceptDistance)
        {
            currentState = States.Attacking;
        }
    }
}
