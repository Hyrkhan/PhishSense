using Firebase.Firestore;
using System.Collections.Generic;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using System.Linq;
using Firebase.Auth;
using System;
using System.Threading.Tasks;
using UnityEngine.Analytics;

public class AnalyticsFetcher : MonoBehaviour
{
    private Dictionary<string, (int correct, int total)> questionPerformance = new Dictionary<string, (int, int)>();
    private FirebaseFirestore db;

    public gridPlotting gridPlotting;
    public accuracyGridPlotting accuracyGridPlotting;

    public GameObject loadingbuffer;
    public MenuButtons menuButtons;

    public static class AnalyticsStorage
    {
        public static Dictionary<string, int> DayScores { get; set; } = new Dictionary<string, int>
        {
            { "SundayData", 0 },
            { "MondayData", 0 },
            { "TuesdayData", 0 },
            { "WednesdayData", 0 },
            { "ThursdayData", 0 },
            { "FridayData", 0 },
            { "SaturdayData", 0 }
        };
        public static Dictionary<string, float[]> DailyEvaluationScores { get; set; } = new Dictionary<string, float[]>
        {
            { "SundayData", new float[6] },
            { "MondayData", new float[6] },
            { "TuesdayData", new float[6] },
            { "WednesdayData", new float[6] },
            { "ThursdayData", new float[6] },
            { "FridayData", new float[6] },
            { "SaturdayData", new float[6] }
        };

        public static void Reset()
        {
            DayScores = new Dictionary<string, int>
        {
            { "SundayData", 0 },
            { "MondayData", 0 },
            { "TuesdayData", 0 },
            { "WednesdayData", 0 },
            { "ThursdayData", 0 },
            { "FridayData", 0 },
            { "SaturdayData", 0 }
        };

            DailyEvaluationScores = new Dictionary<string, float[]>
        {
            { "SundayData", new float[6] },
            { "MondayData", new float[6] },
            { "TuesdayData", new float[6] },
            { "WednesdayData", new float[6] },
            { "ThursdayData", new float[6] },
            { "FridayData", new float[6] },
            { "SaturdayData", new float[6] }
        };
        }
    }

    private string gameMode;

    private void Start()
    {
        gameMode = "Easy";
        AnalyticsStorage.Reset();

        db = FirebaseFirestore.DefaultInstance;
        FetchAnalyticsFromFirestore();

        // Check and reset analytics if a new week has started
        CheckAndResetWeeklyAnalytics();
    }

    public void FetchAnalytics(string mode)
    {
        gameMode = mode;
        Debug.Log($"{gameMode} game moded");

        AnalyticsStorage.Reset();
        db = FirebaseFirestore.DefaultInstance;
        FetchAnalyticsFromFirestore();
    }

    /// <summary>
    /// Checks if it's a new week based on Philippine timezone and resets analytics.
    /// </summary>
    private void CheckAndResetWeeklyAnalytics()
    {
        // Get current Philippine Time (UTC+8)
        DateTime currentTimeInPhilippines = GetPhilippineTime();

        // Retrieve last reset date from PlayerPrefs
        string lastResetDate = PlayerPrefs.GetString("LastResetDate", ""); // Default: Empty
        DateTime lastResetDateTime;

        if (string.IsNullOrEmpty(lastResetDate) || !DateTime.TryParse(lastResetDate, out lastResetDateTime))
        {
            // If no valid reset date exists, perform reset immediately
            ResetAll();
            SaveResetDate(currentTimeInPhilippines);
        }
        else
        {
            // Check if the current day is Monday and a new week has started
            if (currentTimeInPhilippines.DayOfWeek == DayOfWeek.Monday && currentTimeInPhilippines.Date > lastResetDateTime.Date)
            {
                ResetAll();
                SaveResetDate(currentTimeInPhilippines);
            }
        }
    }

