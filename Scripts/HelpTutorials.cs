using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HelpTutorials : MonoBehaviour
{
    public Image imageContainer; // The UI Image to display tutorial sprites.
    public GameObject circleIndicatorPrefab; // Prefab for the circle indicator.
    public Transform circleContainer; // Parent container for the circle indicators.

    public List<Sprite> imageList = new List<Sprite>(); // List of images for the tutorial.

    private int currentIndex = 0; // Index for navigation.
    private List<GameObject> circleIndicators = new List<GameObject>(); // List to hold instantiated circle indicators.

    public GameObject tutorialPage;
    bool isopen = false;

    public GameObject linegraphs;
    bool analyticsopen = true;

    private void Start()
    {
        if (imageList == null || imageList.Count == 0)
        {
            Debug.LogError("Image list is empty or not assigned.");
            return;
        }

        GenerateCircleIndicators();
        DisplayCurrentImage();
    }

    // Display the current image in the container
    public void DisplayCurrentImage()
    {
        if (imageList != null && imageList.Count > 0)
        {
            imageContainer.sprite = imageList[currentIndex];
            UpdateCircleIndicators();
        }
        else
        {
            Debug.LogError("Image list is empty or null.");
        }
    }

    // Navigate to the next image
    public void NextImage()
    {
        if (currentIndex < imageList.Count - 1)
        {
            currentIndex++;
            DisplayCurrentImage();
        }
    }

    // Navigate to the previous image
    public void PrevImage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            DisplayCurrentImage();
        }
    }

    // Generate circle indicators based on the number of sprites in the image list
    private void GenerateCircleIndicators()
    {
        // Clear old indicators
        foreach (GameObject circle in circleIndicators)
        {
            Destroy(circle);
        }
        circleIndicators.Clear();

        if (imageList != null)
        {
            for (int i = 0; i < imageList.Count; i++)
            {
                GameObject circle = Instantiate(circleIndicatorPrefab, circleContainer);
                circleIndicators.Add(circle);
            }
        }
    }

    // Update the circle indicators to reflect the current index
    private void UpdateCircleIndicators()
    {
        for (int i = 0; i < circleIndicators.Count; i++)
        {
            Image circleImage = circleIndicators[i].GetComponent<Image>();
            if (circleImage != null)
            {
                circleImage.color = (i == currentIndex) ? Color.green : Color.white;
            }
        }
    }

    public void TutorialPage()
    {
        isopen = !isopen;

        if (isopen)
        {
            tutorialPage.SetActive(true);
        }
        else
        {
            tutorialPage.SetActive(false);
        }
    }

    public void linegraph()
    {
        analyticsopen = !analyticsopen;

        if (analyticsopen)
        {
            linegraphs.SetActive(true);
        }
        else
        {
            linegraphs.SetActive(false);
        }
    }
}
