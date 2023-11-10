using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SoldierUnit : Unit
{
    public float damageInterval = 2.0f;
    public int damageAmount = 10;
    public LayerMask Building;
    public LayerMask Enemy;
    public List<GameObject> Enemies = new List<GameObject>();
    public List<GameObject> Buildings = new List<GameObject>();
    public GameObject EnemyGameObject;
    public GameObject SoldierGameObject;
    public GameObject buildingGameObject;
    [SerializeField] private GameObject PlaceHolder;

    private bool isAttacking = false;
    private float damageTimer = 0.0f;
    public BuildingBase buildingBase;
    public SelectionManager selectionmanager;
    public float currentBuildingDist;

    private void Start()
    {
        selectionmanager = FindObjectOfType<SelectionManager>();
    }

    private void Update()
    {
        //SpawnBuildings();
        //pak dan de currentBuildingDist.
        currentBuildingDist = Vector3.Distance(transform.position,selectionmanager.buildingPosition);
        if (currentBuildingDist <= 1)
        {
            Debug.Log("Attacking");
            isAttacking = true;
            DealDamageToBuildings();
        }
        if(EnemyGameObject != null)
        {
            if (Vector3.Distance(SoldierGameObject.transform.position, EnemyGameObject.transform.position) <5)
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
        //ComputerEnemy kijk hoeveel gespawned is
        //[SerializeField] private List<BuildingBase> placedBuildings;
        //als buildings is gespawned dan in lijst.
        /*if ()//placebuildings > 0
        {
            Buildings.Add(buildingGameObject);
            //print(Buildings.Count);
            //print(buildingGameObject.name);
            //print(selectionmanager.selectedBuilding.name);
            
        }*/
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
                this.buildingBase.buildingHp -= damageAmount;
                print(buildingBase.buildingHp);
                damageTimer = 0.0f;
            }
            if(this.buildingBase.buildingHp <= 0)
            {
                Debug.Log("building Destroyed");
                Buildings.RemoveAt(0);
                Destroy(selectionmanager.selectedBuilding);
                isAttacking = false;

                if(selectionmanager.selectedBuilding == null)
                {
                    selectionmanager.selectedBuilding = PlaceHolder;
                }
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
