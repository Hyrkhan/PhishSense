using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;

public class evaluationScript : MonoBehaviour
{
    public List<Button> yesButtons;  // List of Yes buttons
    public List<Button> noButtons;   // List of No buttons

    public Button lowButton;         // Low risk button
    public Button midButton;         // Medium risk button
    public Button highButton;        // High risk button

    // Initial colors
    private Color defaultColor = Color.white;
    private Color yesColor = Color.green;
    private Color noColor = Color.red;
    private Color lowColor = Color.green;
    private Color midColor = Color.yellow;
    private Color highColor = Color.red;

    // To track the state of each Yes/No button pair
    private bool[] isYesSelected;
    private bool[] isNoSelected;

    // To track the state of the risk level buttons
    private bool isLowSelected = false;
    private bool isMidSelected = false;
    private bool isHighSelected = false;

    private List<List<string>> emailEvaluations = new List<List<string>>();
    private List<Color[]> yesButtonColors = new List<Color[]>();
    private List<Color[]> noButtonColors = new List<Color[]>();
    private List<Color[]> riskButtonColors = new List<Color[]>();
    private int currentEmailIndex;

    public TMP_InputField violationsText;
    public scanResultScript scanResultScript;
    public GameObject safeLogo;
    //public GameObject zoom_safeLogo;
    public GameObject warningLogo;
    //public GameObject zoom_warningLogo;

    private List<bool?> emailMarks = new List<bool?>();
    public int emailCount;

    public List<int> finalScores = new List<int>();
    public List<List<string>> userAnswers = new List<List<string>>();
    public List<List<string>> checkAnswers = new List<List<string>>();

    private List<int?> userViolations;

    public Dictionary<string, (int correct, int total)> questionPerformance = new Dictionary<string, (int, int)>
    {
        { "Grammar Check", (0, 0) },
        { "Suspicious Sender", (0, 0) },
        { "URL Risk", (0, 0) },
        { "Violations", (0, 0) },
        { "Certificate Status", (0, 0) },
        { "Email Mark", (0, 0) }
    };
    void Start()
    {
        ResetButtons();
    }
    public void InitializeViolations(int emailcount)
    {
        // Assuming emails.Count is the total number of emails
        InitializeViolationsList(emailcount);

        // Add listener for text input changes
        violationsText.onValueChanged.AddListener(SaveViolationsInput);
    }
    public void SaveViolationsInput(string input)
    {
        if (int.TryParse(input, out int violationCount))
        {
            userViolations[currentEmailIndex] = violationCount;
            Debug.Log($"Saved violations for email {currentEmailIndex}: {violationCount}");
        }
        else
        {
            userViolations[currentEmailIndex] = null;
            Debug.Log($"Invalid input for email {currentEmailIndex}: {input}");
        }
    }

    public void LoadViolationsInput(int emailIndex)
    {
        if (userViolations[emailIndex].HasValue)
        {
            violationsText.text = userViolations[emailIndex].Value.ToString();
        }
        else
        {
            violationsText.text = "";
        }
    }

    void InitializeViolationsList(int emailCount)
    {
        userViolations = new List<int?>(new int?[emailCount]);
        for (int i = 0; i < emailCount; i++)
        {
            userViolations.Add(null);
        }
    }
    public void InitializeEmailEvaluationList(int count)
    {
        isYesSelected = new bool[3];
        isNoSelected = new bool[3];
        // For simplicity, assuming you have 6 questions
        emailCount = count;
        int numOfEvaluation = 6;

        for (int i = 0; i < count; i++)
        {
            List<string> emailEvaluate = new List<string>(new string[numOfEvaluation]); // initialize with empty strings
            emailEvaluations.Add(emailEvaluate);
            List<string> emptyanswer = new List<string>(new string[numOfEvaluation]);
            userAnswers.Add(emptyanswer);
            List<string> emptychecks = new List<string>(new string[numOfEvaluation]);
            checkAnswers.Add(emptychecks);

            emailMarks.Add(null);
            finalScores.Add(0);


            yesButtonColors.Add(new Color[yesButtons.Count]);
            noButtonColors.Add(new Color[noButtons.Count]);
            riskButtonColors.Add(new Color[] { defaultColor, defaultColor, defaultColor });

            // Set all to default color initially
            for (int j = 0; j < yesButtons.Count; j++)
            {
                yesButtonColors[i][j] = defaultColor;
                noButtonColors[i][j] = defaultColor;
            }
        }
    }

