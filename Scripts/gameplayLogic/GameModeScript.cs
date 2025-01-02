using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static EmailFetcher;

public class GameModeScript : MonoBehaviour
{
    public gameObjectsScript gameObjectsScript;

    public emailNavScript emailNavScript; 
    public EmailFetcher emailFetcher;  
    public evaluationScript evaluationScript;
    public GameOverScript gameOverScript;
    public scanResultScript scanResultScript;
    public otherButtons otherbuttons;
    public emailReviewScript emailReviewScript;
    public GameSceneTransition gameSceneTransition;

    private bool isScanScreenActive = false;
    private bool isVMScreenActive = false;
    private bool isEmailContActive = true;
    private bool isWindowsMenuActive = false;
    private bool isWarningPopup = false;

    public uploadFirebase uploadFirebase;

    private void Start()
    {

        // Subscribe to the OnEmailsFetched event
        emailFetcher.OnEmailsFetched += OnEmailsFetched;
        evaluationScript.InitializeViolations(emailFetcher.GetEmails().Count);
        gameObjectsScript.gamemodetext.text = $"{PlayerPrefs.GetString("GameMode")} Mode";
    }

    public void ProceedToGame()
    {
        gameObjectsScript.loadingbuffer.SetActive(true);
        evaluationScript.InitializeViolations(emailFetcher.GetEmails().Count);

        // Check if emails are loaded before proceeding
        if (emailFetcher.GetEmails().Count > 0)
        {
            gameObjectsScript.gameScreen.SetActive(true);
            gameObjectsScript.emailCont.SetActive(true);
            gameObjectsScript.gameOverScreen.SetActive(false);

            // Pass the fetched emails to emailNavScript and display the first one
            emailNavScript.SetEmails(emailFetcher.GetEmails());
            gameOverScript.SetEmails(emailFetcher.GetEmails());
            scanResultScript.SetEmails(emailFetcher.GetEmails());
            emailReviewScript.SetEmails(emailFetcher.GetEmails());
            evaluationScript.InitializeEmailEvaluationList(emailFetcher.GetEmails().Count);
            emailNavScript.DisplayEmail(0);
        }
        else
        {
            gameObjectsScript.loadingbuffer.SetActive(false);
            Debug.LogError("Emails are not loaded yet. Please wait.");
        }
        gameObjectsScript.loadingbuffer.SetActive(false);
    }
    // When emails are fetched
    private void OnEmailsFetched()
    {
        Debug.Log("Emails fetched, ready to display.");
    }
    public void BacktoLevelsScene()
    {
        PlayerPrefs.SetInt("SkipLevelsTransition", 1);
        PlayerPrefs.Save();
        gameSceneTransition.TransitionToScene("LevelsScene");
    }

    public void goToDetection()
    {
        gameObjectsScript.gameScreen.SetActive(false);
        gameObjectsScript.detectionScreen.SetActive(true);
        if (gameObjectsScript.detectionVMScreen.activeSelf == true)
        {
            gameObjectsScript.maximizebutton.SetActive(true);
        }
        else if (gameObjectsScript.ScanresultScreen.activeSelf == true)
        {
            gameObjectsScript.maximizebutton.SetActive(true);
        }
    }

    public void backToGame()
    {
        gameObjectsScript.detectionScreen.SetActive(false);
        gameObjectsScript.gameScreen.SetActive(true);
        //gameObjectsScript.maximizebutton.SetActive(false);
    }

