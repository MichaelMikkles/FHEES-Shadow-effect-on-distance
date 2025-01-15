using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    public GameObject startScreen; // Drag the start screen canvas here
    public GameObject experimentObjects; // The objects or UI elements for the experiment

    void Start()
    {
        // Ensure the start screen is visible at the beginning
        startScreen.SetActive(true);
        experimentObjects.SetActive(false); // Hide the experiment objects initially

    }
    void StartExperiment()
    {
        // Hide the start screen and show the experiment objects when the button is pressed
        startScreen.SetActive(false);
        experimentObjects.SetActive(true);
    }
}
