using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public float spawnInterval = 3f;
    public int maxCubes = 6; 
    private float timer = 0f;
    private int spawnedCubes = 0;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval && spawnedCubes < maxCubes)
        {
            Instantiate(cubePrefab, transform.position, Quaternion.identity);

            timer = 0f;

            spawnedCubes++;
        }
    }
}