    /// <summary>
    /// Gets the current time in the Philippine Timezone (UTC+8).
    /// </summary>
    private DateTime GetPhilippineTime()
    {
        DateTime utcNow = DateTime.UtcNow;
        TimeZoneInfo philippineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"); // UTC+8 equivalent
        return TimeZoneInfo.ConvertTimeFromUtc(utcNow, philippineTimeZone);
    }

    /// <summary>
    /// Saves the last reset date in PlayerPrefs.
    /// </summary>
    private void SaveResetDate(DateTime resetDate)
    {
        PlayerPrefs.SetString("LastResetDate", resetDate.Date.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();
    }

    public void FetchAnalyticsFromFirestore()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.LogError("No user logged in.");
            return;
        }
        loadingbuffer.SetActive(true);

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        db.Collection("userAnalytics").Document(user.UserId).Collection($"games{gameMode}Analytics").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                QuerySnapshot querySnapshot = task.Result;

                foreach (DocumentSnapshot document in querySnapshot.Documents)
                {
                    if (document.Exists)
                    {
                        string dayId = document.Id;

                        // Skip the "OverallWeek" document
                        if (dayId == "OverallWeek") continue;

                        // Update DayScores for overallScore
                        if (document.TryGetValue("overallScore", out float overallScore))
                        {
                            AnalyticsStorage.DayScores[dayId] = Mathf.RoundToInt(overallScore);
                        }

                        // Initialize default scores for the day
                        float[] evaluationPercentages = new float[6];

                        // Attempt to fetch the evaluationPercentages map
                        if (document.TryGetValue("evaluationPercentages", out Dictionary<string, object> percentagesMap))
                        {
                            percentagesMap.TryGetValue("Certificate Status", out object certStatus);
                            percentagesMap.TryGetValue("Email Mark", out object emailMark);
                            percentagesMap.TryGetValue("Grammar Check", out object grammarCheck);
                            percentagesMap.TryGetValue("Suspicious Sender", out object suspiciousSender);
                            percentagesMap.TryGetValue("URL Risk", out object urlRisk);
                            percentagesMap.TryGetValue("Violations", out object violations);

                            // Safely parse and store the values into the array
                            evaluationPercentages[0] = grammarCheck != null ? Convert.ToSingle(grammarCheck) : 0f;
                            evaluationPercentages[1] = suspiciousSender != null ? Convert.ToSingle(suspiciousSender) : 0f;
                            evaluationPercentages[2] = urlRisk != null ? Convert.ToSingle(urlRisk) : 0f;
                            evaluationPercentages[3] = violations != null ? Convert.ToSingle(violations) : 0f;
                            evaluationPercentages[4] = certStatus != null ? Convert.ToSingle(certStatus) : 0f;
                            evaluationPercentages[5] = emailMark != null ? Convert.ToSingle(emailMark) : 0f;

                            Debug.Log($"Fetched scores for {dayId}: {string.Join(", ", evaluationPercentages)}");
                            AnalyticsStorage.DailyEvaluationScores[dayId] = evaluationPercentages;
                        }
                        else
                        {
                            loadingbuffer.SetActive(false);
                            Debug.LogWarning($"Unexpected day ID: {dayId}");
                        }
                    }
                }

