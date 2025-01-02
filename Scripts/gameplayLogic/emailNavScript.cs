using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class emailNavScript : MonoBehaviour
{
    public gameObjectsScript gameObjectsScript;

    private List<EmailFetcher.Email> emails;
    private int currentIndex = 0;  // Track which email is currently displayed

    public hintsScript hintSystem;
    public scanResultScript scanResultScript;
    public evaluationScript evaluationScript;
    public EvaluateFormScript evaluateFormScript;
    public detectionScript detectionScript;

    // Set the emails from the fetched data
    public void SetEmails(List<EmailFetcher.Email> fetchedEmails)
    {
        emails = fetchedEmails;
    }

    // Display the email data in the UI
    public void DisplayEmail(int index)
    {
        if (emails == null || index < 0 || index >= emails.Count)
        {
            Debug.LogError("Invalid email index or emails not loaded.");
            return;
        }

        EmailFetcher.Email email = emails[index];

        gameObjectsScript.senderEmailDisplay.text = email.senderEmail;
        gameObjectsScript.emailBodyDisplay.text = email.emailTextBody;
        gameObjectsScript.emailLinkDisplay.text = email.emailLink;
        gameObjectsScript.emailNumber.text = $"Email {currentIndex+1}";

        // Update hint system with the new hint
        hintSystem.SetHint(email.hint);  // Set the hint for the current email
        evaluationScript.SetCurrentEmailIndex(index);
        evaluateFormScript.SetCurrentEmailIndex(index);

        Debug.Log($"Email {index} displayed");
    }

    // Navigation for next and previous buttons
    public void NextEmail()
    {
        if (currentIndex < emails.Count - 1)
        {
            currentIndex++;
            DisplayEmail(currentIndex);

            // Check if the email has been evaluated previously
            if (IsEmailEvaluated(currentIndex))
            {
                // Display saved button colors and marks
                evaluationScript.DisplayEmailMark();
            }
            else
            {
                // Reset buttons for a new, unevaluated email
                evaluationScript.DisplayEmailMark();
                evaluationScript.TurnOffButtons();
            }
        }
    }

    public void PreviousEmail()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            DisplayEmail(currentIndex);

            // Display saved evaluation data and button colors
            if (IsEmailEvaluated(currentIndex))
            {
                evaluationScript.DisplayEmailMark();
            }
            else
            {
                // Reset buttons for an unevaluated email
                evaluationScript.DisplayEmailMark();
                evaluationScript.TurnOffButtons();
            }
        }
    }

    public bool IsEmailEvaluated(int index)
    {
        // Get the evaluations for the current email
        List<string> emailEvaluations = evaluationScript.GetEmailEvaluations(index);

        // Check if any evaluation exists for this email
        return emailEvaluations.Any(evaluation => !string.IsNullOrEmpty(evaluation) && evaluation != "null");
    }
}
