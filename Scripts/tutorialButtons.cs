using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tutorialButtons : MonoBehaviour
{
    public Scrollbar tutorialScrollbar; // Reference to the tutorial scrollbar
    public TutorialPageManager tutorialPageManager;
    public GameObject tutorials;

    void Start()
    {
        // Example: Reset the tutorial scrollbar on start
        ResetTutorialScrollbar();
    }

    public void ResetTutorialScrollbar()
    {
        if (tutorialScrollbar != null)
        {
            tutorialScrollbar.value = 1f; // Reset to the top/left
            Debug.Log("Tutorial scrollbar reset to the top/left.");
        }
        else
        {
            Debug.LogWarning("Tutorial scrollbar reference is null. Please assign it in the Inspector.");
        }
    }

    public void OpenTutorialPage()
    {
        tutorials.SetActive(true);
    }

    public void CloseTutorialPage()
    {
        tutorials.SetActive(false);
    }

    public void GoEmails()
    {
        OpenTutorialPage();
        tutorialPageManager.SetCurrentList("Emails");
    }

    public void GoEmailEval()
    {
        OpenTutorialPage();
        tutorialPageManager.SetCurrentList("EmailEvaluations");
    }

    public void GoScanURL()
    {
        OpenTutorialPage();
        tutorialPageManager.SetCurrentList("ScanURL");
    }

    public void GoVM()
    {
        OpenTutorialPage();
        tutorialPageManager.SetCurrentList("VM");
    }
    public void GoHints()
    {
        OpenTutorialPage();
        tutorialPageManager.SetCurrentList("Hints");
    }

    public void GoMenu()
    {
        OpenTutorialPage();
        tutorialPageManager.SetCurrentList("Menu");
    }

    public void GoGameOver()
    {
        OpenTutorialPage();
        tutorialPageManager.SetCurrentList("GameOver");
    }

    public void GoEmailReview()
    {
        OpenTutorialPage();
        tutorialPageManager.SetCurrentList("EmailReview");
    }

}
