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
            yield return new WaitForSeconds(0.5f);
           
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
    }
    

    Vector3 GetWorldPosition(int x, int z){
        return new Vector3(85 - x * cellWidth, 0f, -65 + z * cellWidth);
    }

    Vector2 GenerateRandomLandMark(Vector3 cellPosition){
        int randomLandmark = Random.Range(0, 3);
        int randomLandmarkObject = 0;
        if(randomLandmark == 0){
            //wall landmark
            //GameObject wallLandMark = Instantiate(wallLandMarks[Random.Range(0, wallLandMarks.Count)], cellPosition, Quaternion.identity); //TODO: pick a random wall and use it's rotation and position
            //randomLandmarkObject =
        }
        else if(randomLandmark == 1){
            //corner landmark
            Vector3 cornerPosition = new Vector3(cellPosition.x + cellWidth * (Random.Range(0,2) * 0.6f - 0.3f), cellPosition.y, cellPosition.z + cellWidth * (Random.Range(0,2) * 0.6f - 0.3f));
            int randomCornerLandmark = Random.Range(0, cornerLandMarks.Count);
            //Debug.Log("SERVER: Spawning corner landmark at " + cornerPosition);
            SpawnObjectClientRPC(randomLandmark, randomCornerLandmark, cornerPosition, Quaternion.identity);
            randomLandmarkObject = randomCornerLandmark;
        }
        else{
            //center landmark
            int randomCenterLandmark = Random.Range(0, centerLandMarks.Count);
            //Debug.Log("SERVER: Spawning corner landmark at " + cellPosition);
            SpawnObjectClientRPC(randomLandmark, randomCenterLandmark, cellPosition, Quaternion.identity);
            randomLandmarkObject = randomCenterLandmark;
        }
        return new Vector2(randomLandmark, randomLandmarkObject);
    }

    [ClientRpc]
    void SpawnObjectClientRPC(int landmarkList, int landmark, Vector3 position, Quaternion rotation){
        //Debug.Log("CLIENT: Spawning landmark at " + position);
        //TODO: Spawn the object at an appropriate y (might need to calculate object height somehow...)
        if(landmarkList  == 0){
            Instantiate(wallLandMarks[landmark], position, Quaternion.identity);
        }
        else if(landmarkList == 1){
            Instantiate(cornerLandMarks[landmark], position, Quaternion.identity);
        }
        else{
            Instantiate(centerLandMarks[landmark], position, Quaternion.identity);
        }
        
        
    }

}
