using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR; // For VR input

public class ExperimentController : MonoBehaviour
{
    public GameObject experimentObjects; // The objects or UI elements for the experiment
    public GameObject blackScreen;       // The black screen overlay
    public GameObject startScreen;       // The first window when execute
    public RandomSpherePosition sphereRandomizer; // Reference to the sphere randomizer script


    private bool isBlackScreenActive = false;
    private bool experimentStarted = false;
    private int runs = 0;

    void Start()
    {
        // Initially hide the black screen and experiment objects
        blackScreen.SetActive(false);
        experimentObjects.SetActive(false);
        experimentStarted = false;
        startScreen.SetActive(true);
    }

    void Update()
    {
        // Wait for the VR trigger to start the experiment
        if (!experimentStarted)
        {
            // Check for the VR controller trigger (this example uses the primary trigger for XR devices)
            if (Input.GetButtonDown("Fire1") )
            {
                startScreen.SetActive(false);
                StartExperiment();  // Start the experiment when trigger is pressed
            }
        }

        // Check if the black screen is active and wait for the user to press the trigger again
        if (isBlackScreenActive && experimentStarted)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                runs += 1; // Finish current run and go to the next one
                TurnOffBlackScreen(); // Turn off the black screen when the trigger is pressed
            }
        }
    }

    void StartExperiment()
    {
        // Start a coroutine that controls the experiment timing
        experimentStarted = true;
        StartCoroutine(ShowExperimentThenBlackScreen());
    }

    IEnumerator ShowExperimentThenBlackScreen()
    {
        // Show experiment objects for 10 seconds
        experimentObjects.SetActive(true);
        yield return new WaitForSeconds(10f);

        // Turn the screen black
        TurnOnBlackScreen();
    }

    void TurnOnBlackScreen()
    {
        // Hide experiment objects and activate the black screen
        experimentObjects.SetActive(false);
        blackScreen.SetActive(true);
        isBlackScreenActive = true;
    }

    void TurnOffBlackScreen()
    {
        // Disable the black screen and proceed
        blackScreen.SetActive(false);
        isBlackScreenActive = false;

        if (runs <= 17)
        {
            // Call the randomizer to place the sphere in a new position before starting the next trial
            sphereRandomizer.NextTrial();
            // Start a new experiment trial after turning off the black screen
            Start();
        }
    }
}
