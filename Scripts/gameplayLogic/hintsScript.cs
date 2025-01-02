using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class hintsScript : MonoBehaviour
{
    public gameObjectsScript gameObjectsScript;
    public bool isPressed = false;  
    private string theHint = "";    

    private void Start()
    {
        gameObjectsScript.hintDisplay.SetActive(false);  // Make sure the hint display is initially hidden
    }

    // Call this method to update the hint content
    public void SetHint(string hint)
    {
        theHint = hint;                  // Set the hint text
    }

    // This method toggles the hint display on or off
    public void displayHint()
    {
        if (isPressed)
        {
            gameObjectsScript.hintDisplay.SetActive(false);  // Hide the hint display
        }
        else
        {
            gameObjectsScript.hintDisplay.SetActive(true);   // Show the hint display
            gameObjectsScript.hintText.text = theHint;       // Update the displayed hint text
        }

        isPressed = !isPressed;  // Toggle the pressed state
    }
}
