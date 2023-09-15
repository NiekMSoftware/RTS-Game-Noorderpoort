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
        Miner
    }

    public enum TypeUnit {
        Human,
        Animal,
        DarkElves 
    }

    #endregion

    #region Functions

    protected void TakeDamage() {
        
    }
    
    protected void Death() {
        // Kill off the Unit once it's health reaches 0
        if (this.unitHealth <= 0) {
            // Play death animation + particle system
        }
    }

    #endregion
    
}