    // Call this function when loading a new email
    public void SetCurrentEmailIndex(int emailIndex)
    {
        currentEmailIndex = emailIndex;
        ResetButtons();
        for (int i = 0; i < yesButtons.Count; i++)
        {
            yesButtons[i].GetComponent<Image>().color = yesButtonColors[emailIndex][i];
            noButtons[i].GetComponent<Image>().color = noButtonColors[emailIndex][i];
        }

        lowButton.GetComponent<Image>().color = riskButtonColors[emailIndex][0];
        midButton.GetComponent<Image>().color = riskButtonColors[emailIndex][1];
        highButton.GetComponent<Image>().color = riskButtonColors[emailIndex][2];
        violationsText.text = emailEvaluations[currentEmailIndex][4];
    }

    private void ResetButtons()
    {
        // If no evaluation saved for this email, set buttons to default colors
        if (emailEvaluations[currentEmailIndex].TrueForAll(e => e == null || e == "null"))
        {
            for (int i = 0; i < yesButtons.Count; i++)
            {
                yesButtons[i].GetComponent<Image>().color = defaultColor;
                noButtons[i].GetComponent<Image>().color = defaultColor;
            }
            lowButton.GetComponent<Image>().color = defaultColor;
            midButton.GetComponent<Image>().color = defaultColor;
            highButton.GetComponent<Image>().color = defaultColor;
        }
    }

    public void TurnOffButtons()
    {
        for (int i = 0; i < yesButtons.Count; i++)
        {
            yesButtons[i].GetComponent<Image>().color = defaultColor;
            noButtons[i].GetComponent<Image>().color = defaultColor;
        }

        lowButton.GetComponent<Image>().color = defaultColor;
        midButton.GetComponent<Image>().color = defaultColor;
        highButton.GetComponent<Image>().color = defaultColor;

        // Ensure button states are reset as well
        isYesSelected = new bool[yesButtons.Count];
        isNoSelected = new bool[noButtons.Count];
        isLowSelected = false;
        isMidSelected = false;
        isHighSelected = false;
        violationsText.text = "";
    }

    public void ToggleYesButton(int index)
    {
        if (!isYesSelected[index])  // If not selected, turn Yes green and reset No
        {
            yesButtons[index].GetComponent<Image>().color = yesColor;
            noButtons[index].GetComponent<Image>().color = defaultColor;  // Reset No button
            isYesSelected[index] = true;
            isNoSelected[index] = false;
            emailEvaluations[currentEmailIndex][index] = "Yes";

            yesButtonColors[currentEmailIndex][index] = yesColor;
            noButtonColors[currentEmailIndex][index] = defaultColor;
        }
        else  // If already selected, reset color
        {
            yesButtons[index].GetComponent<Image>().color = defaultColor;
            isYesSelected[index] = false;
            emailEvaluations[currentEmailIndex][index] = "null";

            yesButtonColors[currentEmailIndex][index] = defaultColor;
        }
        //Debug.Log($"Button Yes: {index} is pressed");
    }

