using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject cubePrefab;
    public float spawnCooldown = 0f;
    private float cooldownTimer = 0f;
    public List<GameObject> spawnedCubes = new List<GameObject>();
    public int maxCubes = 0;

    void Update()
    {
        cooldownTimer += Time.deltaTime;
        CheckCubes();
        
        if (cooldownTimer >= spawnCooldown && spawnedCubes.Count < maxCubes)
        {
            SpawnCube();
            cooldownTimer = 0f;
        }
    }

    void SpawnCube()
    {
        if (spawnedCubes.Count < maxCubes)
        {
            GameObject newCube = Instantiate(cubePrefab, transform.position, Quaternion.identity);
            spawnedCubes.Add(newCube);
        }
    }

    public void CheckCubes()
    {
        for(int i = 0; i< spawnedCubes.Count; i++)
        {
            if (spawnedCubes[i] == null) 
            {
                //print("er ontbreekt een kubus!");
                spawnedCubes.RemoveAt(i);

            } 
        }
    }

    public void OnCubeDeleted(GameObject cubeToDelete)
    {
        print("cube deleted");
       //DestroyImmediate(cubeToDelete, true);
        spawnedCubes.Remove(cubeToDelete);
         
        SpawnCube();
    }
}
