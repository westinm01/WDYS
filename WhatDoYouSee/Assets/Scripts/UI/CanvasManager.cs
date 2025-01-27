using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject WinPanel;
    public GameObject LosePanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShowWin(){
        WinPanel.SetActive(true);
        
    }
    public void ShowLose(){
        LosePanel.SetActive(true);
    }
}
