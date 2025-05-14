using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float zoomSpeed = 100f; // Speed of zooming
    public float minZoom = 20f;  // Minimum field of view
    public float maxZoom = 60f;  // Maximum field of view

    private Camera cam;
    PlayerObject p;

    public float sensitivity = 100f; // Mouse sensitivity
    public bool lockCursor = true;   // Option to lock the cursor
    public bool invertY = false;     // Option to invert vertical movement

    private Vector3 originalPosition;
    private Vector3 originalRotation;


    void Start()
    {
        // Get the Camera component
        cam = gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Camera>();
        p = gameObject.GetComponent<PlayerObject>();
        StartCoroutine(WaitForRoles());
        originalPosition = transform.position;
        originalRotation = transform.eulerAngles;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if(p.role == 0)
        {
           //get culling mask and deselect layer 3
            cam.cullingMask = ~(1 << 3); //might not even be necessary...

        }

    }

    IEnumerator WaitForRoles()
    {
        while(p.role == -1){
            yield return new WaitForSeconds(1f);
        }
    }

    void Update()
    {
        // Get scroll wheel input
        if(p.role == 0)
        { 
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput != 0f)
            {
                //move the character forward
                //get grandparent
                
                Transform t = gameObject.transform; 
                float currY = t.position.y;
                
                t.position += t.forward * scrollInput * zoomSpeed * currY * Time.deltaTime;
            }
             // Check if the right mouse button is held down
            if (Input.GetMouseButton(1))
            {
                // Get mouse input
                float mouseX = Input.GetAxis("Mouse X") * -1* sensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

                // Update yaw (horizontal) and pitch (vertical)
                Vector3 rotate = new Vector3(mouseY, mouseX, 0f);
                transform.eulerAngles -= rotate;
            }

            //checkif the player presses the R key
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetCamera();
            }
        }
    }

    //create a function, ResetCamera, that interpolates the camera back to its original position and rotation
    public void ResetCamera()
    {
        if(p.role == 0){
            StartCoroutine(ResetCameraCoroutine());
        }
        
    }

    IEnumerator ResetCameraCoroutine()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, originalPosition, t);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, originalRotation, t);
            yield return null;
        }
    }

    public void setOriginalPosition(Vector3 pos)
    {
        originalPosition = pos;
    }

    public void setOriginalRotation(Vector3 rot)
    {
        originalRotation = rot;
    }

}




