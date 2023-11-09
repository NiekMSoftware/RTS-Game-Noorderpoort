using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierUnit : Unit
{
    [SerializeField] private GameObject enemyHouse;
    public float damageInterval = 2.0f;
    public float damageAmount = 10.0f;
    public LayerMask Building;
    public LayerMask Enemy;
    public GameObject EnemyGameObject;
    public GameObject SoldierGameObject;

    private bool isAttacking = false;
    private float damageTimer = 0.0f;
    public BuildingBase buildingBase;
    public SelectionManager selectionmanager = new SelectionManager();
    public float currentBuildingDist;

    private void Update()
    {
        currentBuildingDist = Vector3.Distance(transform.position,selectionmanager.buildingPosition);
        //print(currentDist);
        if (currentBuildingDist <= 1)
        {
            Debug.Log("Attacking");
            isAttacking = true;
            DealDamageToBuildings();
        }
        if (Vector3.Distance(SoldierGameObject.transform.position, EnemyGameObject.transform.position) <5)
        {
            Debug.Log("Enemy in range");
            Debug.Log("moving to enemy myself");
            myAgent.SetDestination(EnemyGameObject.transform.position);
            DealDamageToEnemiesInRange();
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
                //DealDamageToEnemiesInRange();
                buildingBase.buildingHp--;
                print(buildingBase.buildingHp);
                damageTimer = 0.0f;
            }    
        }
    }

    //damage to enemies
    private void DealDamageToEnemiesInRange()
    {
        if (Vector3.Distance(SoldierGameObject.transform.position, EnemyGameObject.transform.position) < 1)
        {
            Debug.Log("Attacking Enemy");
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                unitHealth--;
                print(unitHealth);              
                damageTimer = 0.0f;
            }
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
