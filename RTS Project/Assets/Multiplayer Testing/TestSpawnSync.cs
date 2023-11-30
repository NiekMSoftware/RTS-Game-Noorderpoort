using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestSpawnSync : NetworkBehaviour
{
    [SerializeField] private Transform spawnedObjectPrefab;
    void Update()
    {
        print("123123132");
        //if (!IsOwner) return;
        

        if (Input.GetKeyDown(KeyCode.T))
        {
            print("123");
            Transform spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}
