using UnityEngine;
using UnityEngine.AI;

public class SoldierUnit : Unit
{
    public int damageAmount = 10;
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

    public float damageInterval = 2.0f;
    private float damageTimer;
    public float currentBuildingDist;
    private float distanceToEnemy;
    private float distanceToSoldier;

    public DetectEnemies detectEnemies;

    private void Start()
    {
        unit = FindObjectOfType<Unit>();
        selectionManager = FindObjectOfType<NewSelectionManager>();
        print("selection manager : " + selectionManager);
        myAgent = GetComponent<NavMeshAgent>();
        detectEnemies = GetComponent<DetectEnemies>();
    }

    private void Update()
    {
        if (buildingToAttack == null) return;
        ChooseBuildingToAttack();

        if (detectEnemies.visibleTargets.Count > 0)
        {
            enemyUnit = detectEnemies.visibleTargets[0].GetComponent<Unit>();
            soldierUnit = detectEnemies.visibleTargets[0].GetComponent<SoldierUnit>();
        }

        if (isInRange && enemyUnit != null)
        {
            if (typeUnit != enemyUnit.typeUnit || typeUnit != soldierUnit.typeUnit)
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
        }

        if (enemyUnit == null)
        {
            isInRange = false;
        }

        if (isInRange)
        {
            EnemyRange();
        }

        if (soldierUnit == null)
        {
            isInRange = false;
        }

        /* //print("choosing building to Attack");
         if (buildingToAttack == null) return;
         ChooseBuildingToAttack();

         if(enemyUnit != null)
         {
             enemyUnit = selectionManager.GetEnemyToAttack();
         }

         if (soldierUnit != null)
         {
             soldierUnit = selectionManager.GetEnemyToAttack();
         }

         if (isInRange && enemyUnit != null)
         {
             if (typeUnit != enemyUnit.typeUnit || typeUnit != soldierUnit.typeUnit)
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
         }*/
    }

    public void CheckRangeAndAttack(Unit targetUnit)
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetUnit.transform.position);

        if(distanceToTarget <= 1.5f)
        {
            Debug.Log("ataacking");
            DoDamageToTargetInRange(targetUnit);
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

    private void DoDamageToTargetInRange(Unit targetUnit)
    {
        Unit targetHP = targetUnit.GetComponent<Unit>();
        float distanceToTarget = Vector3.Distance(transform.position, targetUnit.transform.position);

        if (distanceToTarget < 1.5f)
        {
            Debug.Log("attacking target");
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                if (targetHP != null)
                {
                    targetHP.UnitHealth -= damageAmount;
                    Debug.Log(targetUnit.UnitHealth);

                    damageTimer = 0;

                    if (targetHP.UnitHealth <= 0)
                    {
                        isInRange = false;
                        Destroy(targetUnit.gameObject);
                        Debug.Log("target died");
                        currentTarget = null;
                        target = null;
                    }
                }
            }
        }
    }

    //Enemy in range check
    /*public void EnemyRange()
    {
        Debug.Log("check enemy distance");

        distanceToEnemy = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (distanceToEnemy <= 1.5f)
        {
            Debug.Log("Attacking enemy");
            //DealDamageToEnemiesInRange();
        }

    }

    public void SoldierRange()
    {
        Debug.Log("check soldier distance");
        distanceToSoldier = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (distanceToSoldier <= 1.5f)
        {
            Debug.Log("Attacking soldier");
            //DealDamageToSoldiersInRange();
        }
    }*/

    /*private void OnTriggerEnter(Collider other)
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
        if (typeUnit == TypeUnit.Enemy && currentTarget!=null)
        {
            myAgent.SetDestination(currentTarget.transform.position);
            SoldierRange();
        }

        if (typeUnit == TypeUnit.Human && currentTarget != null)
        {
            myAgent.SetDestination(currentTarget.transform.position);
            EnemyRange();
        }
    }*/

    /*private void DealDamageToEnemiesInRange()
    {
        enemyHp = unit.GetComponent<Unit>();
        if (distanceToSoldier < 1.5f)
        {
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
    }*/
}
