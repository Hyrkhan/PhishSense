using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Firestore;
using TMPro;
using System;
using UnityEngine.UI;

public class rankingScript : MonoBehaviour
{
    [Header("Leaderboard UI Content")]
    [SerializeField] private Transform overallLeaderboardContent;
    [SerializeField] private Transform certificateLeaderboardContent;
    [SerializeField] private Transform emailMarkLeaderboardContent;
    [SerializeField] private Transform grammarCheckLeaderboardContent;
    [SerializeField] private Transform suspiciousSenderLeaderboardContent;
    [SerializeField] private Transform urlRiskLeaderboardContent;
    [SerializeField] private Transform violationsLeaderboardContent;

    [Header("Leaderboard Entry Prefab")]
    [SerializeField] private GameObject leaderboardEntryPrefab;

    [SerializeField] private Sprite firstPlaceIcon;
    [SerializeField] private Sprite secondPlaceIcon;
    [SerializeField] private Sprite thirdPlaceIcon;

    private FirebaseFirestore db;

    private string gameMode;
    public MenuButtons menuButtons;


    private void Start()
    {
        //gameMode = PlayerPrefs.GetString("GameMode", "Easy");
        gameMode = "Easy";
        db = FirebaseFirestore.DefaultInstance;
        FetchAndDisplayLeaderboard();
    }

    public void DisplayGameModeLeaderboard(string mode)
    {
        gameMode = mode;
        db = FirebaseFirestore.DefaultInstance;
        FetchAndDisplayLeaderboard();
    }

    private async void FetchAndDisplayLeaderboard()
    {
        try
        {
            QuerySnapshot usersSnapshot = await db.Collection("userAnalytics").GetSnapshotAsync();
            Debug.Log($"Fetched {usersSnapshot.Count} users from Firestore.");

            List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();

            foreach (DocumentSnapshot userDoc in usersSnapshot.Documents)
            {
                LeaderboardEntry entry = await FetchUserLeaderboardEntry(userDoc);
                if (entry != null)
                {
                    leaderboardEntries.Add(entry);
                }
            }

            // Display all leaderboards
            DisplayLeaderboard(overallLeaderboardContent, leaderboardEntries, "OverallWeek");
            DisplayLeaderboard(certificateLeaderboardContent, leaderboardEntries, "CertificateStatus");
            DisplayLeaderboard(emailMarkLeaderboardContent, leaderboardEntries, "EmailMark");
            DisplayLeaderboard(grammarCheckLeaderboardContent, leaderboardEntries, "GrammarCheck");
            DisplayLeaderboard(suspiciousSenderLeaderboardContent, leaderboardEntries, "SuspiciousSender");
            DisplayLeaderboard(urlRiskLeaderboardContent, leaderboardEntries, "URLRisk");
            DisplayLeaderboard(violationsLeaderboardContent, leaderboardEntries, "Violations");

            Debug.Log("Finished displaying leaderboards.");
        }
        catch (System.Exception ex)
        {
            menuButtons.ShowWarningPopup($"Error fetching leaderboard data: {ex.Message}");
            Debug.LogError($"Error fetching leaderboard data: {ex.Message}");
        }
    }

    private async Task<LeaderboardEntry> FetchUserLeaderboardEntry(DocumentSnapshot userDoc)
    {
        string username = userDoc.GetValue<string>("username");
        string userId = userDoc.Id;

        Debug.Log($"Fetching data for user: {username}");

        try
        {
            Debug.Log($"Game mode is: {gameMode}");
            // Fetch OverallWeek document
            DocumentSnapshot weekDoc = await db.Collection("userAnalytics")
                                               .Document(userId)
                                               .Collection($"games{gameMode}Analytics")
                                               .Document("OverallWeek")
                                               .GetSnapshotAsync();

            if (weekDoc.Exists)
            {
                int totalGamesPlayed = weekDoc.GetValue<int>("totalGamesPlayed");

                // Fetch evaluation percentages and overallScore
                var evaluationPercentages = weekDoc.GetValue<Dictionary<string, object>>("evaluationPercentages");

                float overallWeek = weekDoc.GetValue<float>("overallScore");
                float certificateStatus = evaluationPercentages.ContainsKey("Certificate Status")
                                          ? Convert.ToSingle(evaluationPercentages["Certificate Status"])
                                          : 0f;
                float emailMark = evaluationPercentages.ContainsKey("Email Mark")
                                  ? Convert.ToSingle(evaluationPercentages["Email Mark"])
                                  : 0f;
                float grammarCheck = evaluationPercentages.ContainsKey("Grammar Check")
                                     ? Convert.ToSingle(evaluationPercentages["Grammar Check"])
                                     : 0f;
                float suspiciousSender = evaluationPercentages.ContainsKey("Suspicious Sender")
                                         ? Convert.ToSingle(evaluationPercentages["Suspicious Sender"])
                                         : 0f;
                float urlRisk = evaluationPercentages.ContainsKey("URL Risk")
                                ? Convert.ToSingle(evaluationPercentages["URL Risk"])
                                : 0f;
                float violations = evaluationPercentages.ContainsKey("Violations")
                                   ? Convert.ToSingle(evaluationPercentages["Violations"])
                                   : 0f;

                return new LeaderboardEntry(username, totalGamesPlayed, overallWeek,
                                             certificateStatus, emailMark, grammarCheck,
                                             suspiciousSender, urlRisk, violations);
            }
            else
            {
                Debug.LogWarning($"No OverallWeek document for user {username}");
                return null; // No data to add for this user
            }
        }
        catch (System.Exception ex)
        {
            menuButtons.ShowWarningPopup($"Error fetching OverallWeek for user {username}: {ex.Message}");
            Debug.LogError($"Error fetching OverallWeek for user {username}: {ex.Message}");
            return null;
        }
    }

