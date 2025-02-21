using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GenerateLandmarks : NetworkBehaviour
{
    public List<GameObject> wallLandMarks = new List<GameObject>();
    public List<GameObject> cornerLandMarks = new List<GameObject>();
    public List<GameObject> centerLandMarks = new List<GameObject>();
    GameManager gm;
    public int gameState = 1; //when this is expected to occur
    public float cellWidth = 10f;
    private int mapWidth = 15; //TODO: this info should be from game manager
    private int mapHeight = 10; //TODO: this info should be from game manager
    
    public override void OnNetworkSpawn(){
        if(!IsServer){
            enabled = false;
            return;
        }

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine(GenerateLandMarksCoroutine());
    }

    IEnumerator GenerateLandMarksCoroutine(){
        while(gm.gameState < gameState){
            //wait for 0.5 seconds
            yield return new WaitForSeconds(0.25f);
           
        }
        //for each row in the maze
        Debug.Log("Generating Landmarks!");
        for(int i = 0; i < mapHeight; i++){
            int randomNumberOfCells = Random.Range(1, mapWidth); //can also change this to be a fixed number
            for(int j = 0; j < randomNumberOfCells; j++){
                int randomCell = Random.Range(0, mapWidth);
                Vector3 cellPosition = GetWorldPosition(randomCell, i);
                Vector2 randomLandmarkData = GenerateRandomLandMark(cellPosition); //number will reflect what type of landmark and what position it was spawned at

            }

        }
        gm.gameState++;
    }
    

    Vector3 GetWorldPosition(int x, int z){
        return new Vector3(85 - x * cellWidth, 0f, -65 + z * cellWidth);
    }

    Vector2 GenerateRandomLandMark(Vector3 cellPosition){
        int randomLandmark = Random.Range(0, 3);
        int randomLandmarkObject = 0;
        
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 3f) * 90, 0);
        if(randomLandmark == 0){
            //wall landmark
            //Pick a random direction
            int direction = Random.Range(0, 4);
            int dx = 0;
            int dz = 0;
            switch(direction){
                case 0:
                    dx = 1;
                    break;
                case 1:
                    dx = -1;
                    break;
                case 2:
                    dz = 1;
                    break;
                case 3:
                    dz = -1;
                    break;
            }

            int randomDecal = Random.Range(0, wallLandMarks.Count);
            Vector3 e = new Vector3(cellPosition.x + dx, cellPosition.y + 1, cellPosition.z + dz);
            //This might be troubling if there are colliders in the way...
            SpawnObjectClientRPC(randomLandmark, randomDecal, cellPosition, randomRotation, e, new Vector3(dz, 0f, -1* dx)); //the d parameter is strange, but it's the direction vector to hit the right wall
            randomLandmarkObject = randomDecal;
            //GameObject wallLandMark = Instantiate(wallLandMarks[Random.Range(0, wallLandMarks.Count)], cellPosition, Quaternion.identity); //TODO: pick a random wall and use it's rotation and position
            //randomLandmarkObject =
        }
        else if(randomLandmark == 1){
            //corner landmark
            Vector3 cornerPosition = new Vector3(cellPosition.x + cellWidth * (Random.Range(0,2) * 0.6f - 0.3f), cellPosition.y, cellPosition.z + cellWidth * (Random.Range(0,2) * 0.6f - 0.3f));
            int randomCornerLandmark = Random.Range(0, cornerLandMarks.Count);
            //Debug.Log("SERVER: Spawning corner landmark at " + cornerPosition);
            SpawnObjectClientRPC(randomLandmark, randomCornerLandmark, cornerPosition, randomRotation);
            randomLandmarkObject = randomCornerLandmark;
        }
        else{
            //center landmark
            int randomCenterLandmark = Random.Range(0, centerLandMarks.Count);
            //Debug.Log("SERVER: Spawning corner landmark at " + cellPosition);
            SpawnObjectClientRPC(randomLandmark, randomCenterLandmark, cellPosition, randomRotation);
            randomLandmarkObject = randomCenterLandmark;
        }
        return new Vector2(randomLandmark, randomLandmarkObject);
    }

    [ClientRpc]
    void SpawnObjectClientRPC(int landmarkList, int landmark, Vector3 position, Quaternion rotation, Vector3 e = new Vector3(), Vector3 d = new Vector3()){
        //Debug.Log("CLIENT: Spawning landmark at " + position);
        //TODO: Spawn the object at an appropriate y (might need to calculate object height somehow...)
        if(landmarkList == 0){
            RaycastHit hit;
            if (Physics.Raycast(e, d, out hit, cellWidth))
            {
                    GameObject right = hit.collider.gameObject;
                    //create a new landmark object as a child of right
                    GameObject newLandmark = Instantiate(wallLandMarks[landmark], right.transform);
                    newLandmark.transform.localScale = new Vector3(1f, 1f, 1f);
                    newLandmark.transform.localRotation = Quaternion.Euler(0f, 90f, 0f); //rotate y
                    newLandmark.transform.localPosition = new Vector3(0.5f * d.z, 0f, 0.5f * d.x);
                    
                    
                    
            }
            //Instantiate(wallLandMarks[landmark], position, Quaternion.identity);
        }
        else if(landmarkList == 1){
            Instantiate(cornerLandMarks[landmark], position, rotation);
        }
        else{
            Instantiate(centerLandMarks[landmark], position, rotation);
        }
        
        
    }

}
