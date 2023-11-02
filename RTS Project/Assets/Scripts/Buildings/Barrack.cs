using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrack : MonoBehaviour
{
    // TODO: Clear out all pending Unit's in barrack and release them
        //  (Release them, is clearing them out of the list 1 by 1 until 0 remains)
    
    [Header("Unit's List")]
    private List<GameObject> unitList = new List<GameObject>(5);
    public GameObject unitToSpawn;
    private const int CAPACITY = 5;

    [Header("Barrack Properties")]
    [SerializeField] private float queue = 0f;  // queue to keep track of
    [SerializeField] private float maxTimeUntilNext = 5f;   // max time var, this will also be used on Invoke
    [SerializeField] private Transform spawnPosition;
    private bool waitUntilSpawned = false;

    private void Start() {
        InvokeRepeating(nameof(InvokeSpawnEnemies), 1f, 1f);
    }

    private void Update() {
        CountDown();
    }

    private void InvokeSpawnEnemies() {
        if (unitList.Count >= 0) {
            AddUnitToList();
        }

        if (unitList.Count >= CAPACITY) {
            SpawnEnemy();
        }
    }
    
    private void AddUnitToList() {
        // Reset the queue and spawn in Units
        if (CountDown() >= maxTimeUntilNext) {
            if(!waitUntilSpawned) {
                // Check if the list isn't already full
                if (unitList.Count != CAPACITY) {
                    // Add unit to list
                    unitList.Add(unitToSpawn);
                    print($"Added unit {unitList.Count}");
                    
                    print("Spawned in units");
                    Instantiate(unitToSpawn, spawnPosition);
                }
            }
            
            // // Check if the unit hit their capacity
            if (unitList.Count == CAPACITY) {
                waitUntilSpawned = true;
            }
            
            queue = 0.0f;
        }
    }

    private void SpawnEnemy() {
        // Removing stuff :D
        for (int i = unitList.Count - 1; i >= 0; i--) {
            print("REMOVING ITEMS OUT OF LIST");
            unitList.RemoveAt(i);
        }
        
        waitUntilSpawned = false;
    }
    
    private float CountDown() => queue += Time.deltaTime;
}
