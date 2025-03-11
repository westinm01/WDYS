using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GenerateMazeWalls : NetworkBehaviour
{
    public int width, height;
    public int numCells;
    private bool[,] visited;
    public GameManager gm;
    public int gameState = 3; //when this is expected to occur
    

    // Constants for cell spacing and starting position
    private const float startX = 85f, startZ = -65f, cellSpacing = 10f;


    public override void OnNetworkSpawn()
    {
        if(!IsServer)
        {
            enabled = false;
            return;
        }
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        StartCoroutine(GenerateMazeCoroutine());
    }

    IEnumerator GenerateMazeCoroutine()
    {
        while(gm.gameState < gameState){
            yield return new WaitForSeconds(0.25f);
        }
        Debug.Log(gameState + ": Generating Maze Walls!");

        //This if-else creates the exit
        //pick a random number 0 to 4
        int randomSide = Random.Range(0, 4);

        if(randomSide % 2 == 1){
            //left or right
            int randomCell = Random.Range(0, height);
            
            if(randomSide == 1){
                //left
                Debug.Log("Destroying wall on left " + randomCell);
                DestroyWall(new Vector2Int(0, randomCell), new Vector2Int(-1, randomCell), true);
            }
            else{
                //right
                Debug.Log("Destroying wall on right " + randomCell);
                DestroyWall(new Vector2Int(width - 1, randomCell), new Vector2Int(width, randomCell), true);
            }
        }
        else{
            //top or bottom
            int randomCell = Random.Range(0, width);
            if(randomSide == 0){
                //top
                Debug.Log("Destroying wall on top " + randomCell);
                DestroyWall(new Vector2Int(randomCell, 0), new Vector2Int(randomCell, -1), true);
            }
            else{
                //bottom
                Debug.Log("Destroying wall on bottom " + randomCell);
                DestroyWall(new Vector2Int(randomCell, height - 1), new Vector2Int(randomCell, height), true);
            }
        }



        int debugCounter = 20000; // no infinite loops

        visited = new bool[width, height];  // Initialize grid
        print("Generating maze with " + width + "x" + height + " cells");

        // Mark all cells as unvisited
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                visited[x, y] = false;
            }
        }

        // Choose a random starting cell and mark it as visited
        Vector2Int startNode = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        visited[startNode.x, startNode.y] = true;

        // Generate maze using Wilson's algorithm
        print("Starting Wilson's algorithm with node " + startNode);
        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(startNode);

        while (path.Count < numCells && debugCounter > 0)
        {
            Vector2Int currentNode = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            List<Vector2Int> currPath = new List<Vector2Int>();

           // print("Starting loop-erased random walk from " + currentNode);
            while (debugCounter > 0)
            {
                if (path.Contains(currentNode))
                {
                    //print("PATH HIT at " + currentNode);
                    break;
                }

                //print("Unvisited node: " + currentNode);

                if (currPath.Contains(currentNode))
                {
                    //print("Loop detected at " + currentNode);
                    int loopStartIndex = currPath.IndexOf(currentNode);
                    currPath.RemoveRange(loopStartIndex + 1, currPath.Count - loopStartIndex - 1);
                }

                currPath.Add(currentNode);
                currentNode = GetRandomNeighbor(currentNode);
                debugCounter--;
                yield return null; // Pause here and resume in the next frame
            }

            if (!currPath.Contains(currentNode) && currPath.Count > 0)
            {
                currPath.Add(currentNode);
            }

            //print("Current path is " + currPath.Count + " long");

            foreach (var node in currPath)
            {
                if (!path.Contains(node))
                {
                    path.Add(node);
                    visited[node.x, node.y] = true;
                }
            }

            for (int i = 0; i < currPath.Count - 1; i++)
            {
                DestroyWall(currPath[i], currPath[i + 1]);
                //yield return new WaitForSeconds(0.035f); // Pause for visibility
                yield return null;
            }

            //print("TOTAL Path length: " + path.Count);
            debugCounter--;
        }
        gm.MazeDone();
        
    }

    Vector2Int GetRandomNeighbor(Vector2Int node)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        if (node.x > 0) neighbors.Add(new Vector2Int(node.x - 1, node.y));
        if (node.x < width - 1) neighbors.Add(new Vector2Int(node.x + 1, node.y));
        if (node.y > 0) neighbors.Add(new Vector2Int(node.x, node.y - 1));
        if (node.y < height - 1) neighbors.Add(new Vector2Int(node.x, node.y + 1));

        return neighbors.Count > 0 ? neighbors[Random.Range(0, neighbors.Count)] : node;
    }

    void DestroyWall(Vector2Int i, Vector2Int j, bool isExit = false)
    {
        float worldi_x = startX - i.x * cellSpacing;
        float worldi_z = startZ + i.y * cellSpacing;
        float worldj_x = startX - j.x * cellSpacing;
        float worldj_z = startZ + j.y * cellSpacing;

        Vector3 direction = new Vector3(worldj_x - worldi_x, 0, worldj_z - worldi_z).normalized;
        Vector3 rayStart1 = Vector3.zero;
        Vector3 rayStart2 = Vector3.zero;

        if (worldj_x == worldi_x)
        {
            rayStart1 = new Vector3(worldi_x - 0.7f, 2.5f, worldi_z);
            rayStart2 = new Vector3(worldi_x + 0.7f, 2.5f, worldi_z);
        }
        else
        {
            rayStart1 = new Vector3(worldi_x, 2.5f, worldi_z - 0.7f);
            rayStart2 = new Vector3(worldi_x, 2.5f, worldi_z + 0.7f);
        }

        if(isExit){
            gm.SetExitCoordinates(i.x, i.y);
        }
        
        DestroyWallClientRPC(rayStart1, direction, isExit);
        DestroyWallClientRPC(rayStart2, direction, isExit);
        //exception handling maybe

        //print($"Destroyed wall between ({i.x}, {i.y}) and ({j.x}, {j.y})");
        
    }

    [ClientRpc]
    void DestroyWallClientRPC(Vector3 rayStart, Vector3 direction, bool isExit = false){

        RaycastHit hit;
        if (Physics.Raycast(rayStart, direction, out hit, cellSpacing))
        {
            if (hit.collider.CompareTag("innerWall") || isExit == true)
            {
                GameObject oldWall = hit.collider.gameObject;
                if(isExit){
                    GameObject exit = Instantiate(oldWall, oldWall.transform.position, oldWall.transform.rotation);
                    gm.MakeExitObject(exit);
                }
                
                
                Destroy(oldWall);
            }
        }
    }
}