    private void DisplayLeaderboard(Transform leaderboardContent, List<LeaderboardEntry> entries, string metric)
    {
        Debug.Log($"Displaying {metric} leaderboard.");

        // Clear existing entries
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // Sort leaderboard entries based on the chosen metric
        switch (metric)
        {
            case "OverallWeek":
                entries.Sort((a, b) => b.OverallWeek.CompareTo(a.OverallWeek));
                break;
            case "CertificateStatus":
                entries.Sort((a, b) => b.CertificateStatus.CompareTo(a.CertificateStatus));
                break;
            case "EmailMark":
                entries.Sort((a, b) => b.EmailMark.CompareTo(a.EmailMark));
                break;
            case "GrammarCheck":
                entries.Sort((a, b) => b.GrammarCheck.CompareTo(a.GrammarCheck));
                break;
            case "SuspiciousSender":
                entries.Sort((a, b) => b.SuspiciousSender.CompareTo(a.SuspiciousSender));
                break;
            case "URLRisk":
                entries.Sort((a, b) => b.URLRisk.CompareTo(a.URLRisk));
                break;
            case "Violations":
                entries.Sort((a, b) => b.Violations.CompareTo(a.Violations));
                break;
        }

        // Instantiate and display new leaderboard entries
        for (int i = 0; i < entries.Count; i++)
        {
            GameObject newEntry = Instantiate(leaderboardEntryPrefab, leaderboardContent);

            TMP_Text usernameText = newEntry.transform.Find("usernameText").GetComponent<TMP_Text>();
            TMP_Text totalGamesText = newEntry.transform.Find("gamesText").GetComponent<TMP_Text>();
            TMP_Text scoreText = newEntry.transform.Find("scoreText").GetComponent<TMP_Text>();
            Image rankIcon = newEntry.transform.Find("rankIcon").GetComponent<Image>();

            usernameText.text = entries[i].Username;
            totalGamesText.text = entries[i].TotalGamesPlayed.ToString();

            // Display the chosen metric (score)
            switch (metric)
            {
                case "OverallWeek":
                    scoreText.text = entries[i].OverallWeek.ToString();
                    break;
                case "CertificateStatus":
                    scoreText.text = entries[i].CertificateStatus.ToString();
                    break;
                case "EmailMark":
                    scoreText.text = entries[i].EmailMark.ToString();
                    break;
                case "GrammarCheck":
                    scoreText.text = entries[i].GrammarCheck.ToString();
                    break;
                case "SuspiciousSender":
                    scoreText.text = entries[i].SuspiciousSender.ToString();
                    break;
                case "URLRisk":
                    scoreText.text = entries[i].URLRisk.ToString();
                    break;
                case "Violations":
                    scoreText.text = entries[i].Violations.ToString();
                    break;
            }
            scoreText.text += "%";

            // Assign rank icons for top 3
            if (i == 0)
            {
                rankIcon.sprite = firstPlaceIcon;
                rankIcon.gameObject.SetActive(true);
            }
            else if (i == 1)
            {
                rankIcon.sprite = secondPlaceIcon;
                rankIcon.gameObject.SetActive(true);
            }
            else if (i == 2)
            {
                rankIcon.sprite = thirdPlaceIcon;
                rankIcon.gameObject.SetActive(true);
            }
            else
            {
                rankIcon.GetComponent<Image>().enabled = false;
            }
        }
    }
}

public class LeaderboardEntry
{
    public string Username;
    public int TotalGamesPlayed;
    public float OverallWeek;
    public float CertificateStatus;
    public float EmailMark;
    public float GrammarCheck;
    public float SuspiciousSender;
    public float URLRisk;
    public float Violations;

    public LeaderboardEntry(string username, int totalGamesPlayed, float overallWeek,
                             float certificateStatus, float emailMark, float grammarCheck,
                             float suspiciousSender, float urlRisk, float violations)
    {
        Username = username;
        TotalGamesPlayed = totalGamesPlayed;
        OverallWeek = overallWeek;
        CertificateStatus = certificateStatus;
        EmailMark = emailMark;
        GrammarCheck = grammarCheck;
        SuspiciousSender = suspiciousSender;
        URLRisk = urlRisk;
        Violations = violations;
    }
}
