using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public int walkPower = 5;
    Rigidbody rb;
    
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get input from keyboard
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right Arrow keys
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down Arrow keys

        // Calculate movement direction
        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;

        // Update Rigidbody velocity
        rb.velocity = new Vector3(move.x * walkPower, rb.velocity.y, move.z * walkPower);
    }
}
