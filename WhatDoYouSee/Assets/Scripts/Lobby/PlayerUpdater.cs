using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using TMPro;
using UnityEngine;

public class PlayerUpdater : MonoBehaviour
{
    [SerializeField] TestLobby testLobby;
    public float refreshTime = 2f;
    private float refreshTimer = 2f;
    List<string> displayedPlayers = new List<string>();
    public GameObject playerDataPanelPrefab;
    public TextMeshProUGUI lobbyCode;
    // Start is called before the first frame update
    void Start()
    {
        lobbyCode.text = "Lobby Code: " + testLobby.GetLobbyCode();
    }

    // Update is called once per frame
    void Update()
    {
        refreshTimer += Time.deltaTime;
        if (refreshTimer >= refreshTime)
        {
            refreshTimer = 0f;
            RefreshPlayerList();
        }
    }

    void RefreshPlayerList()
    {
        testLobby.RefreshLobby();
        List<string> usernames = new List<string>();
        if (testLobby != null)
        {
            usernames = testLobby.GetLobbyUsernames();
            //destroy all playerdatapanels
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        if (usernames.Count > 0)
        {
            foreach (string username in usernames)
            {
                if (!displayedPlayers.Contains(username))
                {
                    displayedPlayers.Add(username);
                    Debug.Log("New player joined: " + username);
                }
            }
            DisplayPlayers();
        }
        else
        {
            Debug.Log("No players in the lobby.");
        }
        
    }

    void DisplayPlayers(){
        //Create a prefab of a playerdatapanel and fill in the child username text with username
        //Instantiate the prefab and set the username text
        foreach (string username in displayedPlayers)
        {
            GameObject playerDataPanel = Instantiate(playerDataPanelPrefab, transform);
            TextMeshProUGUI usernameText = playerDataPanel.GetComponentInChildren<TextMeshProUGUI>();
            usernameText.text = username;
        }
        
    }
}