                Debug.Log("Analytics fetched and stored successfully.");
                gridPlotting.startPlot();
                accuracyGridPlotting.startPlotting();
                loadingbuffer.SetActive(false);
            }
            else
            {
                loadingbuffer.SetActive(false);
                menuButtons.ShowWarningPopup("Failed to fetch analytics: " + task.Exception);
                Debug.LogError("Failed to fetch analytics: " + task.Exception);
            }
        });
    }

    public void ResetAll()
    {
        ResetPlayerAnalytics("Easy");
        ResetPlayerAnalytics("Normal");
        ResetPlayerAnalytics("Hard");
    }

    public void ResetPlayerAnalytics(string mode)
    {
        gameMode = mode;
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user == null)
        {
            Debug.Log("No user logged in.");
            return;
        }
        loadingbuffer.SetActive(true);
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        DocumentReference userDocRef = db.Collection("userAnalytics").Document(user.UserId);
        CollectionReference gamesAnalyticsRef = userDocRef.Collection($"games{gameMode}Analytics");

        List<Task> tasks = new List<Task>();

        // Step 1: Fetch all day documents (MondayData, TuesdayData, etc.)
        tasks.Add(gamesAnalyticsRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                QuerySnapshot dayDocumentsSnapshot = task.Result;

                if (dayDocumentsSnapshot.Documents.Any())
                {
                    foreach (DocumentSnapshot dayDocument in dayDocumentsSnapshot.Documents)
                    {
                        string dayDocId = dayDocument.Id;
                        DocumentReference dayDocRef = gamesAnalyticsRef.Document(dayDocId);

                        // Step 2: Delete the "Games" collection inside each day document
                        tasks.Add(dayDocRef.Collection("Games").GetSnapshotAsync().ContinueWithOnMainThread(gamesTask =>
                        {
                            if (gamesTask.IsCompleted)
                            {
                                QuerySnapshot gamesSnapshot = gamesTask.Result;

                                foreach (DocumentSnapshot gameDoc in gamesSnapshot.Documents)
                                {
                                    tasks.Add(gameDoc.Reference.DeleteAsync().ContinueWithOnMainThread(deleteTask =>
                                    {
                                        if (!deleteTask.IsCompletedSuccessfully)
                                        {
                                            menuButtons.ShowWarningPopup($"Failed to delete game document: {deleteTask.Exception}");
                                            Debug.LogError($"Failed to delete game document: {deleteTask.Exception}");
                                        }
                                    }));
                                }
                            }
                            else
                            {
                                menuButtons.ShowWarningPopup($"Failed to fetch games in {dayDocId}: {gamesTask.Exception}");
                                Debug.LogError($"Failed to fetch games in {dayDocId}: {gamesTask.Exception}");
                            }
                        }));

                        // Step 3: Reset the fields in the day document
                        Dictionary<string, object> resetFields = new Dictionary<string, object>
                    {
                        { "evaluationPercentages", new Dictionary<string, float>
                            {
                                { "Certificate Status", 0f },
                                { "Email Mark", 0f },
                                { "Grammar Check", 0f },
                                { "Suspicious Sender", 0f },
                                { "URL Risk", 0f },
                                { "Violations", 0f }
                            }
                        },
                        { "overallScore", 0f },
                        { "totalGamesPlayed", 0 },
                        { "timestamp", null }
                    };

                        tasks.Add(dayDocRef.UpdateAsync(resetFields).ContinueWithOnMainThread(updateTask =>
                        {
                            if (!updateTask.IsCompletedSuccessfully)
                            {
                                menuButtons.ShowWarningPopup($"Failed to reset fields for {dayDocId}: {updateTask.Exception}");
                                Debug.LogError($"Failed to reset fields for {dayDocId}: {updateTask.Exception}");
                            }
                        }));
                    }
                }
                else
                {
                    Debug.LogWarning("No day documents found in 'gamesAnalytics'.");
                }
            }
            else
            {
                menuButtons.ShowWarningPopup($"Failed to fetch day documents: {task.Exception}");
                Debug.LogError($"Failed to fetch day documents: {task.Exception}");
            }
        }));

        // Step 4: Reset user root document
        tasks.Add(userDocRef.SetAsync(new Dictionary<string, object>
    {
        { "username", user.DisplayName ?? "Guest" },
        { "lastGameTimestamp", null }
    }).ContinueWithOnMainThread(task =>
    {
        if (!task.IsCompletedSuccessfully)
        {
            menuButtons.ShowWarningPopup($"Failed to reset user root document: {task.Exception}");
            Debug.LogError($"Failed to reset user root document: {task.Exception}");
        }
    }));

        // Deactivate loading buffer when all tasks are completed
        Task.WhenAll(tasks).ContinueWithOnMainThread(_ =>
        {
            loadingbuffer.SetActive(false);
            Debug.Log("All tasks completed.");
        });
    }

}
