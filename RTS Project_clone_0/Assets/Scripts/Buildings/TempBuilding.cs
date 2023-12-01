using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempBuilding : MonoBehaviour
{
    private GameObject buildingToSpawn;
    private float buildTime;

    private void Update()
    {
        if (buildingToSpawn == null) return;

        buildTime -= Time.deltaTime;

        if (buildTime < 0)
        {
            Build();
        }
    }

    public void Setup(GameObject buildingToSpawn, float buildTime)
    {
        this.buildingToSpawn = buildingToSpawn;
        this.buildTime = buildTime;
    }

    private void Build()
    {
        Instantiate(buildingToSpawn, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
