using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrack_Door : MonoBehaviour
{
    private Barrack barrack;

    private void Awake() {
        barrack = GameObject.Find("Barrack").GetComponent<Barrack>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("AI")) {
            // Destroy the unit
            Destroy(other.gameObject);
            
            // Add a new item to the list!
            barrack.UnitList.Add(barrack.unitToSpawn);
        }
    }
}
