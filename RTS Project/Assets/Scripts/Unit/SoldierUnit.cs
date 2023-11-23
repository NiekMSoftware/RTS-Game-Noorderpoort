using System.Collections.Generic;
using UnityEngine;

public class SoldierUnit : Unit
{
    //wat moet er nog gefixt worden?
    //- selectedenemy check maken voor als enemy destroyed wordt.
    public float damageInterval = 2.0f;
    public int damageAmount = 10;
    public LayerMask Building;
    public LayerMask Enemy;
    public List<GameObject> Enemies = new List<GameObject>();
    public List<BuildingBase> Buildings = new List<BuildingBase>();
    public GameObject EnemyGameObject;
    public GameObject SoldierGameObject;
    public GameObject buildingGameObject;

    private bool isAttacking = false;
    private float damageTimer = 0.0f;
    public BuildingBase buildingBase;
    public Unit enemyHp;
    public SelectionManager selectionmanager;
    public float currentBuildingDist;

    private void Start()
    {
        selectionmanager = FindObjectOfType<SelectionManager>();
    }

    private void Update()
    {
        SpawnBuildings();
        EnemyGameObject = selectionmanager.selectedEnemy;
        if (EnemyGameObject != null)
        {
            //DealDamageToEnemiesInRange();
            if (Vector3.Distance(SoldierGameObject.transform.position, EnemyGameObject.transform.position) < 5)
            {
                Debug.Log("Enemy in range");
                Debug.Log("moving to enemy myself");
                myAgent.SetDestination(EnemyGameObject.transform.position);
                DealDamageToEnemiesInRange();
            }
        }
    }

    private void SpawnBuildings()
    {
        buildingGameObject = selectionmanager.GetBuildingToAttack();
        if (buildingBase)
            buildingBase = buildingGameObject.GetComponent<BuildingBase>();

        if (buildingGameObject != null)
        {
            currentBuildingDist = Vector3.Distance(transform.position, selectionmanager.buildingPosition);
            if (currentBuildingDist <= 1)
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
                buildingBase.buildingHp -= damageAmount;
                print(buildingBase.buildingHp);
                damageTimer = 0.0f;
            }
            if (buildingBase.buildingHp <= 0)
            {
                Debug.Log("building Destroyed");
                Destroy(buildingGameObject);
                isAttacking = false;
            }
        }
    }

    //damage to enemies
    private void DealDamageToEnemiesInRange()
    {
        enemyHp = EnemyGameObject.GetComponent<Unit>();
        if (Vector3.Distance(SoldierGameObject.transform.position, EnemyGameObject.transform.position) < 1)
        {
            Debug.Log("Attacking Enemy");
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                enemyHp.UnitHealth -= damageAmount;
                print(enemyHp.UnitHealth);
                damageTimer = 0.0f;
            }
            if (enemyHp.UnitHealth <= 0)
            {
                Debug.Log("destoying enemy");
                Destroy(EnemyGameObject);
                isAttacking = false;
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
