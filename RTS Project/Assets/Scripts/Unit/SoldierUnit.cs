using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class SoldierUnit : Unit
{
    public int damageAmount;
    public BuildingBase buildingToAttack;
    private GameObject currentTarget;

    public bool isInRange;
    private bool isAttacking;

    public Unit enemyUnit;
    public Unit soldierUnit;
    public Unit enemyHp;
    public Unit soldierHp;
    public Unit unit;
    public Unit target = null;

    public NewSelectionManager selectionManager;
    public Worker worker;

    public float damageInterval = 2.0f;
    private float damageTimer;
    public float currentBuildingDist;
    private float distanceToEnemy;
    private float distanceToSoldier;

    private void Start()
    {
        unit = FindObjectOfType<Unit>();
        selectionManager = FindObjectOfType<NewSelectionManager>();
        print("selection manager : " + selectionManager);
        myAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //print("choosing building to Attack");
        if (buildingToAttack == null) return;
        ChooseBuildingToAttack();

        if (enemyUnit != null)
        {
            enemyUnit = selectionManager.GetEnemyToAttack();
        }

        if (soldierUnit != null)
        {
            soldierUnit = selectionManager.GetEnemyToAttack();
        }

        if (isInRange && enemyUnit != null)
        {
            if (typeUnit != enemyUnit.typeUnit)
            {
                if (soldierUnit.typeUnit == TypeUnit.Human)
                {
                    SoldierRange();
                }
            }
            if(typeUnit != soldierUnit.typeUnit)
            {
                if (enemyUnit.typeUnit == TypeUnit.Enemy)
                {
                    EnemyRange();
                }
            }
        }

        //enemy null check
        if (enemyUnit == null)
        {
            isInRange = false;
        }

        if (isInRange)
        {
            EnemyRange();
        }

        //soldier null check
        if (soldierUnit == null)
        {
            isInRange = false;
        }

        if (isInRange)
        {
            SoldierRange();
        }
    }

    //Enemy in range check
    public void EnemyRange()
    {
        Debug.Log("check enemy distance");

        distanceToEnemy = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (distanceToEnemy <= 1.5f)
        {
            Debug.Log("Attacking enemy");
            DealDamageToSoldiersInRange();
        }
    }

    public void SoldierRange()
    {
        worker = GetComponent<Worker>();
        if (worker == null)
        {
            Debug.Log("check soldier distance");
            distanceToSoldier = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (distanceToSoldier <= 1.5f)
            {
                Debug.Log("Attacking soldier");
                DealDamageToEnemiesInRange();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        target = other.GetComponent<Unit>();
        if (target != null)
        {
            if (target.typeUnit != typeUnit)
            {
                if (typeUnit == TypeUnit.Enemy)
                {
                    print(typeUnit);
                    soldierUnit = other.GetComponent<Unit>();
                    isInRange = true;
                    if (other.tag == "AI")
                    {
                        currentTarget = soldierUnit.gameObject;
                    }
                    print(soldierUnit);
                }

                if (typeUnit == TypeUnit.Human)
                {
                    print(typeUnit);
                    enemyUnit = other.GetComponent<Unit>();
                    isInRange = true;
                    if (other.tag == "AI")
                    {
                        currentTarget = enemyUnit.gameObject;
                    }
                    print(enemyUnit);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (typeUnit == TypeUnit.Enemy && currentTarget != null)
        {
            myAgent.SetDestination(currentTarget.transform.position);
            SoldierRange();
        }

        if (typeUnit == TypeUnit.Human && currentTarget != null)
        {
            myAgent.SetDestination(currentTarget.transform.position);
            EnemyRange();
        }
    }

    private void ChooseBuildingToAttack()
    {
        buildingToAttack = selectionManager.GetBuildingToAttack();

        print("found building base1");

        if (buildingToAttack != null)
        {
            print("found building base2");
            currentBuildingDist = Vector3.Distance(transform.position, buildingToAttack.transform.position);
            print("found building position");
            if (currentBuildingDist <= 5)
            {
                Debug.Log("Attacking");
                isAttacking = true;
                DealDamageToBuildings();
            }
        }
    }

    private void DealDamageToBuildings()
    {
        if (isAttacking)
        {
            Debug.Log("isAttacking is True");

            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                buildingToAttack.buildingHp -= damageAmount;
                print(buildingToAttack.buildingHp);
                damageTimer = 0.0f;
            }

            if (buildingToAttack.buildingHp <= 0)
            {
                Debug.Log("building Destroyed");
                Destroy(buildingToAttack.gameObject);
                isAttacking = false;
            }
        }
    }

    private void DealDamageToEnemiesInRange()
    {
        enemyHp = unit.GetComponent<Unit>();
        if (distanceToSoldier < 1.5f)
        {
            print(damageAmount);
            Debug.Log("Attacking Enemy");
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                enemyHp = unit.GetComponent<Unit>();
                if (enemyHp != null)
                {
                    enemyHp.UnitHealth -= damageAmount;
                    print(enemyHp.UnitHealth);
                    damageTimer = 0.0f;
                    if (enemyHp.UnitHealth <= 0)
                    {
                        isInRange = false;
                        print(isInRange);
                        Destroy(unit.gameObject);
                        Debug.Log("enemy hp is 0");
                        currentTarget = null;
                        target = null;
                    }
                }
            }
        }
    }

    private void DealDamageToSoldiersInRange()
    {
        soldierHp = unit.GetComponent<Unit>();
        if (distanceToEnemy < 1.5f)
        {
            print(damageAmount);
            Debug.Log("Attacking soldier");
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                soldierHp = unit.GetComponent<Unit>();
                if (soldierHp != null)
                {
                    soldierHp.UnitHealth -= damageAmount;
                    print(soldierHp.UnitHealth);
                    damageTimer = 0.0f;
                    if (soldierHp.UnitHealth <= 0)
                    {
                        isInRange = false;
                        print(isInRange);
                        Destroy(unit.gameObject);
                        Debug.Log("soldier hp is 0");
                        currentTarget = null;
                        target = null;
                    }
                }
            }
        }
    }
}