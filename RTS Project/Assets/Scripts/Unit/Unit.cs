using System;
using UnityEngine;

public class Unit : MonoBehaviour {
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
    [SerializeField] protected TypeUnit typeUnit;

    void Start() {
        // Insert code here that needs to be initiated from the start method.
    }

    void Update() {
        // Insert code here that needs to use the Update function
            // Think of: Looking for enemies, healing allies etc.
    }

    #region Enums

    public enum Jobs {
        None,
        Builder,
        Hunter,
        Explorer,
        Miner,
        Nurse
    }

    public enum TypeUnit {
        None,
        Human,
        Animal,
        DarkElves 
    }

    #endregion
    
    #region Combat Functions

    public int TakeDamage(int dealtDamage) {
        int remainingHealth = this.unitHealth - dealtDamage;
        return remainingHealth;
    }

    public int DealDamage(int damage) {
        return damage;
    }
    
    public void Death() {
        // Kill off the Unit once it's health reaches 0
        if (unitHealth <= 0) {
            // Play death animation + particle system
        }
    }

    public int Heal(int healing) {
        int gainedHealth = unitHealth + healing;

        return gainedHealth;
    }

    #endregion

    #region Unit Controller

    // Give the unit other functions
    
    #endregion
}
