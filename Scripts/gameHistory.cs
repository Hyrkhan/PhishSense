using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Auth;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class gameHistory : MonoBehaviour
{
    [SerializeField] private GameObject gameHistoryEntryPrefab; // Prefab for displaying game history entries
    [SerializeField] private Transform gameHistoryContent; // Parent object for game history entries

    private FirebaseFirestore db;
    private string[] gameModes = { "gamesEasyAnalytics", "gamesNormalAnalytics", "gamesHardAnalytics" }; // Predefined game modes
    private string[] days = { "MondayData", "TuesdayData", "WednesdayData", "ThursdayData", "FridayData", "SaturdayData", "SundayData" }; // Predefined days

    public GameObject loadingbuffer;
    public MenuButtons menuButtons;
    public Scrollbar historyScroll;

    public void ResetScrollBar()
    {
        // Make sure the scrollbar is active before resetting its value
        historyScroll.gameObject.SetActive(true);

        // Optionally force layout recalculation
        LayoutRebuilder.ForceRebuildLayoutImmediate(historyScroll.GetComponentInParent<RectTransform>());

        // Reset scrollbar value to 1
        historyScroll.value = 1f;
    }

    private IEnumerator ResetScrollBarAfterDelay()
    {
        // Wait for one frame to make sure everything is fully active
        yield return null;

        // Now reset the scrollbar
        ResetScrollBar();
    }

    public void getHistory()
    {
        db = FirebaseFirestore.DefaultInstance;
        FetchAndDisplayGameHistory();
    }

    private async void FetchAndDisplayGameHistory()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

        if (user == null)
        {
            Debug.LogError("No user logged in.");
            return;
        }

        loadingbuffer.SetActive(true);

        // Clear existing child objects in gameHistoryContent
        foreach (Transform child in gameHistoryContent)
        {
            Destroy(child.gameObject);
        }

        string userId = user.UserId; // Dynamically fetch the logged-in user ID
        List<GameHistoryEntry> historyEntries = new List<GameHistoryEntry>();
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        string[] gameModes = { "gamesEasyAnalytics", "gamesNormalAnalytics", "gamesHardAnalytics" };
        string[] days = { "MondayData", "TuesdayData", "WednesdayData", "ThursdayData", "FridayData", "SaturdayData", "SundayData" };

        try
        {
            foreach (string gameMode in gameModes)
            {
                CollectionReference gameModeCollection = db.Collection("userAnalytics").Document(userId).Collection(gameMode);

                foreach (string day in days)
                {
                    DocumentReference dayDocRef = gameModeCollection.Document(day);

                    // Check if the day document exists
                    DocumentSnapshot dayDoc = await dayDocRef.GetSnapshotAsync();

                    if (!dayDoc.Exists)
                    {
                        Debug.Log($"No data for {gameMode} on {day}");
                        continue;
                    }

                    CollectionReference gamesCollection = dayDocRef.Collection("Games");

                    // Check if the Games collection exists
                    QuerySnapshot gamesSnapshot;
                    try
                    {
                        gamesSnapshot = await gamesCollection.GetSnapshotAsync();
                    }
                    catch
                    {
                        Debug.Log($"No Games collection for {gameMode} on {day}");
                        continue;
                    }

                    // Parse each game document
                    foreach (DocumentSnapshot gameDoc in gamesSnapshot.Documents)
                    {
                        GameHistoryEntry entry = ParseGameHistoryEntry(gameDoc, gameMode.Replace("games", "").Replace("Analytics", ""), day);
                        if (entry != null)
                        {
                            historyEntries.Add(entry);
                        }
                    }
                }
            }

            // Sort entries by timestamp (latest first)
            historyEntries.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));

            // Display entries
            foreach (GameHistoryEntry entry in historyEntries)
            {
                GameObject newEntry = Instantiate(gameHistoryEntryPrefab, gameHistoryContent);

                TMP_Text timestampText = newEntry.transform.Find("timestampText").GetComponent<TMP_Text>();
                TMP_Text gameModeText = newEntry.transform.Find("gameModeText").GetComponent<TMP_Text>();
                TMP_Text scoreText = newEntry.transform.Find("scoreText").GetComponent<TMP_Text>();
                TMP_Text dayText = newEntry.transform.Find("dayText").GetComponent<TMP_Text>();

                timestampText.text = entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                gameModeText.text = entry.GameMode;
                scoreText.text = $"{entry.Score}/{entry.Total}";
                dayText.text = entry.Day.Replace("Data", "");
            }
            loadingbuffer.SetActive(false);
            Debug.Log("Game history displayed successfully.");
            StartCoroutine(ResetScrollBarAfterDelay());
        }
        catch (Exception ex)
        {
            loadingbuffer.SetActive(false);
            //check for no internet
            menuButtons.ShowWarningPopup($"Error fetching game history: {ex.Message}");
            //Debug.LogError($"Error fetching game history: {ex.Message}");
            StartCoroutine(ResetScrollBarAfterDelay());
        }
        
    }

    private GameHistoryEntry ParseGameHistoryEntry(DocumentSnapshot gameDoc, string gameMode, string day)
    {
        if (!gameDoc.Exists)
        {
            return null;
        }

        try
        {
            long totalScore = 0;
            long totalPossible = 0;

            // Retrieve the analytics map
            if (gameDoc.ContainsField("analytics"))
            {
                Dictionary<string, Dictionary<string, object>> analytics =
                    gameDoc.GetValue<Dictionary<string, Dictionary<string, object>>>("analytics");

                foreach (var category in analytics)
                {
                    if (category.Key == "Email Mark")
                    {
                        // Multiply Email Mark by 5
                        totalScore += category.Value.ContainsKey("correct") ? Convert.ToInt64(category.Value["correct"]) * 5 : 0;
                        totalPossible += category.Value.ContainsKey("total") ? Convert.ToInt64(category.Value["total"]) * 5 : 0;
                    }
                    else
                    {
                        // Other categories
                        totalScore += category.Value.ContainsKey("correct") ? Convert.ToInt64(category.Value["correct"]) : 0;
                        totalPossible += category.Value.ContainsKey("total") ? Convert.ToInt64(category.Value["total"]) : 0;
                    }
                }
            }

            // Get the timestamp and convert to local time
            Timestamp timestamp = gameDoc.GetValue<Timestamp>("timestamp");

            if (timestamp == null)
            {
                Debug.LogWarning($"Skipping game document with missing timestamp in {gameMode} on {day}");
                return null;
            }

            DateTime localDateTime = timestamp.ToDateTime().ToLocalTime();

            return new GameHistoryEntry
            {
                GameMode = gameMode,
                Day = day,
                Score = (int)totalScore,
                Total = (int)totalPossible,
                Timestamp = localDateTime
            };
        }
        catch (Exception ex)
        {
            menuButtons.ShowWarningPopup($"Error parsing game document: {ex.Message}");
            Debug.LogError($"Error parsing game document: {ex.Message}");
            return null;
        }
    }
}

public class GameHistoryEntry
{
    public string GameMode { get; set; }
    public string Day { get; set; }
    public int Score { get; set; }
    public int Total { get; set; }
    public DateTime Timestamp { get; set; }
}
