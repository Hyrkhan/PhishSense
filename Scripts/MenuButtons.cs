using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using TMPro;
using static UnityEngine.ParticleSystem;
using UnityEngine.Analytics;

public class MenuButtons : MonoBehaviour
{
    public FirebaseAuth auth;
    public FirebaseUser user;

    public GameObject warningPopup;
    public TMP_Text warningtext;

    public GameObject loadingbuffer;
    public LevelLoader levelLoader;
    void Start()
    {
        // Initialize Firebase Authentication
        auth = FirebaseAuth.DefaultInstance;
    }

    private bool IsInternetAvailable()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
    public void BacktoLevelsScene()
    {
        PlayerPrefs.SetInt("SkipLevelsTransition", 1);
        PlayerPrefs.Save();
        levelLoader.crossfadeCont.SetActive(true);
        levelLoader.LoadSceneAndTransition2("LevelsScene");
    }

    public void LevelsScene()
    {
        levelLoader.circlewipeCont.SetActive(true);
        levelLoader.LoadSceneAndTransition("LevelsScene");
    }
    public void QuitGame()
    {
        Application.Quit();

        // Reset the "IsGameJustStarted" flag so it triggers on the next app launch
        PlayerPrefs.SetInt("IsGameJustStarted", 1);

        // Optionally reset other PlayerPrefs if needed
        PlayerPrefs.Save(); // Save the changes to ensure they persist
    }

    public void GoMainMenu()
    {
        levelLoader.circlewipeCont.SetActive(true);
        levelLoader.LoadSceneAndTransition("MainMenu");
    }

    public void BacktoMainMenu()
    {
        PlayerPrefs.SetInt("SkipLevelsTransition", 2);
        PlayerPrefs.SetInt("SkipIntroVideo", 1);
        PlayerPrefs.Save();
        levelLoader.reverseCircleWipe.SetActive(true);
        levelLoader.LoadBackSceneAndTransition("MainMenu");
    }
    public void TutorialModeScene()
    {
        loadingbuffer.SetActive(false);
        PlayerPrefs.SetInt("SkipLevelsTransition", 1);
        PlayerPrefs.Save();
        levelLoader.crossfadeCont.SetActive(true);
        levelLoader.LoadSceneAndTransition2("TutorialMode");
    }
    public void EasyModeScene()
    {
        loadingbuffer.SetActive(true);
        if (IsInternetAvailable())
        {
            PlayerPrefs.SetString("GameMode", "Easy");
            loadingbuffer.SetActive(false);
            PlayerPrefs.SetInt("SkipLevelsTransition", 1);
            PlayerPrefs.Save();
            levelLoader.crossfadeCont.SetActive(true);
            levelLoader.LoadSceneAndTransition2("GAME");
        }
        else
        {
            loadingbuffer.SetActive(false);
            ShowWarningPopup("No internet connection. Please connect to proceed.");
        }
    }
    public void NormalModeScene()
    {
        loadingbuffer.SetActive(true);
        if (IsInternetAvailable())
        {
            PlayerPrefs.SetString("GameMode", "Normal");
            loadingbuffer.SetActive(false);
            PlayerPrefs.SetInt("SkipLevelsTransition", 1);
            PlayerPrefs.Save();
            levelLoader.crossfadeCont.SetActive(true);
            SceneManager.LoadScene("GAME");
        }
        else
        {
            loadingbuffer.SetActive(false);
            ShowWarningPopup("No internet connection. Please connect to proceed.");
        }
    }
    public void HardModeScene() 
    {
        loadingbuffer.SetActive(true);
        if (IsInternetAvailable())
        {
            PlayerPrefs.SetString("GameMode", "Hard");
            loadingbuffer.SetActive(false);
            PlayerPrefs.SetInt("SkipLevelsTransition", 1);
            PlayerPrefs.Save();
            levelLoader.crossfadeCont.SetActive(true);
            SceneManager.LoadScene("GAME");
        }
        else
        {
            loadingbuffer.SetActive(false);
            ShowWarningPopup("No internet connection. Please connect to proceed.");
        }
    }
    public void BacktoAuthenticate()
    {
        PlayerPrefs.SetInt("SkipLevelsTransition", 2);
        PlayerPrefs.Save();
        levelLoader.reverseCircleWipe.SetActive(true);
        levelLoader.LoadBackSceneAndTransition("Authentication");
    }
    public void GoLogin()
    {
        loadingbuffer.SetActive(true);
        if (IsInternetAvailable())
        {
            levelLoader.circlewipeCont.SetActive(true);
            loadingbuffer.SetActive(false);
            levelLoader.LoadSceneAndTransition("LoginScene");
        }
        else
        {
            loadingbuffer.SetActive(false);
            ShowWarningPopup("No internet connection. Please connect to proceed.");
        }
    }
    public void GoRegister()
    {
        loadingbuffer.SetActive(true);
        if (IsInternetAvailable())
        {
            levelLoader.circlewipeCont.SetActive(true);
            loadingbuffer.SetActive(false);
            levelLoader.LoadSceneAndTransition("RegisterScene");
        }
        else
        {
            loadingbuffer.SetActive(false);
            ShowWarningPopup("No internet connection. Please connect to proceed.");
        }
    }
    public void GoManual()
    {
        levelLoader.circlewipeCont.SetActive(true);
        levelLoader.LoadSceneAndTransition("Glossary");
    }
    public void GoAnalytics()
    {
        loadingbuffer.SetActive(true);
        if (!IsInternetAvailable())
        {
            loadingbuffer.SetActive(false);
            ShowWarningPopup("No internet connection. Please connect to proceed.");
            return;
        }

        if (auth.CurrentUser != null)
        {
            levelLoader.circlewipeCont.SetActive(true);
            loadingbuffer.SetActive(false);
            levelLoader.LoadSceneAndTransition("Analytics");
        }
        else
        {
            loadingbuffer.SetActive(false);
            ShowWarningPopup("Guest User cannot access Player Analytics and Game History.");
        }
    }

    public void ShowWarningPopup(string message)
    {
        warningPopup.SetActive(true);
        warningtext.text = message;
    }

    public void ClosePopup()
    {
        warningPopup.SetActive(false);
    }

    public void GoRankings()
    {
        loadingbuffer.SetActive(true);
        if (IsInternetAvailable())
        {
            levelLoader.circlewipeCont.SetActive(true);
            loadingbuffer.SetActive(false);
            levelLoader.LoadSceneAndTransition("Rankings");
        }
        else
        {
            loadingbuffer.SetActive(false);
            ShowWarningPopup("No internet connection. Please connect to proceed.");
        }
    }

    public void RetryGame()
    {
        if (PlayerPrefs.HasKey("GameMode"))
        {
            string key = PlayerPrefs.GetString("GameMode");

            if (key == "Easy")
            {
                EasyModeScene();
            }
            else if (key == "Normal")
            {
                NormalModeScene();
            }
            else if (key == "Hard")
            {
                HardModeScene();
            }
            else
            {
                Debug.LogWarning("Unknown game mode: " + key);
            }
        }
        else
        {
            Debug.LogWarning("Game mode not set in PlayerPrefs.");
        }
    }

}
