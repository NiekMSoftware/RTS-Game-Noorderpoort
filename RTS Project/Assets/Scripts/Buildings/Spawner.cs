using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Prefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Instantiate(Prefab, transform.position, Quaternion.identity);
        }
    }
}
