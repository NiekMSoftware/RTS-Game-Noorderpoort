using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Unit Boisky", menuName = "Create Unit", order = 1)]
[Serializable]
public class Unit : ScriptableObject {
    // Naming and other basic variables
    [Header("Basic Variables and Naming")]
    
    [SerializeField] string unitName;

    [Space] 
    [SerializeField] int unitHealth;
    [SerializeField] int unitMaxHealth;
    [SerializeField] int unitHealing;
    
    [Space]
    [SerializeField] int unitSpeed;
    [SerializeField] int unitDamage;

    [Header("Enum Data")]
    [SerializeField] Jobs job;
    [SerializeField] TypeUnit typeUnit;

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

    public Unit() {
        this.TakeDamage(this.DealDamage(this.unitDamage));
        this.Death();
    }
}
