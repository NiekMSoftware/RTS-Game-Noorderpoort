using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierUnit : Unit
{
    public float damageInterval = 2.0f;
    public float damageAmount = 10.0f;
    public LayerMask enemyLayer;

    private Transform targetBuilding;
    private bool isAttacking = false;
    private float damageTimer = 0.0f;

    private void Update()
    {
        if (isAttacking)
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                DealDamageToEnemiesInRange();
                damageTimer = 0.0f;
            }
        }
    }

    public void AssignToBuilding(Transform buildingTransform)
    {
        targetBuilding = buildingTransform;
        isAttacking = true;
    }

    private void DealDamageToEnemiesInRange()
    {
        Collider[] colliders = Physics.OverlapBox(
            transform.position, Vector3.one, Quaternion.identity, enemyLayer);

        foreach (var collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                SoldierUnit enemySoldier = collider.GetComponent<SoldierUnit>();
                if (enemySoldier != null)
                {
                    enemySoldier.TakeDamage(damageAmount);
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
