using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoinLobbyButton : MonoBehaviour
{
    [SerializeField]TestLobby testLobby;
    [SerializeField]TextMeshProUGUI lobbyCodeText;

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ClickToJoinLobby);
        }
        else
        {
            Debug.LogError("Button component not found on this GameObject.");
        }
    }
    
    //make an onclick even for the button that takes the string from lobbyIDText and calls the JoinLobby function in TestLobby.cs
    public void ClickToJoinLobby()
    {
        string lobbyCode = lobbyCodeText.text;
        //only get first 6 characters
        if (lobbyCode.Length > 6)
        {
            lobbyCode = lobbyCode.Substring(0, 6);
        }
        testLobby.JoinLobby(lobbyCode);
    }
}
