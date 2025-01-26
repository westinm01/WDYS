using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float zoomSpeed = 10f; // Speed of zooming
    public float minZoom = 20f;  // Minimum field of view
    public float maxZoom = 60f;  // Maximum field of view

    private Camera cam;
    CreateCart c;

    public float sensitivity = 100f; // Mouse sensitivity
    public bool lockCursor = true;   // Option to lock the cursor
    public bool invertY = false;     // Option to invert vertical movement

    private float yaw = 0f;          // Horizontal rotation
    private float pitch = 90f;        // Vertical rotation

    void Start()
    {
        // Get the Camera component
        cam = gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Camera>();
        c = gameObject.GetComponent<CreateCart>();

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        // Get scroll wheel input
        if(c.isCart)
        { 
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput != 0f)
            {
                // Adjust the camera's field of view (FOV)
                cam.fieldOfView -= scrollInput * zoomSpeed;

                // Clamp the FOV to stay within the specified range
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
            }
             // Check if the right mouse button is held down
            if (Input.GetMouseButton(1))
            {
                // Get mouse input
                float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

                // Update yaw (horizontal) and pitch (vertical)
                yaw += mouseX;
                pitch -= invertY ? -mouseY : mouseY;

                // Clamp pitch to avoid flipping (optional)
                pitch = Mathf.Clamp(pitch, -90f, 90f);

                // Apply the updated rotation
                transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
            }
        }

        
    }
}
