using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class SoldierUnit : Unit
{
    //wat moet er nog gefixt worden?
    //- gebruik Unit.cs zijn health system in plaats van zijn eigen enemyHP;
    //soldiers vallen elkaar aan.
    public float damageInterval = 2.0f;
    public int damageAmount = 10;
    
    public Unit enemy;
    public GameObject SoldierGameObject;
    public BuildingBase buildingToAttack;
    

    private bool isAttacking = false;
    private float damageTimer = 0.0f;
    public Unit enemyHp;
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
        //FindClosestEnemy();
        print("choosing building to Attack");
        ChooseBuildingToAttack();
        enemy = selectionManager.GetEnemyToAttack();
        print(enemy);
        if (enemy == null) return;
        if (enemy == this) return;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Collideded");
        //SetEnemy(other.gameObject);

        //als soldier in de radius zit van de enemy
        //zet pak de gameobject uit de lijst als de enemy
        //val aan met DealDamageToEnemies()
        Unit enemyUnit = other.GetComponent<Unit>();
        myAgent.SetDestination(enemyUnit.transform.position);
        float DistanceToEnemy = Vector3.Distance(transform.position, enemyUnit.transform.position);
        print(DistanceToEnemy);
        if (enemyUnit != null && enemyUnit.typeUnit == TypeUnit.Enemy)
        {
            //float distanceToEnemy = Vector3.Distance(transform.position, enemyUnit.transform.position);
            //print(distanceToEnemy);
            if (DistanceToEnemy < 1.0f)
            {
                Debug.Log("Attacking enemy");
                DealDamageToEnemiesInRange();
            }
        }
    }

    //maak marker false zodat soldier naar enemy loopt.
    //zorg ervoor dat de Enemy GameObject gelijk is aan enemy zodat de soldier de enemy aan kan vallen. 
    //- trigger op de enemies doen
    //- ontriggerenter gebruiken
    /*private void SetEnemy(GameObject enemyObject)
    {
        Unit enemyUnit = enemyObject.GetComponent<Unit>();
        if(enemyUnit != null)
        {
            enemy = enemyUnit;
        }

        if (selectionManager.GetMarker() == true)
        {
            Debug.Log("marker true");
            myAgent.SetDestination(selectionManager.GetMarker().transform.position);
            selectionManager.GetMarker();
        }
        else
        {
            Debug.Log("Enemy in range");
            Debug.Log("moving to enemy myself");
            //closestDistance = distanceToEnemy;
            //enemy = potentialEnemy;
            //print(potentialEnemy);
            myAgent.SetDestination(enemy.transform.position);
            Debug.Log("setting destination");
            DealDamageToEnemiesInRange();
            Debug.Log("dealing damage to enemies in range");
        }
    }*/

    /*private void FindClosestEnemy()
    {
        //gamemanager maken:
        //- 2 list met alle units per team.
        //- unit moet zelf doorgeven wanneer hij in de lijst moet.
        //- uit de lijst de enemies halen en dan hieronder gebruiken.

        Unit[] enemies = GameObject.FindObjectsOfType<Unit>();
        float closestDistance = 5;
        enemy = null;

        foreach (Unit potentialEnemy in enemies )
        {
            if (potentialEnemy != this)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, potentialEnemy.transform.position);

                if (distanceToEnemy < closestDistance)
                {
                    if (selectionManager.GetMarker() == true)
                    {
                        myAgent.SetDestination(selectionManager.GetMarker().transform.position);
                        selectionManager.GetMarker();
                    }
                    else
                    {
                        Debug.Log("Enemy in range");
                        Debug.Log("moving to enemy myself");
                        closestDistance = distanceToEnemy;
                        enemy = potentialEnemy;
                        print(potentialEnemy);
                        myAgent.SetDestination(enemy.transform.position);
                        DealDamageToEnemiesInRange();
                    }
                }
            }
        }
    }*/

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
        enemyHp = enemy.GetComponent<Unit>();
        print(enemyHp);
        if (enemy != null && Vector3.Distance(SoldierGameObject.transform.position, enemy.transform.position) < 1)
        {
            print(Vector3.Distance(SoldierGameObject.transform.position, enemy.transform.position));
            Debug.Log("Attacking Enemy");
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                enemyHp = enemy.GetComponent<Unit>();
                if (enemyHp != null)
                {
                    enemyHp.UnitHealth -= damageAmount;
                    print(enemyHp.UnitHealth);
                    damageTimer = 0.0f;
                    if (enemyHp.UnitHealth <= 0)
                    {
                        Debug.Log("destroying enemy");
                        Destroy(enemy.gameObject);
                        isAttacking = false;
                    }
                }
            }
            //if (selectionmanager.selectedEnemy == null)
            //{
            //    selectionmanager.selectedEnemy = placeHolder;
            //}
        }
        /*Collider[] colliders = Physics.OverlapBox(
            transform.position, Vector3.one, Quaternion.identity, Enemy);

        foreach (var collider in colliders)
        {
            Debug.Log("Colliding with enemy.");
            print(collider);
            if (collider.gameObject != gameObject)
            {
                SoldierUnit Enemy = collider.GetComponent<SoldierUnit>();
                if (Enemy != null)
                {
                    print(damageAmount);
                    Enemy.TakeDamage(damageAmount);
                }
            }
        }*/
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
