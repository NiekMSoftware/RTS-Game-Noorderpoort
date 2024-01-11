using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    // TODO: Refactor the cast performing so it will shoot a ray, then do an overlap sphere on said ground to check the radius
    // if there are things in that radius show them, else not (this goes for all units).

    // TODO: Rewrite the entire fog of war :(

    [Header("Unit Properties")]
    public List<SoldierUnit> soldiers;
    public List<Unit> enemyUnits;

    [Space]
    
    public LayerMask soldierLayer;

    [Header("Fog of War Properties")]
    [SerializeField] private Transform fogTransform;

    void Update()
    {
        // Gather all local soldiers
        UpdateSoldierList();

        // Only start routine once there are units in the list
        if (soldiers.Count > 0)
            StartCoroutine(nameof(SendRays));
        else
        {
            Debug.LogWarning("Not running routine anymore!");
        }
    }

    IEnumerator SendRays()
    {
        for (int i = 0; i < soldiers.Count; i++)
        {
            Transform soldierPositions = soldiers[i].transform.GetComponent<Transform>();
            print($"Soldier(s) {i + 1}: {soldiers[i].transform.position}");
        }

        if (soldiers.Count == 0) yield break;
        Debug.LogWarning("Ending loop...");
    }

    private void UpdateSoldierList()
    {
        // Remove any null or destroyed soldiers from the List
        soldiers.RemoveAll(soldier => soldier == null);

        // find the game object that the soldiers are
        GameObject[] soldierObjects = GameObject.FindGameObjectsWithTag("AI");

        // iterate over each obj
        foreach (var obj in soldierObjects)
        {
            SoldierUnit soldier = obj.GetComponent<SoldierUnit>();

            if(soldier != null && !soldiers.Contains(soldier))
                soldiers.Add(soldier);
        }
    }
}
