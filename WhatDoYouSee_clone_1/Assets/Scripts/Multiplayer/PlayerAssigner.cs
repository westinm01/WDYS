using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerAssigner : NetworkBehaviour
{
    public bool isCart = false; //true if server is cart, otherwise false.
    private GameManager gm;
    private bool generated = false;
    private static ulong cartId;
    private static ulong flashId;
    public bool rolesAssigned = false;

    public override void OnNetworkSpawn()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.players.Add(gameObject);
        Debug.Log("Added!");
        if (IsServer && !rolesAssigned)
        {
            Debug.Log("Server is waiting");
            StartCoroutine(WaitForPlayers());
            Debug.Log("Done waiting");
        }
    }

    private IEnumerator WaitForPlayers()
    {
        Debug.Log("NetworkManager.Singleton.ConnectedClientsIds.Count: " + NetworkManager.Singleton.ConnectedClientsIds.Count);
        while (NetworkManager.Singleton.ConnectedClientsIds.Count < GameManager.expectedPlayers)
        {
            Debug.Log("Waiting for both players to connect...");
            yield return new WaitForSeconds(1f); // Check every sec
        }

        AssignSelf();
    }

    private void AssignSelf(){
        rolesAssigned = true;





        ulong[] playerIds = new ulong[NetworkManager.Singleton.ConnectedClientsList.Count];
        Debug.Log("Connected clients: " + NetworkManager.Singleton.ConnectedClientsList.Count);
        if (playerIds.Length < 2)
        {
            Debug.LogError("Not enough players to assign roles!");
            return;
        }
        playerIds[0] = NetworkManager.Singleton.ConnectedClientsList[0].ClientId;
        playerIds[1] = NetworkManager.Singleton.ConnectedClientsList[1].ClientId;
        
        bool cartFirst = Random.Range(0, 1000) % 2 == 0;
        
        cartId = cartFirst ? playerIds[0] : playerIds[1];
        flashId = cartFirst ? playerIds[1] : playerIds[0];
        SetIsCartClientRPC(cartId, flashId);
    }

    private void DisableByTag(GameObject g, string tag){
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        foreach(GameObject obj in objs){
            obj.SetActive(false);
        }
    }

    /*[ServerRpc]
    public void SetIsCartServerRPC(){
        if(IsServer && !generated){
            generated = true;
            int random = Random.Range(0, 1000);
            Debug.Log("Random number:"+ random);
            isCart = random % 2 == 0;
        }
        SetIsCartClientRPC(isCart);
    }*/

    [ClientRpc]
    public void SetIsCartClientRPC(ulong cartId, ulong flashId){
        Debug.Log("CartId: " + cartId + " FlashId: " + flashId + ", MyId: " + OwnerClientId);
        if(OwnerClientId == cartId){
            isCart = true;
            Debug.Log("Cart");
            CartSetup();
        }
        else if(OwnerClientId == flashId){
            isCart = false;
            Debug.Log("Flash");
            FlashSetup();
        }
        rolesAssigned = true;

    }

    private void CartSetup(){
            gameObject.GetComponent<FirstPersonController>().enabled = false;
            //move to point tagged "cartPoint"
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            transform.position = GameObject.FindGameObjectWithTag("CartPoint").transform.position;
            transform.rotation = GameObject.FindGameObjectWithTag("CartPoint").transform.rotation;
            //disable collider
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            isCart = true;
            DisableByTag(gameObject, "Flash");
            
            
    }

    private void FlashSetup(){
        DisableByTag(gameObject, "Cart");
        isCart = false;
        transform.position = new Vector3(85, 0, -55);
    }
    
}
