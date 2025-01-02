using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using TMPro;
using System.Text;
using Firebase.Auth;

public class GameOverScript : MonoBehaviour
{
    public List<GameObject> safeLogos;
    public List<GameObject> warningLogos;
    private List<EmailFetcher.Email> emails;

    public evaluationScript evaluationScript;
    private int totalscores;
    public TMP_Text totalScore;

    public List<GameObject> checkLogos;
    public List<GameObject> wrongLogos;
    public List<TMP_Text> emailScores;

    

    public bool isnotfinished = false;

    

    public void SetEmails(List<EmailFetcher.Email> fetchedEmails)
    {
        emails = fetchedEmails;
    }
    public void SetTotalScore()
    {
        totalscores = evaluationScript.GetFinalScores();
    }

    public void ShowAnalytics()
    {
        var questionPerformance = evaluationScript.GetAnalytics();
        StringBuilder analyticsOutput = new StringBuilder();

        foreach (var item in questionPerformance)
        {
            string questionType = item.Key;
            int correct = item.Value.Item1; // Access the first item (correct count)
            int total = item.Value.Item2;   // Access the second item (total count)
            double accuracy = total > 0 ? (double)correct / total : 0;

            //Debug.Log($"{questionType} Accuracy: {accuracy * 100}%");
            analyticsOutput.AppendLine($"{questionType} Accuracy: {accuracy * 100:F2}%");
        }

    }

    public bool ShowAnswers()
    {
        SetTotalScore();
        ShowAnalytics();
        List<bool?> emailMarks = evaluationScript.GetEmailMarks();
        var finalScores = evaluationScript.finalScores;

        bool hasUnansweredMarks = false;

        for (int i = 0; i < emails.Count; i++)
        {
            var email = emails[i];
            var mark = emailMarks[i];

            if (email.markAnswer == "Phishing")
            {
                ShowEmailMarkAnswer(true, i);  // Passing true for phishing
            }
            else if (email.markAnswer == "Safe")
            {
                ShowEmailMarkAnswer(false, i);  // Passing false for safe
            }
            else
            {
                Debug.Log("Somehow, error");
            }

            if (mark.HasValue)  // Checks if mark is not null
            {
                if (mark.Value == true && email.markAnswer == "Phishing")
                {
                    ShowUserMarkAnswer(false, i);
                }
                else if (mark.Value == false && email.markAnswer == "Safe")
                {
                    ShowUserMarkAnswer(false, i);
                }
                else
                {
                    ShowUserMarkAnswer(true, i);
                }
            }
            else
            {
                ShowUserMarkAnswer(true, i);
                Debug.Log($"No mark found for email {i}");
                hasUnansweredMarks = true;
            }

            emailScores[i].text = $"{finalScores[i]}/10";
        }

        totalScore.text = $"{totalscores} / {emails.Count}0";

        return hasUnansweredMarks;  // Return true if there are unanswered marks
    }


    public void ShowEmailMarkAnswer(bool isPhishing, int number)
    {
        if (isPhishing)
        {
            // Show warning logo, hide safe logo
            safeLogos[number].SetActive(false);
            warningLogos[number].SetActive(true);
        }
        else
        {
            // Show safe logo, hide warning logo
            warningLogos[number].SetActive(false);
            safeLogos[number].SetActive(true);
        }
    }
    public void ShowUserMarkAnswer(bool incorrect, int number)
    {
        if (incorrect)
        {
            checkLogos[number].SetActive(false);
            wrongLogos[number].SetActive(true);
        }
        else
        {
            wrongLogos[number].SetActive(false);
            checkLogos[number].SetActive(true);
        }
    }


}
