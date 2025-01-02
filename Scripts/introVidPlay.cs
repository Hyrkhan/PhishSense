using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class introVidPlay : MonoBehaviour
{
    [Header("Video Player and Screens")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoScreen; // Video playing screen
    [SerializeField] private GameObject nextScreen;  // Next screen to show

    private bool videoPlaying = true;

    void Awake()
    {

        if (PlayerPrefs.GetInt("SkipIntroVideo", 0) == 1)
        {
            videoScreen.SetActive(false);

            PlayerPrefs.SetInt("SkipIntroVideo", 0);
            PlayerPrefs.Save();
        }
        else
        {
            // Ensure video plays automatically
            videoPlayer.Play();

            // Listen for video end event
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    private void Update()
    {
        // Detect user tap to skip the video
        if (videoPlaying && Input.GetMouseButtonDown(0))
        {
            SkipVideo();
        }
    }

    private void SkipVideo()
    {
        videoPlaying = false;
        videoPlayer.Stop();
        videoScreen.SetActive(false);
        ShowNextScreen();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        videoPlaying = false;
        videoScreen.SetActive(false);
        ShowNextScreen();
    }

    private void ShowNextScreen()
    {
        videoScreen.SetActive(false); // Hide video screen
        nextScreen.SetActive(true);  // Show the next screen
    }
}