using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;

public class otherButtons : MonoBehaviour
{
    private string whatScreen = "";

    //public detectionScript detectionScript;
    public gameObjectsScript gameObjectsScript;

    void Start()
    {
        OpenCoverpanel();
    }

    // Method to maximize the email screen
    public void MaximizeScreen()
    {
        // Copy the email content from normal view to fullscreen view
        gameObjectsScript.fullscreenSenderEmailText.text = gameObjectsScript.senderEmailDisplay.text;
        gameObjectsScript.fullscreenEmailBodyText.text = gameObjectsScript.emailBodyDisplay.text;
        gameObjectsScript.fullscreenEmailLinkText.text = gameObjectsScript.emailLinkDisplay.text;

        // Switch to fullscreen email view
        gameObjectsScript.gameScreen.SetActive(false);
        gameObjectsScript.emailScreen.SetActive(true);
    }

    // Method to minimize the email screen and go back to the game screen
    public void MinimizeScreen()
    {
        gameObjectsScript.emailScreen.SetActive(false);
        gameObjectsScript.gameScreen.SetActive(true);
    }
    public void MaximizeScanResult()
    {
        gameObjectsScript.resultScreen.SetActive(false);
        gameObjectsScript.detectionButtons.SetActive(false);
        //gameObjectsScript.zoom_resultScreen.SetActive(true);
    }
    public void MinimizeScanResult()
    {
        //gameObjectsScript.zoom_resultScreen.SetActive(false);
        gameObjectsScript.detectionButtons.SetActive(true);
        gameObjectsScript.resultScreen.SetActive(true);
    }
    public void MaximizeVMScreen()
    {
        gameObjectsScript.detectionVMScreen.SetActive(false);
        //gameObjectsScript.Zoom_VMScreen.SetActive(true);
        gameObjectsScript.maximizebutton.SetActive(false);
        gameObjectsScript.backToEmailsButton.SetActive(false);
        gameObjectsScript.minimizebutton.SetActive(true);
    }
    public void MinimizeVMScreen()
    {
        gameObjectsScript.detectionButtons.SetActive(true);
        //gameObjectsScript.Zoom_VMScreen.SetActive(false);
        gameObjectsScript.detectionVMScreen.SetActive(true);
        gameObjectsScript.backToEmailsButton.SetActive(true);
        gameObjectsScript.maximizebutton.SetActive(true);
        gameObjectsScript.minimizebutton.SetActive(false);
    }
    public void SetWhatScreen(string screen)
    {
        whatScreen = screen;
    }

    public void Maximize()
    {
        if (whatScreen == "Scan")
        {
            MaximizeScanResult();
        }
        else if (whatScreen == "VM")
        {
            MaximizeVMScreen();
        }
    }

    public void CloseCoverpanel()
    {
        gameObjectsScript.coverPanel.SetActive(false);
        gameObjectsScript.gameWelcomeScreen.SetActive(false);
    }

    public void OpenCoverpanel()
    {
        gameObjectsScript.coverPanel.SetActive(true);
        gameObjectsScript.gameWelcomeScreen.SetActive(true);
    }

    public void OpenPanelOnly()
    {
        gameObjectsScript.coverPanel.SetActive(true);
    }

    

}
