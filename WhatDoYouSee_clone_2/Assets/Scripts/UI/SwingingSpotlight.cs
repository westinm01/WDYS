using System.Collections;
using UnityEngine;

public class SwingingSpotlight : MonoBehaviour
{
    public float swingIntensity = 15f; // Maximum angle of swing
    public float swingSpeed = 2f;     // Base speed of swinging
    public float flickerIntensity = 1f; // Intensity of flickering movement
    public float flickerSpeed = 10f;    // Speed of flickering

    private Quaternion initialRotation;
    private Light spotlight;
    
    void Start()
    {
        // Store the initial rotation of the spotlight
        initialRotation = transform.rotation;

        // Get the Light component, if there is one
        spotlight = GetComponent<Light>();
        if (spotlight == null)
        {
            Debug.LogWarning("No Light component found on the GameObject.");
        }
    }

    void Update()
    {
        SwingLight();
        //FlickerLight();
    }

    void SwingLight()
    {
        // Calculate swing angles based on time and Perlin noise
        float swingAngleX = Mathf.Sin(Time.time * swingSpeed) * swingIntensity;
        float swingAngleY = Mathf.Cos(Time.time * swingSpeed * 0.8f) * swingIntensity;

        // Apply the swing rotation
        Quaternion swingRotation = Quaternion.Euler(swingAngleX, swingAngleY, 0f);
        transform.rotation = initialRotation * swingRotation;
    }

    void FlickerLight()
    {
        if (spotlight != null)
        {
            // Use Perlin noise to create a subtle flicker effect
            float flicker = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f) * flickerIntensity;
            spotlight.intensity = Mathf.Lerp(spotlight.intensity, flicker, Time.deltaTime);
        }
    }
}