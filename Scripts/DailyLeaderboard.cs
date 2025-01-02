using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

public class DailyLeaderboard : MonoBehaviour

{
    [Header("Daily Leaderboard UI Content")]
    [SerializeField] private Transform dailyOverallLeaderboardContent;
    [SerializeField] private Transform dailyCertificateLeaderboardContent;
    [SerializeField] private Transform dailyEmailMarkLeaderboardContent;
    [SerializeField] private Transform dailyGrammarCheckLeaderboardContent;
    [SerializeField] private Transform dailySuspiciousSenderLeaderboardContent;
    [SerializeField] private Transform dailyURLRiskLeaderboardContent;
    [SerializeField] private Transform dailyViolationsLeaderboardContent;

    [Header("Leaderboard Entry Prefab")]
    [SerializeField] private GameObject leaderboardEntryPrefab;

    [Header("Rank Icons")]
    [SerializeField] private Sprite firstPlaceIcon;
    [SerializeField] private Sprite secondPlaceIcon;
    [SerializeField] private Sprite thirdPlaceIcon;

    private FirebaseFirestore db;

    private string gameMode;
    public GameObject loadingbuffer;
    public rankingButtons rankingButtons;

    private void Start()
    {
        gameMode = "Easy";
        db = FirebaseFirestore.DefaultInstance;
        FetchAndDisplayDailyLeaderboard();
    }

    public void DisplayDailyboard(string mode)
    {
        gameMode = mode;
        db = FirebaseFirestore.DefaultInstance;
        FetchAndDisplayDailyLeaderboard();
    }

    private async void FetchAndDisplayDailyLeaderboard()
    {
        try
        {
            loadingbuffer.SetActive(true);
            // Get today's day of the week
            string today = DateTime.Now.DayOfWeek.ToString() + "Data";

            QuerySnapshot usersSnapshot = await db.Collection("userAnalytics").GetSnapshotAsync();
            Debug.Log($"Fetched {usersSnapshot.Count} users from Firestore.");

            List<DailyLeaderboardEntry> leaderboardEntries = new List<DailyLeaderboardEntry>();

            foreach (DocumentSnapshot userDoc in usersSnapshot.Documents)
            {
                DailyLeaderboardEntry entry = await FetchUserDailyLeaderboardEntry(userDoc, today);
                if (entry != null)
                {
                    leaderboardEntries.Add(entry);
                }
            }

            // Display all daily leaderboards
            DisplayLeaderboard(dailyOverallLeaderboardContent, leaderboardEntries, "OverallDay");
            DisplayLeaderboard(dailyCertificateLeaderboardContent, leaderboardEntries, "CertificateStatus");
            DisplayLeaderboard(dailyEmailMarkLeaderboardContent, leaderboardEntries, "EmailMark");
            DisplayLeaderboard(dailyGrammarCheckLeaderboardContent, leaderboardEntries, "GrammarCheck");
            DisplayLeaderboard(dailySuspiciousSenderLeaderboardContent, leaderboardEntries, "SuspiciousSender");
            DisplayLeaderboard(dailyURLRiskLeaderboardContent, leaderboardEntries, "URLRisk");
            DisplayLeaderboard(dailyViolationsLeaderboardContent, leaderboardEntries, "Violations");

            rankingButtons.ResetScrollbars(0);
            loadingbuffer.SetActive(false);
            Debug.Log("Finished displaying daily leaderboards.");
        }
        catch (Exception ex)
        {
            loadingbuffer.SetActive(false);
            Debug.LogError($"Error fetching daily leaderboard data: {ex.Message}");
        }
    }

