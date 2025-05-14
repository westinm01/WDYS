using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SaveUsernameButton : MonoBehaviour
{
    string username;
    List<string> responses = new List<string>();
    [SerializeField] TextMeshProUGUI errorText;
    [SerializeField] TMP_InputField inputField;

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(SaveUsername);
            responses.Add("Username saved successfully.");
            responses.Add("Error: username must have between 3 and 12 characters.");
            responses.Add("Error: username must not contain special characters.");
        }
        else
        {
            Debug.LogError("Button component not found on this GameObject.");
        }
    }
    
    //make an onclick even for the button that takes the string from lobbyIDText and calls the JoinLobby function in TestLobby.cs
    public void SaveUsername()
    {
        //call datamanager to save the username
        //get the username from the input field
        
        if (inputField != null)
        {
            username = inputField.text;
            //save the username to the data manager
            int response = DataManager.Instance.SaveUsername(username);
            if (response == 0)
            {
                Debug.Log("Username saved successfully.");
                MenuManager.Instance.SetCurrentPanel(1);
            }
            else
            {
                Debug.LogError("Failed to save username.");
                //handle errors here
                errorText.text = responses[response];

            }
        }
        else
        {
            Debug.LogError("InputField component not found in children of this GameObject.");
        }
    }
}
