using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyReset : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject resetPanel;
    private bool resetOn = false;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = gameObject.transform.parent;
        resetPanel = gameObject.transform.GetChild(0).gameObject;
        resetPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if((resetOn == false) && (playerTransform.position.y < 50  )){ //|| playerTransform.rotation != Quaternion.Euler(90,0,0)
            ShowReset();
            resetOn = true;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            resetOn = false;
            resetPanel.SetActive(false);
        }
    }

    void ShowReset(){
        resetPanel.SetActive(true);
    }
}
