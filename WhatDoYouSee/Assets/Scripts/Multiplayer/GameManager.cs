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


    //gameobject array for players of size 2
    public List<GameObject> players = new List<GameObject>(2);
    //public List<TMPro.TextMeshProUGUI> timerTexts = new List<TMPro.TextMeshProUGUI>(2);
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
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
        
        timerText = c.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        
    }
    

    void Start()
    {
        
    }

    
    void Update()
    {
        if(!serverGameManager)
        {
            return;
        }
        if(players.Count <= 1)
        {
            return;
        }
        timeRemaining -= Time.deltaTime;
        timeRemainingInt.Value = (uint)timeRemaining;
        if(timeRemainingInt.Value != lastTimeRemainingInt)
        {
            lastTimeRemainingInt = timeRemainingInt.Value;
            //send message to clients
            UpdateCanvasTime(timeRemainingInt.Value);
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
        
    }

    private async void ClientDisconnected(ulong u){
        await Task.Yield();
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        
    }


}
