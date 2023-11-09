using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierUnit : Unit
{
    public float damageInterval = 2.0f;
    public int damageAmount = 10;
    public LayerMask Building;
    public LayerMask Enemy;
    public GameObject EnemyGameObject;
    public GameObject SoldierGameObject;
    public GameObject buildingGameObject;

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
                buildingBase.buildingHp -= damageAmount;
                print(buildingBase.buildingHp);
                damageTimer = 0.0f;
            }
            if(buildingBase.buildingHp <= 0)
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
        if (Vector3.Distance(SoldierGameObject.transform.position, EnemyGameObject.transform.position) < 1)
        {
            Debug.Log("Attacking Enemy");
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                unitHealth -= damageAmount;
                print(unitHealth);              
                damageTimer = 0.0f;
            }
            if (unitHealth <= 0)
            {
                Debug.Log("destoying enemy");
                Destroy(EnemyGameObject);
                isAttacking = false;
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
