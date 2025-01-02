using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public Animator transition2;
    public Animator transition3;
    public GameObject circlewipeCont;
    public GameObject crossfadeCont;
    public GameObject reverseCircleWipe;

    [Space]
    public AudioSource circleWipe1;
    public AudioSource circleWipe2;
    public AudioSource fadetransition;

    public float transitionTime = 1f;

    private void Start()
    {
        // Check if the game just started
        if (PlayerPrefs.GetInt("IsGameJustStarted", 1) == 1)
        {
            // Play crossfade transition at game startup
            crossfadeCont.SetActive(true);
            circlewipeCont.SetActive(false);
            reverseCircleWipe.SetActive(false);

            // Set the flag to indicate the game has started
            PlayerPrefs.SetInt("IsGameJustStarted", 0);
            PlayerPrefs.Save();
        }
        else if (PlayerPrefs.GetInt("SkipLevelsTransition", 0) == 1)
        {
            // Disable the transition
            crossfadeCont.SetActive(true);
            circlewipeCont.SetActive(false);
            reverseCircleWipe.SetActive(false);

            // Reset the flag to avoid skipping on subsequent scene loads
            PlayerPrefs.SetInt("SkipLevelsTransition", 0);
            PlayerPrefs.Save();
        }
        else if (PlayerPrefs.GetInt("SkipLevelsTransition", 0) == 2)
        {
            crossfadeCont.SetActive(false);
            circlewipeCont.SetActive(false);
            reverseCircleWipe.SetActive(true);

            PlayerPrefs.SetInt("SkipLevelsTransition", 0);
            PlayerPrefs.Save();
        }
        else
        {
            // Default to circle wipe
            crossfadeCont.SetActive(false);
            circlewipeCont.SetActive(true);
            reverseCircleWipe.SetActive(false);
        }
    }

    public void LoadSceneAndTransition(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    public void LoadSceneAndTransition2(string sceneName)
    {
        StartCoroutine(LoadScene2(sceneName));
    }

    IEnumerator LoadScene(string sceneName)
    {
        transition.SetTrigger("Start");
        circleWipe1.Play();
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadScene2(string sceneName)
    {
        transition2.SetTrigger("Start");
        fadetransition.Play();
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadBackSceneAndTransition(string sceneName)
    {
        StartCoroutine(LoadBackScene(sceneName));
    }

    IEnumerator LoadBackScene(string sceneName)
    {
        transition3.SetTrigger("Start");
        circleWipe2.Play();
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }

}
