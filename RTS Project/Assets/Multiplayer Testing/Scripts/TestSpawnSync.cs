using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class TestSpawnSync : NetworkBehaviour
{
    [SerializeField] private GameObject spawnedObjectPrefab;

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    SpawnObjectServerRpc();
        //}
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnObjectServerRpc()
    {
        Color objectColor = Color.blue;

        GameObject spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
        spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
        spawnedObjectTransform.GetComponent<Renderer>().material.color = objectColor;
    }
}
