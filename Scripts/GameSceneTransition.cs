using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class GameSceneTransition : MonoBehaviour
{
    public static GameSceneTransition Instance;

    public VideoPlayer tvAnimation;

    public float transitionTime = 1f;

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    IEnumerator LoadScene(string sceneName)
    {
        tvAnimation.Play();
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }
}
