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
    private bool serverGameManager = false;
    public static int expectedPlayers = 2;
    public List<int> roles = new List<int>(expectedPlayers);
    public  List<GameObject> players = new List<GameObject>(expectedPlayers);
    public List<ulong> playerIds = new List<ulong>(expectedPlayers);
    
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
            yield return new WaitForSeconds(1f);
        }
        //assign playerIds ONCE everyone is here
        for(int i = 0; i < players.Count; i++){
            playerIds.Add(players[i].GetComponent<NetworkObject>().OwnerClientId);
        }
        for(int i = 0; i < players.Count; i++){
            Debug.Log("Assigning role " + roles[i] + " to player " + playerIds[i]);
            players[i].GetComponent<Player>().SetRoleClientRPC(roles[i], playerIds[i]);
        }
    }

    

    void Update()
    {
        if(!serverGameManager)
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
        Debug.Log("Player Connected: " + u);
    }

    private async void ClientDisconnected(ulong u){
        await Task.Yield();
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        
    }

    [ClientRpc]
    void ActivateEndClientRPC(bool win){
        if(win){
            //display win message
            c.GetComponent<CanvasManager>().ShowWin();
        }
        else{
            //display lose message
            c.GetComponent<CanvasManager>().ShowLose();
        }
    }


}
