using System;
using UnityEngine;

public abstract class Unit : MonoBehaviour {
    // Naming and other basic variables
    [Header("Unit variables")]
    [SerializeField] protected int unitHealth;
    [SerializeField] protected int unitMaxHealth;
    [SerializeField] protected int unitHealing;
    
    [Space]
    [SerializeField] protected int unitSpeed;
    [SerializeField] protected int unitDamage;

    [Header("Inventory Stuff")] 
    protected const int maxItems = 5;
    protected int[] inventorySlots = new int[3];

    [Header("Enum Data")]
    [SerializeField] protected Jobs job;
    [SerializeField] protected TypeUnit typeUnit;

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

    protected int TakeDamage(int dealtDamage) {
        int remainingHealth = this.unitHealth - dealtDamage;
        return remainingHealth;
    }

    protected int DealDamage(int damage) {
        return damage;
    }
    
    protected void Death() {
        // Kill off the Unit once it's health reaches 0
        if (this.unitHealth <= 0) {
            // Play death animation + particle system
        }
    }

    protected int Heal(int healing) {
        int gainedHealth = this.unitHealth + healing;

        return gainedHealth;
    }

    #endregion

    protected void DetectUnit() {
        
    }
}