    private async Task<DailyLeaderboardEntry> FetchUserDailyLeaderboardEntry(DocumentSnapshot userDoc, string day)
    {
        string username = userDoc.GetValue<string>("username");
        string userId = userDoc.Id;

        int totalGamesPlayed = 0;

        Debug.Log($"Fetching daily data for user: {username}");

        DocumentSnapshot weekDoc = await db.Collection("userAnalytics")
                                               .Document(userId)
                                               .Collection($"games{gameMode}Analytics")
                                               .Document("OverallWeek")
                                               .GetSnapshotAsync();

        if (weekDoc.Exists)
        {
            totalGamesPlayed = await GetTotalGamesPlayed(userDoc, day);
        }
        else
        {
            Debug.LogWarning($"No OverallWeek document for user {username}");
        }

        try
        {
            // Fetch day-specific document
            DocumentSnapshot dailyDoc = await db.Collection("userAnalytics")
                                               .Document(userId)
                                               .Collection($"games{gameMode}Analytics")
                                               .Document(day)
                                               .GetSnapshotAsync();

            if (dailyDoc.Exists)
            {
                var evaluationPercentages = dailyDoc.GetValue<Dictionary<string, object>>("evaluationPercentages");

                float overallDay = dailyDoc.GetValue<float>("overallScore");
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

                return new DailyLeaderboardEntry(username, totalGamesPlayed, overallDay,
                                             certificateStatus, emailMark, grammarCheck,
                                             suspiciousSender, urlRisk, violations);
            }
            else
            {
                Debug.LogWarning($"No {day} document for user {username}");
                return null; // No data to add for this user
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching {day} data for user {username}: {ex.Message}");
            return null;
        }
    }

    private async Task<int> GetTotalGamesPlayed(DocumentSnapshot userDoc, string day)
    {
        try
        {
            // Access the specific day's "Games" collection
            QuerySnapshot gamesSnapshot = await db.Collection("userAnalytics")
                .Document(userDoc.Id)                                // Current user ID
                .Collection($"games{gameMode}Analytics")           // Example: gamesEasyAnalytics
                .Document(day)                                     // Example: FridayData
                .Collection("Games")                               // The subcollection "Games"
                .GetSnapshotAsync();

            // Return the total number of documents (games) in the "Games" collection
            return gamesSnapshot.Count;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error getting total games played: {ex.Message}");
            return 0; // Return 0 in case of an error
        }
    }

    private void DisplayLeaderboard(Transform leaderboardContent, List<DailyLeaderboardEntry> entries, string metric)
    {
        Debug.Log($"Displaying {metric} leaderboard.");

        // Clear existing entries
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject); // Clear existing entries
        }

        // Sort leaderboard entries based on the chosen metric
        switch (metric)
        {
            case "OverallDay":
                entries.Sort((a, b) => b.OverallDay.CompareTo(a.OverallDay));
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
            DailyLeaderboardEntry entry = entries[i];
            GameObject newEntry = Instantiate(leaderboardEntryPrefab, leaderboardContent);

            TMP_Text usernameText = newEntry.transform.Find("usernameText").GetComponent<TMP_Text>();
            TMP_Text totalGamesText = newEntry.transform.Find("gamesText").GetComponent<TMP_Text>();
            TMP_Text scoreText = newEntry.transform.Find("scoreText").GetComponent<TMP_Text>();
            Image rankIcon = newEntry.transform.Find("rankIcon").GetComponent<Image>(); // Ensure rankIcon exists

            usernameText.text = entry.Username;
            totalGamesText.text = entry.TotalGamesPlayed.ToString();

            // Display the chosen metric (score)
            switch (metric)
            {
                case "OverallDay":
                    scoreText.text = entry.OverallDay.ToString();
                    break;
                case "CertificateStatus":
                    scoreText.text = entry.CertificateStatus.ToString();
                    break;
                case "EmailMark":
                    scoreText.text = entry.EmailMark.ToString();
                    break;
                case "GrammarCheck":
                    scoreText.text = entry.GrammarCheck.ToString();
                    break;
                case "SuspiciousSender":
                    scoreText.text = entry.SuspiciousSender.ToString();
                    break;
                case "URLRisk":
                    scoreText.text = entry.URLRisk.ToString();
                    break;
                case "Violations":
                    scoreText.text = entry.Violations.ToString();
                    break;
            }
            scoreText.text += "%";

            // Handle rank icons
            switch (i)
            {
                case 0: // 1st place
                    rankIcon.sprite = firstPlaceIcon; // Assign 1st place sprite
                    rankIcon.gameObject.SetActive(true);
                    break;
                case 1: // 2nd place
                    rankIcon.sprite = secondPlaceIcon; // Assign 2nd place sprite
                    rankIcon.gameObject.SetActive(true);
                    break;
                case 2: // 3rd place
                    rankIcon.sprite = thirdPlaceIcon; // Assign 3rd place sprite
                    rankIcon.gameObject.SetActive(true);
                    break;
                default: // No icon for others
                    rankIcon.gameObject.SetActive(false);
                    break;
            }
        }
    }

}

public class DailyLeaderboardEntry
{
    public string Username;
    public int TotalGamesPlayed;
    public float OverallDay;
    public float CertificateStatus;
    public float EmailMark;
    public float GrammarCheck;
    public float SuspiciousSender;
    public float URLRisk;
    public float Violations;

    public DailyLeaderboardEntry(string username, int totalGamesPlayed, float overallDay,
                             float certificateStatus, float emailMark, float grammarCheck,
                             float suspiciousSender, float urlRisk, float violations)
    {
        Username = username;
        TotalGamesPlayed = totalGamesPlayed;
        OverallDay = overallDay;
        CertificateStatus = certificateStatus;
        EmailMark = emailMark;
        GrammarCheck = grammarCheck;
        SuspiciousSender = suspiciousSender;
        URLRisk = urlRisk;
        Violations = violations;
    }
}