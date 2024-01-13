using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    // TODO: Refactor the cast performing so it will shoot a ray, then do an overlap sphere on said ground to check the radius
    // if there are things in that radius show them, else not (this goes for all units).

    [Header("Unit Properties")]
    public List<SoldierUnit> soldiers;
    public List<Unit> enemyUnits;

    // private int to keep track of the soldiers
    public int foundSoldiers = 0;

    [Space]
    
    public LayerMask soldierLayer;

    [Header("Fog of War Properties")]
    [SerializeField] private Transform fogTransform;

    void Update()
    {
        // Gather all local soldiers
        UpdateSoldierList();

        // Gather each soldier's pos in the List
        StartCoroutine(nameof(FindPositions));

        // If there are soldiers found, send the rays
        if (foundSoldiers != 0)
        {
            StartCoroutine(nameof(SendRays));
        }
    }

    IEnumerator SendRays()
    {
        Debug.LogWarning("SendRays() is being called, but this statement is empty.");
        if (foundSoldiers == 0)
            yield break;

        yield return null;
    }

    IEnumerator FindPositions()
    {
        while (foundSoldiers != soldiers.Count)
        {
            // if in general the count is equal to 0, break loop
            if (soldiers.Count == 0) yield break;

            for (int i = 0; i < soldiers.Count; i++)
            {
                Transform soldierPositions = soldiers[i].transform.GetComponent<Transform>();
                print($"Soldier(s) {i + 1}: {soldierPositions.position}");
            }

            // return null to avoid freezing
            yield return null;
        }
    }

    private void UpdateSoldierList()
    {
        // Count the number of null items
        int nullSoldiers = soldiers.RemoveAll(soldier => soldier == null);
        
        // Decrease the foundSoldiers
        foundSoldiers -= nullSoldiers;

        // Remove any null items from the soldiers list
        soldiers.RemoveAll(soldier => soldier == null);

        // Remove any null items from the enemies list
        enemyUnits.RemoveAll(soldier => soldier == null);

        // find the game object that the soldiers are
        SoldierUnit[] soldierObjects = FindObjectsOfType<SoldierUnit>();

        // iterate over each obj
        foreach (var obj in soldierObjects)
        {
            SoldierUnit soldier = obj.GetComponent<SoldierUnit>();

            // return if true
            if (soldier == null)
                return;

            switch (soldiers.Contains(soldier))
            {
                case false when obj.Type == Unit.TypeUnit.Human:
                    soldiers.Add(soldier);
                    foundSoldiers++;
                    break;
                case false when obj.Type == Unit.TypeUnit.Enemy && !enemyUnits.Contains(soldier):
                    enemyUnits.Add(soldier);
                    break;
            }
        }
    }
}
