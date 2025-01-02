using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static evaluationScript;

public class EvaluateFormScript : MonoBehaviour
{
    public gameObjectsScript gameObjectsScript;    
    public bool isPressed = false;
    private int currentEmailIndex;
    public evaluationScript evaluationScript;

    private void Start()
    {
        gameObjectsScript.evaluationDisplay.SetActive(false); 
    }

    public void SetCurrentEmailIndex(int emailIndex)
    {
        currentEmailIndex = emailIndex;
    }

    public void displayEvaluationForm()
    {
        if (isPressed)
        {
            gameObjectsScript.evaluationDisplay.SetActive(false); 
        }
        else
        {
            gameObjectsScript.evaluationDisplay.SetActive(true);   
        }

        isPressed = !isPressed;  
    }
}
