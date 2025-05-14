using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public List<GameObject> menuPanels;
    private int currentPanelIndex = 0;
    public static MenuManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        //check if datamanager is first time
        if (DataManager.Instance.firstTime)
        {
            //show the username panel
            menuPanels[currentPanelIndex].SetActive(true);
        }
        else
        {
            //show the main menu panel
            menuPanels[currentPanelIndex].SetActive(false);
            currentPanelIndex = 1;
            menuPanels[currentPanelIndex].SetActive(true);
        }
    }

    public void NextPanel()
    {
        //hide current panel
        menuPanels[currentPanelIndex].SetActive(false);
        //increment current panel index
        currentPanelIndex++;
        //if current panel index is greater than the number of panels, set it to 0
        if (currentPanelIndex >= menuPanels.Count)
        {
            currentPanelIndex = 0;
        }
        //show next panel
        menuPanels[currentPanelIndex].SetActive(true);
    }

    public void SetCurrentPanel(int index)
    {
        //hide current panel
        menuPanels[currentPanelIndex].SetActive(false);
        //set current panel index
        currentPanelIndex = index;
        //show next panel
        menuPanels[currentPanelIndex].SetActive(true);
    }
}
