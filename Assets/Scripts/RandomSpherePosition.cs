using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RandomSpherePosition : MonoBehaviour
{
    
    private List<TrialCondition> conditions = new List<TrialCondition>();
    void GenerateTrialConditions()
    {
        float[] distances = { 3f, 5f, 7f };
        float[] heights = { 0f, 0.5f, 1f };
        bool[] shadows = { true, false };

        foreach (var distance in distances)
        {
            foreach (var height in heights)
            {
                foreach (var shadow in shadows)
                {
                    conditions.Add(new TrialCondition(distance, height, shadow));
                }
            }
        }
        Shuffle(conditions);
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    // Z-axis distances (depth from the participant)
    public float[] distances = { 3, 5, 7 }; // Define the distances you want
    // Y-axis heights (vertical position)
    public float[] heights = { 0.5f, 1.0f, 1.5f }; // Heights you want

    private Vector3 startingPosition;
    

    // Number of trials
    public int trials = 18;
    private int currentTrial = 0;

    // Reference to the MeshRenderer for controlling shadows
    private MeshRenderer sphereRenderer;
    private bool isTriggerPressed = false;

    void Start()
    {
        // Store the initial position of the sphere
        startingPosition = transform.position;
    }

    void Update()
    {
        // Check if the trigger button is pressed (example for XR controller)
        if (Input.GetButtonDown("Fire1"))
        {
            // If the trigger has been pressed, place the sphere randomly and reset the flag
            if (!isTriggerPressed)
            {
                PlaceSphereRandomly();
                isTriggerPressed = true;  // Ensure the function is only called once until the next trigger press
            }
        }

        // Optional: Reset the flag if you want the function to be called again on the next trigger press
        // You can adjust this based on your needs (e.g., for trials or repeated tests)
    }

    void PlaceSphereRandomly()
    {
        if (currentTrial < trials)
        {
            TrialCondition condition = conditions[currentTrial];
            // Randomly select a distance and a height
            float selectedDistance = distances[Random.Range(0, distances.Length)];
            float selectedHeight = heights[Random.Range(0, heights.Length)];

            // Move the sphere to a new random position based on the Y (height) and Z (distance)
            transform.position = new Vector3(startingPosition.x, selectedHeight, selectedDistance);

            // Randomly decide if the sphere should cast a shadow
            bool castShadow = Random.Range(0, 2) == 0; // 50% chance for shadow on or off

            // Update trial count
            currentTrial++;
            isTriggerPressed = false;
        }
        else
        {
            Debug.Log("All trials completed.");
        }
    }


    // Call this method whenever the trial is supposed to advance (e.g., after user response)
    public void NextTrial()
    {
        PlaceSphereRandomly();
    }
}
