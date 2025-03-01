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

    public Vector2 cellCoords = new Vector2(-1, -1);

    [ClientRpc]
    public void SetRoleClientRPC(int r, ulong id){
        if(id != NetworkManager.Singleton.LocalClientId){
            return;
        }
        role = r;
        if(role == 0){
            CartSetup();
            
        }else{
            FlashSetup();
            
        }
    }


    private void DisableByTag(string tag){
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        foreach(GameObject obj in objs){
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
            //get cameracontrol
            CameraControl cc = gameObject.GetComponent<CameraControl>();
            cc.setOriginalPosition(transform.position);
            cc.setOriginalRotation(transform.eulerAngles);

            DisableByTag("Flash");
            
            
    }

    private void FlashSetup(){
        DisableByTag("Cart");
        //Random start cell
        
        int randomCell = Random.Range(0, width); //first number
        int x = randomCell;
        randomCell = Random.Range(0, height);
        int z = randomCell;
        Debug.Log("Random Flash starting cell: " + x + ", " + z);

        float worldi_x = startX - x * cellSpacing;
        float worldi_z = startZ + z * cellSpacing;
        cellCoords = new Vector2(x, z);

        transform.position = new Vector3(worldi_x, 1.5f, worldi_z);
    }

    void OnTriggerEnter(Collider collision)
    {
        if(role == 1 && collision.gameObject.CompareTag("Exit"))
        {
            
            //Flash wins
            
            ActivateEndServerRPC(true);//might need to take this to the serverRPC
        }
    }
    [ServerRpc]
    void ActivateEndServerRPC(bool win){
        GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        gm.ActivateEnd(win);
    }

    public void UpdateCoordinates(Vector2 coords){
        cellCoords = coords;
        float worldi_x = startX - coords.x * cellSpacing;
        float worldi_z = startZ + coords.y * cellSpacing;
        transform.position = new Vector3(worldi_x, 1.5f, worldi_z);
    }

    /*public IEnumerator WaitForMaze(CanvasManager cm){
        //maybe put disable controls
        while(!isMazeDone){
            yield return null;
        }
        Debug.Log("Maze is done");
        //maze is done
        //disable loading screen
        cm.HideLoading();
    }*/
}
