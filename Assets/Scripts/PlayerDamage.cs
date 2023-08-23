using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerDamage : NetworkBehaviour
{
    public void DealDamage()
    {
        Debug.Log($"{OwnerClientId} was shot");

        //Reset Position
        GameManager.Instance.ResetPlayerPosition(NetworkObject, OwnerClientId);
    }
}
