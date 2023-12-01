using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TeamIdForObjects : NetworkBehaviour
{
    public NetworkVariable<int> teamIdForObjects = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public bool CheckIfSameTeam(int _teamId)
    {
        if (_teamId == teamIdForObjects.Value)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AssignTeamIdToObject(int _teamId)
    {
        teamIdForObjects.Value = _teamId;
    }
}
