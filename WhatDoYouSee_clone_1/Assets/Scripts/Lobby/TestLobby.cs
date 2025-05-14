using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using TMPro;
using UnityEngine;


public class TestLobby : MonoBehaviour
{

    private Lobby hostLobby;
    private float heartbeatTimer = 0f;
    [SerializeField] private Canvas canvas;

    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in: " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // Update is called once per frame
    void Update()
    {
        HandleLobbyHeartbeat();
    }

    private async void HandleLobbyHeartbeat()
    {
        //ping the server every 15 seconds
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f) // 15 seconds
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    public async void CreateLobby(){
        try{
            string lobbyName = "TestLobby";
            int maxPlayers = 2;
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer()
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
            Debug.Log("Lobby created: " + lobby.Name + " " + lobby.MaxPlayers + " (" + lobby.LobbyCode + ")");

            TextMeshProUGUI lobbyCodeText = canvas.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            lobbyCodeText.gameObject.SetActive(true);
            lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;
        }
        catch (LobbyServiceException e){
            Debug.Log("Error creating lobby: " + e.Message);
        }
    }


    public async void JoinLobby(string lobbyCode){
        try{
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            hostLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            Debug.Log("Joined lobby: " + hostLobby.Name + " "  + hostLobby.MaxPlayers+ " (" + hostLobby.LobbyCode + ")");
            //print each palyer name in the lobby
            Debug.Log("Players in lobby: ");
            foreach (var player in hostLobby.Players)
            {
                Debug.Log("Player: " + player.Data["Username"].Value);
            }

        }
        catch (LobbyServiceException e){
            Debug.Log("Error joining lobby: " + e.Message);
        }
    }

    private Player GetPlayer(){
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>()
            {
                {"Username", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerPrefs.GetString("Username", "Guest")) }
            },
        };
    }
}
