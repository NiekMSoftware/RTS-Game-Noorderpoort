using Unity.Netcode;
using UnityEngine;

public class TestSpawnSync : NetworkBehaviour
{
    [SerializeField] private GameObject spawnedObjectPrefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnObjectServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnObjectServerRpc()
    {
        Color objectColor = Color.red;

        GameObject spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
        spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
        spawnedObjectTransform.GetComponent<Renderer>().material.color = objectColor;
    }
}
