using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpherePosition : MonoBehaviour
{
    // Z-axis distances (depth from the participant)
    public float[] distances = { 30, 50, 70 }; // Define the distances you want
    // Y-axis heights (vertical position)
    public float[] heights = { 5, 10, 15 }; // Heights you want

    private Vector3 startingPosition;

    // Number of trials
    public int trials = 18;
    private int currentTrial = 0;

    // Reference to the MeshRenderer for controlling shadows
    private MeshRenderer sphereRenderer;

    void Start()
    {
        // Store the initial position of the sphere
        startingPosition = transform.position;

        // Start the first trial
        PlaceSphereRandomly();

        // Get the MeshRenderer component
        sphereRenderer = GetComponent<MeshRenderer>();
    }

    void PlaceSphereRandomly()
    {
        if (currentTrial < trials)
        {
            // Randomly select a distance and a height
            float selectedDistance = distances[Random.Range(0, distances.Length)];
            float selectedHeight = heights[Random.Range(0, heights.Length)];

            // Move the sphere to a new random position based on the Y (height) and Z (distance)
            transform.position = new Vector3(startingPosition.x, selectedHeight, selectedDistance);

            // Randomly decide if the sphere should cast a shadow
            bool castShadow = Random.Range(0, 2) == 0; // 50% chance for shadow on or off
            SetShadow(castShadow);

            // Update trial count
            currentTrial++;
        }
        else
        {
            Debug.Log("All trials completed.");
        }
    }

    void SetShadow(bool enable)
    {
        if (enable)
        {
            // Enable shadow casting
            sphereRenderer.shadowCastingMode = ShadowCastingMode.On;
        }
        else
        {
            // Disable shadow casting
            sphereRenderer.shadowCastingMode = ShadowCastingMode.Off;
        }
    }

    // Call this method whenever the trial is supposed to advance (e.g., after user response)
    public void NextTrial()
    {
        PlaceSphereRandomly();
    }
}