    // Toggle No button for a specific question index
    public void ToggleNoButton(int index)
    {
        if (!isNoSelected[index])  // If not selected, turn No red and reset Yes
        {
            noButtons[index].GetComponent<Image>().color = noColor;
            yesButtons[index].GetComponent<Image>().color = defaultColor;  // Reset Yes button
            isNoSelected[index] = true;
            isYesSelected[index] = false;
            emailEvaluations[currentEmailIndex][index] = "No";

            noButtonColors[currentEmailIndex][index] = noColor;
            yesButtonColors[currentEmailIndex][index] = defaultColor;
        }
        else  // If already selected, reset color
        {
            noButtons[index].GetComponent<Image>().color = defaultColor;
            isNoSelected[index] = false;
            emailEvaluations[currentEmailIndex][index] = "null";

            noButtonColors[currentEmailIndex][index] = defaultColor;
        }
        //Debug.Log($"Button No: {index} is pressed");
    }

    // Toggle Low button
    public void ToggleLowButton()
    {
        if (!isLowSelected)  // If not selected, turn Low green and reset others
        {
            lowButton.GetComponent<Image>().color = lowColor;
            midButton.GetComponent<Image>().color = defaultColor;
            highButton.GetComponent<Image>().color = defaultColor;
            isLowSelected = true;
            isMidSelected = false;
            isHighSelected = false;
            emailEvaluations[currentEmailIndex][3] = "Low";

            riskButtonColors[currentEmailIndex][0] = lowColor;
            riskButtonColors[currentEmailIndex][1] = defaultColor;
            riskButtonColors[currentEmailIndex][2] = defaultColor;
        }
        else  // If already selected, reset color
        {
            lowButton.GetComponent<Image>().color = defaultColor;
            isLowSelected = false;
            emailEvaluations[currentEmailIndex][3] = "null";
            riskButtonColors[currentEmailIndex][0] = defaultColor;
        }
        //Debug.Log($"Low Button is pressed");
    }

    // Toggle Mid button
    public void ToggleMidButton()
    {
        if (!isMidSelected)  // If not selected, turn Mid yellow and reset others
        {
            midButton.GetComponent<Image>().color = midColor;
            lowButton.GetComponent<Image>().color = defaultColor;
            highButton.GetComponent<Image>().color = defaultColor;
            isMidSelected = true;
            isLowSelected = false;
            isHighSelected = false;
            emailEvaluations[currentEmailIndex][3] = "Medium";

            riskButtonColors[currentEmailIndex][0] = defaultColor;
            riskButtonColors[currentEmailIndex][1] = midColor;
            riskButtonColors[currentEmailIndex][2] = defaultColor;
        }
        else  // If already selected, reset color
        {
            midButton.GetComponent<Image>().color = defaultColor;
            isMidSelected = false;
            emailEvaluations[currentEmailIndex][3] = "null";
            riskButtonColors[currentEmailIndex][1] = defaultColor;
        }
        //Debug.Log($"Mid Button is pressed");
    }

    // Toggle High button
    public void ToggleHighButton()
    {
        if (!isHighSelected)  // If not selected, turn High red and reset others
        {
            highButton.GetComponent<Image>().color = highColor;
            lowButton.GetComponent<Image>().color = defaultColor;
            midButton.GetComponent<Image>().color = defaultColor;
            isHighSelected = true;
            isLowSelected = false;
            isMidSelected = false;
            emailEvaluations[currentEmailIndex][3] = "High";

            riskButtonColors[currentEmailIndex][0] = defaultColor;
            riskButtonColors[currentEmailIndex][1] = defaultColor;
            riskButtonColors[currentEmailIndex][2] = highColor;
        }
        else  // If already selected, reset color
        {
            highButton.GetComponent<Image>().color = defaultColor;
            isHighSelected = false;
            emailEvaluations[currentEmailIndex][3] = "null";
            riskButtonColors[currentEmailIndex][2] = defaultColor;
        }
        //Debug.Log($"High Button is pressed");
    }

    public void MarkAsSafe()
    {
        //Debug.Log("Marked as Safe");
        emailMarks[currentEmailIndex] = false;
        ToggleEmailMark(false);
    }

    public void MarkAsPhishing()
    {
        //Debug.Log("Marked as Phishing");
        emailMarks[currentEmailIndex] = true;
        ToggleEmailMark(true);
    }

