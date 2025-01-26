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
    private uint timeRemainingInt;
    private uint lastTimeRemainingInt;
    private bool serverGameManager = false;


    //gameobject array for players of size 2
    public List<GameObject> players = new List<GameObject>(2);
        public List<TMPro.TextMeshProUGUI> timerTexts = new List<TMPro.TextMeshProUGUI>(2);

    public override void OnNetworkSpawn()
    {
        if(!IsServer)
        {
            enabled = false;
            return;
        }
        
        serverGameManager = true;
        timeRemaining = timeLimit;
        timeRemainingInt = (uint)timeRemaining;
        lastTimeRemainingInt = timeRemainingInt;
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
        
        
        
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
        timeRemainingInt = (uint)timeRemaining;
        if(timeRemainingInt != lastTimeRemainingInt)
        {
            lastTimeRemainingInt = timeRemainingInt;
            //send message to clients
            UpdateCanvasTime(timeRemainingInt);
        }

    }

    void UpdateCanvasTime(uint newTime){
        //find players and focus on their canvas object. Locate the time text and update it.
        string timeString = newTime.ToString();
        foreach(TMPro.TextMeshProUGUI text in timerTexts){
            text.text = timeString;
        }
        
    }

    private void ClientConnected(ulong u){
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        foreach(GameObject player in players){
            timerTexts.Add(player.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>());
        }
    }

    private async void ClientDisconnected(ulong u){
        await Task.Yield();
        players = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        foreach(GameObject player in players){
            timerTexts.Add(player.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>());
        }
    }


}
