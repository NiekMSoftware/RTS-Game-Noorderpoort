using Unity.Netcode;
using UnityEngine;

public class TestSpawnSync : NetworkBehaviour
{
    [SerializeField] private GameObject spawnedObjectPrefab;
    private GameObject spawnedObjectTransform;



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
        Color objectColor = Color.blue;

        spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
        spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
        spawnedObjectTransform.GetComponent<Renderer>().material.color = objectColor;
        SpawnObjectPrintClientRpc();
    }

    [ClientRpc]
    void SpawnObjectPrintClientRpc()
    {
        print(spawnedObjectTransform.GetComponent<NetworkObject>().IsOwner);

    }


    // Network variable to synchronize team ID
    public NetworkVariable<int> teamID = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // Other player-related variables and methods...

    private void Start()
    {
        print(IsOwner);
        this.OnGainedOwnership();
        print(IsOwner);
        // Set initial team ID when the player spawns
        if (IsLocalPlayer)
        {
            // You might want to implement a team selection menu for players
            // For simplicity, let's assume the player starts with team 1
            CmdChangeTeamServerRpc(1);
        }
    }

    [ServerRpc]
    private void CmdChangeTeamServerRpc(int newTeamID)
    {
        teamID.Value = newTeamID;
        RpcUpdateTeamOnClientsClientRpc(newTeamID);
    }

    [ClientRpc]
    private void RpcUpdateTeamOnClientsClientRpc(int newTeamID)
    {
        // This method is called on all clients when the team ID changes
        teamID.Value = newTeamID;

        // Update visuals, materials, or any other client-side changes related to team
    }
}
