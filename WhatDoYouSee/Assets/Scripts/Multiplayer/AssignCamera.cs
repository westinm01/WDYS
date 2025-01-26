using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AssignCamera : NetworkBehaviour
{
    public override void OnNetworkSpawn(){
    
        if (!IsOwner)
        {
            gameObject.SetActive(false);
        }
    }
}
