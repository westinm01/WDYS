using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    // Start is called before the first frame update
    public int role = -1;
    private const float startX = 85f, startZ = -65f, cellSpacing = 10f; //also in GenerateMazeWalls. Might be better in GM.
    int width = 15;
    int height = 10;

    [ClientRpc]
    public void SetRoleClientRPC(int r, ulong id){
        Debug.Log(id + "vs." + NetworkManager.Singleton.LocalClientId);
        if(id != NetworkManager.Singleton.LocalClientId){
            return;
        }
        role = r;
        if(role == 0){
            Debug.Log("I am Cart");
            CartSetup();
            
        }else{
            Debug.Log("I am Flash");
            FlashSetup();
            
        }
    }


    private void DisableByTag(string tag){
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        foreach(GameObject obj in objs){
            Debug.Log("Disabling " + obj.name);
            obj.SetActive(false);
        }
    }


    private void CartSetup(){
            gameObject.GetComponent<FirstPersonController>().enabled = false;
            //move to point tagged "cartPoint"
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            transform.position = GameObject.FindGameObjectWithTag("CartPoint").transform.position;
            transform.rotation = GameObject.FindGameObjectWithTag("CartPoint").transform.rotation;
            //disable collider
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            
            DisableByTag("Flash");
            
            
    }

    private void FlashSetup(){
        DisableByTag("Cart");
        //Random start cell
        
        int randomCell = Random.Range(0, width); //first number
        int x = randomCell;
        randomCell = Random.Range(0, height);
        int z = randomCell;
        Debug.Log("Random cell: " + x + ", " + z);

        float worldi_x = startX - x * cellSpacing;
        float worldi_z = startZ + z * cellSpacing;

        transform.position = new Vector3(worldi_x, 1.5f, worldi_z);
    }

    void OnTriggerEnter(Collider collision)
    {
        if(role == 1 && collision.gameObject.CompareTag("Exit"))
        {
            
            //Flash wins
            GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            gm.ActivateEnd(true);
        }
    }
}
