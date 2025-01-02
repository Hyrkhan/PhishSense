using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPageManager : MonoBehaviour
{
    public Image imageContainer; // The UI Image to display tutorial sprites.
    public GameObject circleIndicatorPrefab; // Prefab for the circle indicator.
    public Transform circleContainer; // Parent container for the circle indicators.

    // Lists for each tutorial category
    public List<Sprite> Emails = new List<Sprite>();
    public List<Sprite> EmailEvaluations = new List<Sprite>();
    public List<Sprite> ScanURL = new List<Sprite>();
    public List<Sprite> VM = new List<Sprite>();
    public List<Sprite> Hints = new List<Sprite>();
    public List<Sprite> Menu = new List<Sprite>();
    public List<Sprite> GameOver = new List<Sprite>();
    public List<Sprite> EmailReview = new List<Sprite>();

    private List<Sprite> currentList; // The list currently being displayed.
    private int currentIndex = 0; // Index for navigation.
    private List<GameObject> circleIndicators = new List<GameObject>(); // List to hold instantiated circle indicators.

    private void Start()
    {

    }

    // Call this function to set the current list (e.g., when switching topics)
    public void SetCurrentList(string topic)
    {
        switch (topic)
        {
            case "Emails":
                currentList = Emails;
                break;
            case "EmailEvaluations":
                currentList = EmailEvaluations;
                break;
            case "ScanURL":
                currentList = ScanURL;
                break;
            case "VM":
                currentList = VM;
                break;
            case "Hints":
                currentList = Hints;
                break;
            case "Menu":
                currentList = Menu;
                break;
            case "GameOver":
                currentList = GameOver;
                break;
            case "EmailReview":
                currentList = EmailReview;
                break;
            default:
                Debug.LogError("Invalid topic name");
                return;
        }

        // Reset index and display
        currentIndex = 0;
        GenerateCircleIndicators();
        DisplayCurrentImage();
    }

    // Display the current image in the container
    public void DisplayCurrentImage()
    {
        if (currentList != null && currentList.Count > 0)
        {
            imageContainer.sprite = currentList[currentIndex];
            UpdateCircleIndicators();
        }
        else
        {
            Debug.LogError("Current list is empty or null");
        }
    }

    // Navigate to the next image
    public void NextImage()
    {
        if (currentIndex < currentList.Count - 1)
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

    // Generate circle indicators based on the number of sprites in the current list
    private void GenerateCircleIndicators()
    {
        // Clear old indicators
        foreach (GameObject circle in circleIndicators)
        {
            Destroy(circle);
        }
        circleIndicators.Clear();

        if (currentList != null)
        {
            for (int i = 0; i < currentList.Count; i++)
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
}