    private void UpdateQuestionPerformance(string questionType, bool isCorrect)
    {
        // Retrieve the current values for correct and total
        int correct = questionPerformance[questionType].correct;

        // Update the correct count if the answer is correct
        if (isCorrect)
        {
            correct += 1;
        }

        // Update the dictionary with the new values
        questionPerformance[questionType] = (correct, 5);
    }

    public void AnswerChecker()
    {
        Debug.Log("Answer checker called");
        List<string> evaluations = emailEvaluations[currentEmailIndex];
        var userAns = userAnswers[currentEmailIndex];
        var checkAns = checkAnswers[currentEmailIndex];
        string grammarError = scanResultScript.CheckGrammar(currentEmailIndex);
        string suspiciousSender = scanResultScript.CheckSuspiciousSender(currentEmailIndex);
        string risk = scanResultScript.CalculateSecurityRisk(currentEmailIndex);
        int violations = scanResultScript.CalculateSecurityViolations(currentEmailIndex);
        string markAns = scanResultScript.CheckMarkAnswer(currentEmailIndex);
        string certResult = scanResultScript.CallCertificateResult(currentEmailIndex);
        int violationsInputed = 0;
        int score = 0;


        // Retrieve the user-inputted violations
        if (userViolations[currentEmailIndex].HasValue)
        {
            violationsInputed = userViolations[currentEmailIndex].Value;
            emailEvaluations[currentEmailIndex][4] = violationsInputed.ToString();
        }
        else
        {
            emailEvaluations[currentEmailIndex][4] = "";
        }

        // Grammar Check
        if (evaluations[0] == grammarError)
        {
            checkAns[0] = "Correct";
            score++;
            UpdateQuestionPerformance("Grammar Check", true);
        }
        else
        {
            checkAns[0] = "Incorrect";
            UpdateQuestionPerformance("Grammar Check", false);
        }
        userAns[0] = evaluations[0];

        // Suspicious Sender Check
        if (evaluations[1] == suspiciousSender)
        {
            score++;
            checkAns[1] = "Correct";
            UpdateQuestionPerformance("Suspicious Sender", true);
        }
        else
        {
            checkAns[1] = "Incorrect";
            UpdateQuestionPerformance("Suspicious Sender", false);
        }
        userAns[1] = evaluations[1];

        // URL Risk Check
        if (evaluations[3] == risk)
        {
            score++;
            checkAns[2] = "Correct";
            UpdateQuestionPerformance("URL Risk", true);
        }
        else
        {
            checkAns[2] = "Incorrect";
            UpdateQuestionPerformance("URL Risk", false);
        }
        userAns[2] = evaluations[3];

        // Violations Check
        if (evaluations[4] == $"{violations}")
        {
            score++;
            checkAns[3] = "Correct";
            UpdateQuestionPerformance("Violations", true);
        }
        else
        {
            checkAns[3] = "Incorrect";
            UpdateQuestionPerformance("Violations", false);
        }
        userAns[3] = evaluations[4];

        // Certificate Status Check
        if (evaluations[2] == "Yes" && certResult == "Expired")
        {
            score++;
            checkAns[4] = "Correct";
            UpdateQuestionPerformance("Certificate Status", true);
        }
        else if (evaluations[2] == "No" && certResult == "Valid")
        {
            score++;
            checkAns[4] = "Correct";
            UpdateQuestionPerformance("Certificate Status", true);
        }
        else
        {
            checkAns[4] = "Incorrect";
            UpdateQuestionPerformance("Certificate Status", false);
        }
        userAns[4] = evaluations[2];

        // Email Mark Check
        if (emailMarks[currentEmailIndex] == true && markAns == "Phishing")
        {
            score += 5;
            checkAns[5] = "Correct";
            UpdateQuestionPerformance("Email Mark", true);
        }
        else if (emailMarks[currentEmailIndex] == false && markAns == "Safe")
        {
            score += 5;
            checkAns[5] = "Correct";
            UpdateQuestionPerformance("Email Mark", true);
        }
        else
        {
            checkAns[5] = "Incorrect";
            UpdateQuestionPerformance("Email Mark", false);
        }

        // Save user's mark answer
        if (emailMarks[currentEmailIndex] == true)
        {
            userAns[5] = "Marked as Phishing";
        }
        else if (emailMarks[currentEmailIndex] == false)
        {
            userAns[5] = "Marked as Safe";
        }

        // Save final score for the email
        finalScores[currentEmailIndex] = score;
        Debug.Log($"{score} this the finalscore");
    }


