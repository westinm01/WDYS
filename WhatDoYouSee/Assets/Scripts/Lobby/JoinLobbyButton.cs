using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoinLobbyButton : MonoBehaviour
{
    [SerializeField]TestLobby testLobby;
    [SerializeField]TextMeshProUGUI lobbyCodeText;
    
    //make an onclick even for the button that takes the string from lobbyIDText and calls the JoinLobby function in TestLobby.cs
    public void OnClick()
    {
        string lobbyCode = lobbyCodeText.text;
        testLobby.JoinLobby(lobbyCode);
    }
}
