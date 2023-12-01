using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TeamId : NetworkBehaviour
{
    public NetworkVariable<int> teamId = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        teamId.Value = Random.Range(0, 999999999);
    }
}
