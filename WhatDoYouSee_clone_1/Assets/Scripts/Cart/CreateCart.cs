using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CreateCart : NetworkBehaviour
{
    public bool isCart = false;
    public override void  OnNetworkSpawn()
    {
        if (!IsServer)
        {
            gameObject.GetComponent<FirstPersonController>().enabled = false;
            //move to point tagged "cartPoint"
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            transform.position = GameObject.FindGameObjectWithTag("CartPoint").transform.position;
            transform.rotation = GameObject.FindGameObjectWithTag("CartPoint").transform.rotation;
            //disable collider
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            isCart = true;
        }
    }
}
