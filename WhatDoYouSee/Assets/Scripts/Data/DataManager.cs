using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//make this a singleton
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public bool firstTime = true;
    public bool testFirstTime = true;
    public string username;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        username = PlayerPrefs.GetString("Username", "NO");
        if (username != "NO" && !testFirstTime){
            firstTime = false;
        }
        else{
            Debug.Log("First time playing, ready to receive username.");
        }
    }


    public int SaveUsername(string username){
        // Save the username to PlayerPrefs
        try{
            //ensure player name has 3-12 characters
            if (username.Length < 3 || username.Length > 12){
                Debug.LogError("Username must be between 3 and 12 characters.");
                return 1;
            }
            //ensure player name has no special characters
            if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9]+$")){
                Debug.LogError("Username can only contain letters and numbers.");
                return 2;
            }
            PlayerPrefs.SetString("Username", username);
            PlayerPrefs.Save();
            this.username = username;
            return 0;
        }
        catch(System.Exception e){
            Debug.LogError("Error saving username: " + e.Message);
            return -1;
        }
    }
}
