using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpdater : MonoBehaviour
{
    [SerializeField] TestLobby testLobby;
    public float refreshTime = 2f;
    private float refreshTimer = 2f;
    List<Player> lobbyPlayers = new List<Player>();
    List<string> displayedPlayers = new List<string>();
    public GameObject playerDataPanelPrefab;
    public TextMeshProUGUI lobbyCode;

    public List<Sprite> roleSprites = new List<Sprite>();

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
        if (testLobby != null)
        {
            lobbyPlayers = testLobby.GetLobbyPlayers();
            //destroy all playerdatapanels
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
        if (lobbyPlayers.Count > 0)
        {
            List<string> usernames = GetData("Username");
            List<string> roles = GetData("Role");
            //might make a value for time joined to sort by that always.
            foreach (string username in usernames)
            {
                if (!displayedPlayers.Contains(username))
                {
                    displayedPlayers.Add(username);
                    Debug.Log("New player joined: " + username);
                }
            }
            DisplayPlayers(usernames, roles);
        }
        else
        {
            Debug.Log("No players in the lobby.");
        }
        
    }

    void DisplayPlayers(List<string> usernames, List<string> roles){
        //Create a prefab of a playerdatapanel and fill in the child username text with username
        //Instantiate the prefab and set the username text

        for(int i = 0; i < lobbyPlayers.Count; i++)
        {
            //find the player in the lobbyPlayers list
            GameObject playerDataPanel = Instantiate(playerDataPanelPrefab, transform);

            TextMeshProUGUI usernameText = playerDataPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            usernameText.text = usernames[i];


            Image roleImage = playerDataPanel.transform.GetChild(1).GetComponent<Image>();
            int role = DetermineRoleInt(roles[i]);
            roleImage.sprite = roleSprites[role];

        }
    }
        
    

    int DetermineRoleInt(string role)
    {
        //determine the role int based on the role string
        switch (role)
        {
            case "Flash":
                return 0;
            case "Cart":
                return 1;
            default:
                return -1;
        }
    }

    public List<string> GetData(string key){
        List<string> values = new List<string>();

        foreach (var player in lobbyPlayers)
        {   
            values.Add(player.Data[key].Value);
        }
        return values;
    }

}
