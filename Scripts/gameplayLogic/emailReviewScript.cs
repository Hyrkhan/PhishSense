using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EmailFetcher;

public class emailReviewScript : MonoBehaviour
{
    public evaluationScript evaluationScript;
    public gameObjectsScript gameObjectsScript;
    public scanResultScript scanResultScript;

    private int emailIndex;
    private int currentPageNum = 0;
    private List<EmailFetcher.Email> emails;
    public List<GameObject> ReviewDisplayContainers;


    List<string> questionLabels = new List<string>
    {
        "1. Is there a grammar error on the email?",
        "2. Is the sender of the email suspicious?",
        "3. What is the risk level of the link in the email?",
        "4. How many security violations did the website in the link violate?",
        "5. Is the SSL/TLS certificate of the website in the link expired?",
        "6. So, is the email Safe? or a Phishing email?"
    };

    List<string> correctAnswerExplanations = new List<string>
    {
        "The email body",
        "The sender of the email",
        "According to the scan result of the url link in the email, the risk level is",
        "According to the scan result of the url link in the email, the website have",
        "Based on the scan result of the url link in the email, the SSL/TLS certificate of the website is",
        "Based on the VM simulated result of the url link in the email, the email is"
    };
    public void SetEmails(List<EmailFetcher.Email> fetchedEmails)
    {
        emails = fetchedEmails;
    }

    private List<List<string>> correctAnswersList;

    public void DisplayCorrectAnswerReason()
    {
        // Populate answers if not already populated
        if (correctAnswersList == null || correctAnswersList.Count == 0)
        {
            PopulateAllCorrectAnswers();
        }

        // Access the correct answer explanation for the specific email and page
        gameObjectsScript.correctAnswerExplanation.text = $"{correctAnswerExplanations[currentPageNum]} {correctAnswersList[emailIndex][currentPageNum]}";
        DisplayReviewContainers();
    }

    public void DisplayReviewContainers()
    {
        PopulateReviewContent();

        // Deactivate all containers first
        foreach (var container in ReviewDisplayContainers)
        {
            container.SetActive(false);
        }

        // Activate only the container at currentPageNum, if it's within bounds
        if (currentPageNum >= 0 && currentPageNum < ReviewDisplayContainers.Count)
        {
            ReviewDisplayContainers[currentPageNum].SetActive(true);
        }
    }

    public void PopulateReviewContent()
    {
        gameObjectsScript.ReviewEmailBodyText.text = emails[emailIndex].emailTextBody;
        gameObjectsScript.ReviewSenderEmail.text = emails[emailIndex].senderEmail;
        gameObjectsScript.ReviewRiskLevel.text = scanResultScript.CalculateSecurityRisk(emailIndex);
        gameObjectsScript.ReviewViolations.text = $"{scanResultScript.CalculateSecurityViolations(emailIndex)}";
        gameObjectsScript.ReviewCertIssue.text = emails[emailIndex].certIssueDate;
        gameObjectsScript.ReviewCertExpiry.text = emails[emailIndex].certExpiryDate;
        ShowEmailMarkAnswer();
    }

    public void ShowEmailMarkAnswer()
    {
        if (emails[emailIndex].markAnswer == "Phishing")
        {
            gameObjectsScript.ReviewSafeMark.SetActive(false);
            gameObjectsScript.ReviewPhishingMark.SetActive(true);
        }
        else
        {
            gameObjectsScript.ReviewPhishingMark.SetActive(false);
            gameObjectsScript.ReviewSafeMark.SetActive(true);
        }
    }

    public void PopulateAllCorrectAnswers()
    {
        correctAnswersList = new List<List<string>>();

        // Assuming GiveEmailCount() gives the total number of emails
        for (int i = 0; i < emails.Count; i++)
        {
            var emailCorrectAnswers = new List<string>();

            if (scanResultScript.CheckGrammar(i) == "No")
            {
                emailCorrectAnswers.Add("have no grammatical errors");
            }
            else
            {
                emailCorrectAnswers.Add("have grammatical errors");
            }

            if (scanResultScript.CheckSuspiciousSender(i) == "No")
            {
                emailCorrectAnswers.Add("is not suspicious");
            }
            else
            {
                emailCorrectAnswers.Add("is suspicious");
            }

            emailCorrectAnswers.Add(scanResultScript.CalculateSecurityRisk(i));
            emailCorrectAnswers.Add($"{scanResultScript.CalculateSecurityViolations(i)} violations");
            emailCorrectAnswers.Add(scanResultScript.CallCertificateResult(i));

            

            string vmresult = "";

            if (scanResultScript.CheckWebsiteType(i) == "safeWebsite")
            {
                vmresult += "Safe";
            }
            else if (scanResultScript.CheckWebsiteType(i) == "fakeWebsite")
            {
                vmresult += "Phishing, because the link redirected to a suspicious website";
                if (scanResultScript.CheckFileDownload(i) == "virus")
                {
                    vmresult += " and it downloaded a virus to the computer";
                }
            }
            else if (scanResultScript.CheckFileDownload(i) == "virus")
            {
                vmresult += "Phishing, because the link downloaded a virus to the computer";
            }

            emailCorrectAnswers.Add(vmresult);

            // Add the answers for this email to the main list
            correctAnswersList.Add(emailCorrectAnswers);
        }

    }

    public void CheckEmail1()
    {
        emailIndex = 0;
        currentPageNum = 0;
        DisplayPage();
        DisplayCorrectAnswerReason();
    }
    public void CheckEmail2()
    {
        emailIndex = 1;
        currentPageNum = 0;
        DisplayPage();
        DisplayCorrectAnswerReason(); 
    }

    public void CheckEmail3()
    {
        emailIndex = 2;
        currentPageNum = 0;
        DisplayPage();
        DisplayCorrectAnswerReason();
    }

    public void CheckEmail4()
    {
        emailIndex = 3;
        currentPageNum = 0;
        DisplayPage();
        DisplayCorrectAnswerReason();
    }

    public void CheckEmail5()
    {
        emailIndex = 4;
        currentPageNum = 0;
        DisplayPage();
        DisplayCorrectAnswerReason();
    }

    public void DisplayPage()
    {
        var userEmailanswers = evaluationScript.GetEmailAnswers(emailIndex);
        gameObjectsScript.reviewUserAnswer.text = $"You answered: {userEmailanswers.answers[currentPageNum]}";

        if (userEmailanswers.checkers[currentPageNum] == "Correct")
        {
            gameObjectsScript.reviewCheckAnswer.text = $"Your answer is: <color=green>{userEmailanswers.checkers[currentPageNum]}</color>";
        }
        else
        {
            gameObjectsScript.reviewCheckAnswer.text = $"Your answer is: <color=red>{userEmailanswers.checkers[currentPageNum]}</color>";
        }
        
        gameObjectsScript.pageNumber.text = $"Evaluation {currentPageNum + 1}";
        gameObjectsScript.questionLabel.text = questionLabels[currentPageNum];
    }

    public void PrevPage()
    {
        if (currentPageNum > 0)
        {
            currentPageNum--;
            DisplayPage();
            DisplayCorrectAnswerReason();
        }
    }
    public void NextPage()
    {
        if (currentPageNum < 5)
        {
            currentPageNum++;
            DisplayPage();
            DisplayCorrectAnswerReason();
        }
    }
}
