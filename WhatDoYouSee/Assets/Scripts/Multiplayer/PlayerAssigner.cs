using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerAssigner : NetworkBehaviour
{
    public bool isCart = false;
    private GameManager gm;
    public override void  OnNetworkSpawn()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        AssignSelf();
    }

    private void AssignSelf(){
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
            DisableByTag(gameObject, "Flash");
            
            gm.players.Add(gameObject);
        }
        else{
            DisableByTag(gameObject, "Cart");
            gm.players.Add(gameObject);
        }
    }

    private void DisableByTag(GameObject g, string tag){
        //recursively go through children and disable by tag
        /*if(g.tag == tag){
            g.SetActive(false);
        }
        else{
            foreach(Transform child in g.transform){
                DisableByTag(child.gameObject, tag);
            }
        }*/

        //find all objects with tag tag
        //disable them
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        foreach(GameObject obj in objs){
            obj.SetActive(false);
        }
        
    }
    
}
