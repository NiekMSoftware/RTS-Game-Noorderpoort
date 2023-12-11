using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class SoldierUnit : Unit
{
    public float damageInterval = 2.0f;
    public int damageAmount = 10;
    public bool isInRange = false;
    public Unit enemy;
    public Unit enemyUnit;
    public Unit soldier;
    public Unit soldierUnit;
    public GameObject soldierGameObject;
    public GameObject enemyGameObject;
    public BuildingBase buildingToAttack;
    

    private bool isAttacking = false;
    private float damageTimer = 0.0f;
    public Unit enemyHp;
    public Unit soldierHp;
    public NewSelectionManager selectionManager;
    public Unit unit;
    public float currentBuildingDist;

    private void Start()
    {
        //base.Start();
        unit = FindObjectOfType<Unit>();
        selectionManager = FindObjectOfType<NewSelectionManager>();
        print("selection manager : " + selectionManager);
        myAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        print("choosing building to Attack");
        ChooseBuildingToAttack();
        enemy = selectionManager.GetEnemyToAttack();
        soldier = selectionManager.GetEnemyToAttack();

        if (isInRange && enemy != null)
        {
            if (enemyUnit.typeUnit == TypeUnit.Enemy)
            {
                EnemyRange();
            }

            if (soldierUnit.typeUnit == TypeUnit.Human)
            {
                SoldierRange();
            }
        }

        

        //enemy null check
        if (enemyUnit == null)
        {
            isInRange = false;
        }

        if (isInRange == true)
        {
            EnemyRange();
        }

        if (enemy == null) return;
        if (enemyUnit == null) return;
        if (enemy == this) return;

        //soldier null check
        if (soldierUnit == null)
        {
            isInRange = false;
        }

        if (isInRange == true)
        {
            SoldierRange();
        }

        if(soldier == null) return;
        if (soldierUnit == null) return;
        if (soldier == this) return;
    }

    //Enemy in range check
    public void EnemyRange()
    {
        Debug.Log("check distance");
        float distanceToEnemy = Vector3.Distance(transform.position, enemyUnit.transform.position);
        if (enemyUnit != null && enemyUnit.typeUnit == TypeUnit.Enemy)
        {
            if (distanceToEnemy < 1.5f)
            {
                Debug.Log("Attacking enemy");
                DealDamageToEnemiesInRange();
            }
        }
    }

    public void SoldierRange()
    {
        float distanceToSoldier = Vector3.Distance(enemyUnit.transform.position, soldierUnit.transform.position);
        if (soldierUnit != null && soldierUnit.typeUnit == TypeUnit.Human)
        {
            if (distanceToSoldier < 1.5f)
            {
                Debug.Log("Attacking soldier");
                DealDamageToSoldiersInRange();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Collided");
        if (typeUnit == TypeUnit.Human)
        {
            print(typeUnit);
            enemyUnit = other.GetComponent<Unit>();
            isInRange = true;
            myAgent.SetDestination(enemyUnit.transform.position);
        }

        if (typeUnit == TypeUnit.Enemy)
        {
            print(typeUnit);
            soldierUnit = other.GetComponent<Unit>();
            isInRange = true;
            myAgent.SetDestination(soldierUnit.transform.position);
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

    //damage to buildings.
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

    //damage to enemies
    private void DealDamageToEnemiesInRange()
    {
        Debug.Log("entering dealingdamagetoenemies");
        enemyHp = enemyUnit.GetComponent<Unit>();
        print(enemyHp);
        if (enemyUnit != null && Vector3.Distance(transform.position, enemyUnit.transform.position) < 1.5f)
        {
            //print(Vector3.Distance(SoldierGameObject.transform.position, enemyUnit.transform.position));
            Debug.Log("Attacking Enemy");
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                enemyHp = enemyUnit.GetComponent<Unit>();
                if (enemyHp != null)
                {
                    enemyHp.UnitHealth -= damageAmount;
                    print(enemyHp.UnitHealth);
                    damageTimer = 0.0f;
                    if (enemyHp.UnitHealth <= 0)
                    {
                        isInRange = false;
                        print(isInRange);
                        Destroy(enemyUnit.gameObject);
                        Debug.Log("enemy hp is 0");
                        enemyUnit = null;
                    }
                }
            }
        }
    }

    private void DealDamageToSoldiersInRange()
    {
        soldierHp = soldierUnit.GetComponent<Unit>();
        print(soldierHp);
        if (soldierUnit != null && Vector3.Distance(transform.position, soldierUnit.transform.position) < 1.5f)
        {
            //print(Vector3.Distance(SoldierGameObject.transform.position, enemyUnit.transform.position));
            Debug.Log("Attacking soldier");
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                soldierHp = soldierUnit.GetComponent<Unit>();
                if (soldierHp != null)
                {
                    soldierHp.UnitHealth -= damageAmount;
                    print(soldierHp.UnitHealth);
                    damageTimer = 0.0f;
                    if (soldierHp.UnitHealth <= 0)
                    {
                        isInRange = false;
                        print(isInRange);
                        Destroy(soldierUnit.gameObject);
                        Debug.Log("soldier hp is 0");
                        soldierUnit = null;
                    }
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        unitHealth -= Mathf.FloorToInt(damage);
        if (unitHealth < 0)
        {
            Death();
        }
    }

    protected override void Death()
    {
        if (unitHealth <= 0)
        {
            // Play death animation.
        }
    }
}