    public void ToggleEmailMark(bool isPhishing)
    {
        if (isPhishing)
        {
            // Show warning logo, hide safe logo
            safeLogo.SetActive(false);
            //zoom_safeLogo.SetActive(false); 
            warningLogo.SetActive(true);
            //zoom_warningLogo.SetActive(true);
        }
        else
        {
            // Show safe logo, hide warning logo
            warningLogo.SetActive(false);
            //zoom_warningLogo.SetActive(false);
            safeLogo.SetActive(true);
            //zoom_safeLogo.SetActive(true);
        }
    }

    public void DisplayEmailMark()
    {
        bool? mark = emailMarks[currentEmailIndex];

        if (mark.HasValue)
        {
            // If the email is marked, show the appropriate logo
            ToggleEmailMark(mark.Value);
        }
        else
        {
            // No mark saved, hide both logos
            warningLogo.SetActive(false);
            //zoom_warningLogo.SetActive(false);
            safeLogo.SetActive(false);
            //zoom_safeLogo.SetActive(false);
        }
    }
    public class EmailAnswers
    {
        public List<string> answers { get; set; }
        public List<string> checkers { get; set; }
        public EmailAnswers(List<string> answerList, List<string> checkerList)
        {
            answers = answerList;
            checkers = checkerList;
        }
    }
    public EmailAnswers GetEmailAnswers(int currentEmailindex)
    {
        var answerList = userAnswers[currentEmailindex];
        var checkanwers = checkAnswers[currentEmailindex];
        return new EmailAnswers(answerList, checkanwers);
    }

    public class EmailData
    {
        public bool? Mark { get; set; }
        public List<string> Evaluation { get; set; }

        public EmailData(bool? mark, List<string> evaluation)
        {
            Mark = mark;
            Evaluation = evaluation;
        }
    }
    public EmailData GetEmailData(int currentEmailIndex)
    {
        bool? emailMark = emailMarks[currentEmailIndex];
        List<string> emailEvaluation = emailEvaluations[currentEmailIndex];

        return new EmailData(emailMark, emailEvaluation);
    }

    public List<string> GetEmailEvaluations(int currentEmailIndex)
    {
        // Ensure the index is within bounds
        if (currentEmailIndex < 0 || currentEmailIndex >= emailEvaluations.Count)
        {
            return new List<string>(); // Return an empty list if the index is out of bounds
        }

        List<string> emailEvaluation = emailEvaluations[currentEmailIndex];
        return emailEvaluation;
    }

    public class EmailButtonState
    {
        public string Evaluation { get; set; }  // "Yes", "No", or "null"
        public Color ButtonColor { get; set; }  // Color of the button

        public EmailButtonState(string evaluation, Color buttonColor)
        {
            Evaluation = evaluation;
            ButtonColor = buttonColor;
        }
    }

    public int GetFinalScores()
    {
        int totalScore = 0;
        foreach (var score in finalScores)
        {
            totalScore += score;
        }
        Debug.Log($"{totalScore} this the totalScorescore");
        return totalScore;  
    }
    public List<bool?> GetEmailMarks()
    {
        return emailMarks;
    }
    public Dictionary<string, (int, int)> GetAnalytics()
    {
        return questionPerformance;
    }

    public void CheckAllAnswers()
    {
        for (int i = 0; i < emailCount; i++) // Loop through all emails
        {
            currentEmailIndex = i; // Update index for each email
            AnswerChecker(); // Evaluate the email
        }
    }

  
}