    public void goEmailCont()
    {
        isEmailContActive = !isEmailContActive;

        if (isEmailContActive)
        {
            gameObjectsScript.emailCont.SetActive(true);
        }
        else
        {
            gameObjectsScript.emailCont.SetActive(false);
        }
    }
    public void goScan()
    {
        isScanScreenActive = !isScanScreenActive;

        if (isScanScreenActive)
        {
            otherbuttons.SetWhatScreen("Scan");
            gameObjectsScript.detectionScanScreen.SetActive(true);
        }
        else
        {
            gameObjectsScript.detectionScanScreen.SetActive(false);
        }
    }
    public void goVM()
    {
        isVMScreenActive = !isVMScreenActive;

        if (isVMScreenActive)
        {
            otherbuttons.SetWhatScreen("VM");
            gameObjectsScript.detectionVMScreen.SetActive(true);
            //gameObjectsScript.maximizebutton.SetActive(true);
        }
        else
        {
            gameObjectsScript.detectionVMScreen.SetActive(false);
            //gameObjectsScript.maximizebutton.SetActive(false);
        }
    }
    public void cancelDetect()
    {
        gameObjectsScript.detectionScanScreen.SetActive(false);
        gameObjectsScript.detectionVMScreen.SetActive(false);
        //gameObjectsScript.Zoom_VMScreen.SetActive(false);
        gameObjectsScript.detectionGameScreen.SetActive(true);
        //gameObjectsScript.maximizebutton.SetActive(false);
    }
    public void FinishGame()
    {
        evaluationScript.CheckAllAnswers();  // Final check of all answers

        // Disable other UI elements
        gameObjectsScript.gameScreen.SetActive(false);
        gameObjectsScript.emailCont.SetActive(false);
        gameObjectsScript.evalCont.SetActive(false);
        gameObjectsScript.urlScanCont.SetActive(false);
        gameObjectsScript.vmscreenCont.SetActive(false);
        gameObjectsScript.hintDisplay.SetActive(false);

        // Show the Game Over screen
        gameObjectsScript.gameOverScreen.SetActive(true);
    }

    public void OnGameFinishButtonClick()
    {
        bool hasUnansweredMarks = gameOverScript.ShowAnswers();  // Check for unanswered marks

        if (hasUnansweredMarks)
        {
            // Show the warning popup if there are unanswered marks
            OpenWarningNoAnswer();

            // Optionally hide other UI elements related to the game
            gameObjectsScript.gameScreen.SetActive(false);
            gameObjectsScript.emailCont.SetActive(false);
            gameObjectsScript.evalCont.SetActive(false);
            gameObjectsScript.urlScanCont.SetActive(false);
            gameObjectsScript.vmscreenCont.SetActive(false);
            gameObjectsScript.hintDisplay.SetActive(false);

            // The warning popup allows the user to finish the game or cancel
            // You can add buttons that either call FinishGame or just close the warning.
        }
        else
        {
            // No unanswered marks, finish the game immediately
            OnFinishAnywayButtonClick();
        }
    }

    public void OnFinishAnywayButtonClick()
    {
        // If user chooses to finish anyway, still call ShowAnswers() and finish the game
        FinishGame();  // Proceed to finish the game
        gameOverScript.ShowAnswers();  // Ensure answers are shown even if there are unanswered marks
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.Log("No user logged in. Cant upload data analytics");
        }
        else
        {
            uploadFirebase.UploadGameData(user.UserId);
        }

        
    }

    public void ReviewEmails()
    {
        gameObjectsScript.emailReviewScreen.SetActive(true);
        gameObjectsScript.gameOverScreen.SetActive(false);
    }

    public void GoReview()
    {
        gameObjectsScript.reviewBaseScreen.SetActive(false);
        gameObjectsScript.reviewScreen.SetActive(true);
    }

    public void BackToGameOver()
    {
        gameObjectsScript.emailReviewScreen.SetActive(false);
        gameObjectsScript.gameOverScreen.SetActive(true);
    }
    public void BacktoReviewEmails()
    {

        gameObjectsScript.reviewScreen.SetActive(false);
        gameObjectsScript.reviewBaseScreen.SetActive(true);
    }

    public void OpenWindowsMenu()
    {
        isWindowsMenuActive = !isWindowsMenuActive;

        if (isWindowsMenuActive)
        {
            gameObjectsScript.windowsmenu.SetActive(true);
        }
        else
        {
            gameObjectsScript.windowsmenu.SetActive(false);
        }
    }

    public void OpenWarningNoAnswer()
    {
        isWarningPopup = !isWarningPopup;
        if (isWarningPopup)
        {
            gameObjectsScript.warningPopup.SetActive(true);
        }
        else
        {
            gameObjectsScript.warningPopup.SetActive(false);
        }
    }

}
