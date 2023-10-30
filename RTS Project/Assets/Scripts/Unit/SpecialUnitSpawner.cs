using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialUnitSpawner : MonoBehaviour
{
    public GameObject SpecialUnit;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 randomSpawnPosition = new Vector3(Random.Range(-10,11),5,Random.Range(-10,11));
            Instantiate(SpecialUnit, randomSpawnPosition, Quaternion.identity);
        }
    }
}
