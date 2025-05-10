using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public float timeLimit = 180f;
    
    private float timeRemaining;
    private NetworkVariable<uint> timeRemainingInt = new NetworkVariable<uint>(default, NetworkVariableReadPermission.Everyone);
    private uint lastTimeRemainingInt;
    public Vector2 exitCoordinates = new Vector2(-1, -1);
    public GameObject exitObject;
    [SerializeField]private int winState = -1; //-1, 0, 1 for not over, lose, win
    public int exitRadius = 3;
    private bool isMazeDone = false;

    private bool serverGameManager = false;
    public static int expectedPlayers = 2;
    public List<int> roles = new List<int>(expectedPlayers);
    public  List<GameObject> players = new List<GameObject>(expectedPlayers);
    public List<ulong> playerIds = new List<ulong>(expectedPlayers);

    public int gameState = 0;
    
    
    public Canvas c;
    private TMPro.TextMeshProUGUI timerText;

    public override void OnNetworkSpawn()
    {
        
        if(!IsServer)
        {
            enabled = false;
            return;
        }
        
        serverGameManager = true;
        timeRemaining = timeLimit;
        timeRemainingInt.Value = (uint)timeRemaining;
        lastTimeRemainingInt = timeRemainingInt.Value;

        //randomly determine roles ONCE on the server
        DetermineRoles();

        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
        
        timerText = c.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        StartCoroutine(AssignRoles());
    }
    

    private void DetermineRoles(){
        //0 = cart, 1 = flash

        for(int i = 0; i < expectedPlayers; i++){
            roles.Add(i);
        }
        //shuffle
        for(int i = 0; i < expectedPlayers; i++){
            int temp = roles[i];
            int randomIndex = Random.Range(i, expectedPlayers);
            roles[i] = roles[randomIndex];
            roles[randomIndex] = temp;
        }
    }

    private IEnumerator AssignRoles(){
        //while players.Count < expectedPlayers
        while(players.Count < expectedPlayers){
            yield return new WaitForSeconds(.25f);
        }
        //assign playerIds ONCE everyone is here
        Debug.Log("0: Record player ids");
        for(int i = 0; i < players.Count; i++){
            playerIds.Add(players[i].GetComponent<NetworkObject>().OwnerClientId);
        }
        gameState = 1;
        while(gameState < 2){
            yield return new WaitForSeconds(0.10f);
        }
        Debug.Log("2: Assign roles + cart view");
        for(int i = 0; i < players.Count; i++){
            players[i].GetComponent<Player>().SetRoleClientRPC(roles[i], playerIds[i]);
            if(roles[i] == 0){
                DisableLoadingClientRPC(playerIds[i]);
            }
        }
        gameState++;
    }

    [ClientRpc]
    public void DisableLoadingClientRPC(ulong id){
        if(id != NetworkManager.Singleton.LocalClientId){
            return;
        }
        c.GetComponent<CanvasManager>().HideLoading();
    }
    
    [ClientRpc]
    public void EnableLoadingClientRPC(ulong id){
        if(id != NetworkManager.Singleton.LocalClientId){
            return;
        }
        c.GetComponent<CanvasManager>().ShowLoading();
    }


    

    void Update()
    {
        if(!serverGameManager || isMazeDone == false)
        {
            return;
        }
        if(players.Count < expectedPlayers)
        {
            return;
        }
        timeRemaining -= Time.deltaTime;
        timeRemainingInt.Value = (uint)timeRemaining;
        if(timeRemainingInt.Value != lastTimeRemainingInt && timeRemaining >= 0)
        {
            lastTimeRemainingInt = timeRemainingInt.Value;
            //send message to clients
            UpdateCanvasTime(timeRemainingInt.Value);
        }
        if(timeRemaining <= 0)
        {
            ActivateEndClientRPC(false);
        }

    }

    void UpdateCanvasTime(uint timeVal){
        //find players and focus on their canvas object. Locate the time text and update it.
        UpdateCanvasTimeClientRPC(timeVal);
        
    }

    [ClientRpc]
    private void UpdateCanvasTimeClientRPC(uint timeVal){
        if(timerText == null){
            timerText = c.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        }
        string timeString = timeVal.ToString();
        timerText.text = timeString;
    }

    private void ClientConnected(ulong u){
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        EnableLoadingClientRPC(u);
    }

    private async void ClientDisconnected(ulong u){
        await Task.Yield();
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        
    }

    public void ActivateEnd(bool win){
        winState = win ? 1 : 0;
        ActivateEndClientRPC(win);
    }

    [ClientRpc]
    void ActivateEndClientRPC(bool win){
        if(win){
            //display win message
            winState = 1;
            c.GetComponent<CanvasManager>().ShowWin();
        }
        else{
            //display lose message
            winState = 0;
            c.GetComponent<CanvasManager>().ShowLose();
        }
    }

    public void SetExitCoordinates(int x, int z){
        //Possibly validate coordinates
        exitCoordinates = new Vector2(x, z);
        
        MoveFlashFromExit();
        
    }

    private void MoveFlashFromExit(){
        int flashIndex = roles.IndexOf(1);
        GameObject flash = players[flashIndex];
        Vector2 flashCell = flash.GetComponent<Player>().cellCoords;
        //if the exit is 3 cells away from flash, move flash
        Debug.Log("Distance from exit: " + (Mathf.Abs(flashCell.x - exitCoordinates.x) + Mathf.Abs(flashCell.y - exitCoordinates.y)));
        Debug.Log("Exit coordinates: " + exitCoordinates);
        Debug.Log("Flash coordinates: " + flashCell);
        while(Mathf.Abs(flashCell.x - exitCoordinates.x) + Mathf.Abs(flashCell.y - exitCoordinates.y) < exitRadius){
            //move flash
            flashCell = new Vector2(Random.Range(0, 15), Random.Range(0, 10)); //better would be to take 1 step back constantly. Only problem is when edge is reached.
            Debug.Log("Distance from exit: " + (Mathf.Abs(flashCell.x - exitCoordinates.x) + Mathf.Abs(flashCell.y - exitCoordinates.y)));
            Debug.Log("Exit coordinates: " + exitCoordinates);
            Debug.Log("Flash coordinates: " + flashCell);
        }

        flash.GetComponent<Player>().UpdateCoordinates(flashCell);
    }

    public void MakeExitObject(GameObject exit){
        exitObject = exit;
        //make the collider a trigger
        exitObject.GetComponent<BoxCollider>().isTrigger = true;
        exitObject.GetComponent<MeshRenderer>().enabled = false;
        //tag it with the "Exit" tag
        exitObject.tag = "Exit";
    }

    public void MazeDone(){
        isMazeDone = true;
        //deactivate loading screen for flash
        //Go through each player and call hide loading screen
        for(int i = 0; i < players.Count; i++){
                DisableLoadingClientRPC(playerIds[i]);
        }

    }

}
