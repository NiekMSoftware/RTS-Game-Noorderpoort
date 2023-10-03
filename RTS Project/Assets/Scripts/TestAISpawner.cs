using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAISpawner : MonoBehaviour
{
    [SerializeField] private GameObject aiToSpawn;
    [SerializeField] private LayerMask ground;
    [SerializeField] private Terrain terrain;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
            {
                Vector3 spawnPos = hit.point;
                spawnPos.y = terrain.SampleHeight(hit.point) + aiToSpawn.transform.localScale.y;
                Instantiate(aiToSpawn, spawnPos, Quaternion.identity);
            }
        }
    }
}