using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using System.Threading.Tasks;
using System;
using Firebase.Extensions;
using Firebase.Auth;
using System.Linq;


public class uploadFirebase : MonoBehaviour
{
    public evaluationScript evaluationScript;

    private FirebaseFirestore db;
    private string gameMode;

    private void Awake()
    {
        gameMode = PlayerPrefs.GetString("GameMode");
        db = FirebaseFirestore.DefaultInstance;
    }

    // Uploads game data to the appropriate day in Firestore
    public void UploadGameData(string userId)
    {
        string dayOfWeek = DateTime.UtcNow.DayOfWeek.ToString() + "Data";
        string today = DateTime.UtcNow.ToString("yyyy-MM-dd");

        // Format the game data
        var firebaseData = ConvertToFirestoreFormat();
        var evaluationPercentages = CalculateEvaluationPercentages();
        float overallScore = CalculateOverallScore();

        // Reference the day's collection
        CollectionReference dayCollectionRef = db
            .Collection("userAnalytics")
            .Document(userId)
            .Collection($"games{gameMode}Analytics")
            .Document(dayOfWeek)
            .Collection("Games");

        // Add the game data for the current session
        dayCollectionRef.AddAsync(new Dictionary<string, object>
        {
            { "analytics", firebaseData },
            { "evaluationPercentages", evaluationPercentages },
            { "overallScore", overallScore },
            { "date", today },
            { "timestamp", Timestamp.GetCurrentTimestamp() }
        }).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Game session successfully uploaded for " + dayOfWeek);

                // Update total games played
                //UpdateTotalGamesPlayed(userId);

                // Update daily averages
                UpdateDailyAverages(userId, dayOfWeek);
            }
            else
            {
                Debug.LogError("Failed to upload game session: " + task.Exception);
            }
        });
    }


    private void UpdateTotalGamesPlayed(string userId)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // Reference the OverallWeek document for the current game mode
        DocumentReference overallWeekDocRef = db
            .Collection("userAnalytics")
            .Document(userId)
            .Collection($"games{gameMode}Analytics")
            .Document("OverallWeek");

        // Atomically increment totalGamesPlayed in OverallWeek
        overallWeekDocRef.UpdateAsync(new Dictionary<string, object>
        {
            { "totalGamesPlayed", FieldValue.Increment(1) }
        }).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"totalGamesPlayed updated successfully in OverallWeek for {gameMode} mode.");
            }
            else
            {
                Debug.LogError("Failed to update totalGamesPlayed: " + task.Exception);
            }
        });
    }




    // Updates daily averages in the {DayData} document
    private void UpdateDailyAverages(string userId, string dayOfWeek)
    {
        CollectionReference dayCollectionRef = db
            .Collection("userAnalytics")
            .Document(userId)
            .Collection($"games{gameMode}Analytics")
            .Document(dayOfWeek)
            .Collection("Games");

        dayCollectionRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                var gameDocs = task.Result;
                float totalScore = 0f;
                int gameCount = 0;
                var evaluationSum = new Dictionary<string, float>();

                foreach (var doc in gameDocs.Documents)
                {
                    totalScore += doc.GetValue<float>("overallScore");
                    gameCount++;

                    // Aggregate evaluation percentages
                    var evalPercentages = doc.GetValue<Dictionary<string, object>>("evaluationPercentages");
                    foreach (var eval in evalPercentages)
                    {
                        string evalKey = eval.Key;
                        float evalValue = Convert.ToSingle(eval.Value);

                        if (!evaluationSum.ContainsKey(evalKey))
                            evaluationSum[evalKey] = 0f;

                        evaluationSum[evalKey] += evalValue;
                    }
                }

                // Calculate averages
                float dailyScore = gameCount > 0 ? (float)Math.Round(totalScore / gameCount, 2) : 0f;
                var dailyEvaluations = new Dictionary<string, float>();
                foreach (var eval in evaluationSum)
                {
                    dailyEvaluations[eval.Key] = gameCount > 0 ? (float)Math.Round(eval.Value / gameCount, 2) : 0f;
                }

                // Update the day's document with averages
                DocumentReference dayDocRef = db
                    .Collection("userAnalytics")
                    .Document(userId)
                    .Collection($"games{gameMode}Analytics")
                    .Document(dayOfWeek);

                dayDocRef.SetAsync(new Dictionary<string, object>
                {
                    { "overallScore", dailyScore },
                    { "evaluationPercentages", dailyEvaluations },
                    { "timestamp", Timestamp.GetCurrentTimestamp() }
                }).ContinueWithOnMainThread(updateTask =>
                {
                    if (updateTask.IsCompleted)
                    {
                        Debug.Log("Daily averages updated for " + dayOfWeek);
                        UpdateWeeklyAverages(userId); // Update weekly averages
                    }
                    else
                    {
                        Debug.LogError("Failed to update daily averages: " + updateTask.Exception);
                    }
                });
            }
            else
            {
                Debug.LogError("Failed to retrieve daily game data: " + task.Exception);
            }
        });
    }

    // Updates the weekly averages in the OverallWeek document
    private void UpdateWeeklyAverages(string userId)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        CollectionReference analyticsCollectionRef = db
            .Collection("userAnalytics")
            .Document(userId)
            .Collection($"games{gameMode}Analytics");

        // List of valid day document IDs
        string[] dayDocsIds = { "MondayData", "TuesdayData", "WednesdayData", "ThursdayData", "FridayData", "SaturdayData", "SundayData" };

        analyticsCollectionRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                var dayDocs = task.Result;
                float totalScore = 0f;
                int activeDaysCount = 0; // Count only days with data > 0
                var evaluationSum = new Dictionary<string, float>();

                foreach (var doc in dayDocs.Documents)
                {
                    if (!dayDocsIds.Contains(doc.Id)) continue; // Ignore non-day documents

                    // Check if the day document has valid data
                    float dayScore = doc.GetValue<float>("overallScore");
                    if (dayScore > 0)
                    {
                        totalScore += dayScore;
                        activeDaysCount++;

                        var evalPercentages = doc.GetValue<Dictionary<string, object>>("evaluationPercentages");
                        foreach (var eval in evalPercentages)
                        {
                            string evalKey = eval.Key;
                            float evalValue = Convert.ToSingle(eval.Value);

                            if (!evaluationSum.ContainsKey(evalKey))
                                evaluationSum[evalKey] = 0f;

                            evaluationSum[evalKey] += evalValue;
                        }
                    }
                }

                // Calculate averages based on active days
                float weeklyScore = activeDaysCount > 0 ? (float)Math.Round(totalScore / activeDaysCount, 2) : 0f;
                var weeklyEvaluations = new Dictionary<string, float>();
                foreach (var eval in evaluationSum)
                {
                    weeklyEvaluations[eval.Key] = activeDaysCount > 0 ? (float)Math.Round(eval.Value / activeDaysCount, 2) : 0f;
                }

                // Update or Set the OverallWeek document
                DocumentReference weekDocRef = db
                    .Collection("userAnalytics")
                    .Document(userId)
                    .Collection($"games{gameMode}Analytics")
                    .Document("OverallWeek");

                // Fetch the document snapshot to check for existing fields
                weekDocRef.GetSnapshotAsync().ContinueWithOnMainThread(snapshotTask =>
                {
                    if (snapshotTask.IsCompleted && !snapshotTask.IsFaulted)
                    {
                        DocumentSnapshot weekDoc = snapshotTask.Result;

                        // Check if the document exists and contains the necessary fields
                        bool documentExists = weekDoc.Exists;
                        bool hasFields = documentExists && weekDoc.ContainsField("overallScore");

                        if (hasFields)
                        {
                            // If fields exist, perform UpdateAsync
                            weekDocRef.UpdateAsync(new Dictionary<string, object>
                        {
                            { "overallScore", weeklyScore },
                            { "evaluationPercentages", weeklyEvaluations },
                            { "totalGamesPlayed", FieldValue.Increment(1) },
                            { "timestamp", Timestamp.GetCurrentTimestamp() }
                        }).ContinueWithOnMainThread(updateTask =>
                        {
                            if (updateTask.IsCompleted)
                            {
                                Debug.Log("Weekly averages updated successfully.");
                            }
                            else
                            {
                                Debug.LogError("Failed to update weekly averages: " + updateTask.Exception);
                            }
                        });
                        }
                        else
                        {
                            // If the document doesn't have the required fields, perform SetAsync
                            weekDocRef.SetAsync(new Dictionary<string, object>
                        {
                            { "overallScore", weeklyScore },
                            { "evaluationPercentages", weeklyEvaluations },
                            { "totalGamesPlayed", 1 }, // Initialize totalGamesPlayed since it's a new document
                            { "timestamp", Timestamp.GetCurrentTimestamp() }
                        }).ContinueWithOnMainThread(setTask =>
                        {
                            if (setTask.IsCompleted)
                            {
                                Debug.Log("Weekly averages set successfully.");
                            }
                            else
                            {
                                Debug.LogError("Failed to set weekly averages: " + setTask.Exception);
                            }
                        });
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to fetch OverallWeek document: " + snapshotTask.Exception);
                    }
                });
            }
            else
            {
                Debug.LogError("Failed to retrieve weekly data: " + task.Exception);
            }
        });
    }



    private Dictionary<string, Dictionary<string, int>> ConvertToFirestoreFormat()
    {
        var questionPerformance = evaluationScript.GetAnalytics();
        var firebaseData = new Dictionary<string, Dictionary<string, int>>();

        foreach (var entry in questionPerformance)
        {
            firebaseData[entry.Key] = new Dictionary<string, int>
            {
                { "correct", entry.Value.Item1 },
                { "total", entry.Value.Item2 }
            };
        }

        return firebaseData;
    }

    private Dictionary<string, float> CalculateEvaluationPercentages()
    {
        var questionPerformance = evaluationScript.GetAnalytics();
        var evaluationPercentages = new Dictionary<string, float>();

        foreach (var entry in questionPerformance)
        {
            int correct = entry.Value.Item1;
            int total = entry.Value.Item2;
            float percentage = total > 0 ? (float)Math.Round((correct / (float)total) * 100, 2) : 0;
            evaluationPercentages[entry.Key] = percentage;
        }

        return evaluationPercentages;
    }

    private float CalculateOverallScore()
    {
        return (float)Math.Round((evaluationScript.GetFinalScores() / 50f) * 100f, 2);
    }
}