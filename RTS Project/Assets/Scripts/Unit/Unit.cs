using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    // Naming and other basic variables
    [Header("Unit variables")]
    [SerializeField] protected int unitHealth;
    [SerializeField] protected int unitMaxHealth;
    [SerializeField] protected int unitHealing;

    [Space]
    [SerializeField] protected int unitSpeed;
    [SerializeField] protected int unitDamage;

    [Header("Inventory Stuff")]
    protected int maxItems = 5;
    protected int[] inventorySlots = new int[3];

    [Header("Enum Data")]
    [SerializeField] protected Jobs job;
    [SerializeField] public TypeUnit typeUnit;

    [Header("Select Agent Movement")]
    [SerializeField] GameObject selectionObject;
    [SerializeField] protected NavMeshAgent myAgent;
    [SerializeField] LayerMask groundLayer;
    [Space]
    [SerializeField] GameObject marker;
    [SerializeField] LayerMask clickableUnit;

    Camera myCamera;

    #region Enums

    protected enum Jobs
    {
        None,
        Builder,
        Hunter,
        Explorer,
        Miner,
        Nurse
    }

    public enum TypeUnit
    {
        None,
        Human,
        Animal,
        Enemy,
        Special
    }

    #endregion

    #region Combat Functions

    protected virtual int TakeDamage(int dealtDamage)
    {
        int remainingHealth = this.unitHealth - dealtDamage;
        return remainingHealth;
    }

    protected virtual int DealDamage(int damage)
    {
        return damage;
    }

    protected virtual void Death()
    {

    }

    protected virtual int Heal(int healing)
    {
        int gainedHealth = unitHealth + healing;

        return gainedHealth;
    }

    #endregion

    #region Unit Location Controller

    public void SetSelectionObject(bool value) => selectionObject.SetActive(value);

    public void SendUnitToLocation(Vector3 pos)
    {
        //doesnt work entirely
        if (Vector3.Distance(transform.position, pos) > 1)
        {
            myAgent.SetDestination(pos);
        }
        else
        {
            myAgent.SetDestination(transform.position);
        }
    }

    #endregion
}
