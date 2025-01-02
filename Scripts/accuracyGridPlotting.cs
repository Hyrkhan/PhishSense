using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnalyticsFetcher;

public class accuracyGridPlotting : MonoBehaviour
{
    public RectTransform gridArea; // Assign the grid area (e.g., a UI Panel)
    public GameObject pointPrefab; // Prefab for the points
    public RectTransform lineParent; // Parent object to hold the line points
    public LineRenderer[] lineRenderers; // LineRenderers for each evaluation line (6 in total)

    private float[][] evaluationScores = new float[6][]; // 6 evaluations

    private float yMax = 100f; // Max Y-axis value
    private int xMax = 7; // Days in a week

    public GameObject[] YAxisReferences; // Reference points for Y-axis positions

    public void startPlotting()
    {
        PopulateEvaluationScores();
    }

    private void PopulateEvaluationScores()
    {
        // Initialize arrays for each evaluation
        for (int i = 0; i < 6; i++)
        {
            evaluationScores[i] = new float[7]; // 7 days for each evaluation
        }

        // Days in order from Monday to Sunday
        string[] days = { "MondayData", "TuesdayData", "WednesdayData", "ThursdayData", "FridayData", "SaturdayData", "SundayData" };

        // Populate the scores
        for (int dayIndex = 0; dayIndex < days.Length; dayIndex++)
        {
            string day = days[dayIndex];
            if (AnalyticsStorage.DailyEvaluationScores.TryGetValue(day, out float[] dailyScores))
            {
                for (int evalIndex = 0; evalIndex < 6; evalIndex++)
                {
                    evaluationScores[evalIndex][dayIndex] = dailyScores[evalIndex];
                    //Debug.Log($"eval scores {dailyScores[evalIndex]}");
                }
            }
            else
            {
                Debug.LogWarning($"No data found for {day}. Defaulting to 0.");
                for (int evalIndex = 0; evalIndex < 6; evalIndex++)
                {
                    evaluationScores[evalIndex][dayIndex] = 0; // Default to 0 if no data
                }
            }
        }

        Debug.Log("Evaluation scores populated successfully.");
        PlotEvaluations();
    }

    void PlotEvaluations()
    {
        ClearPointsAndLines();

        if (YAxisReferences.Length != xMax)
        {
            Debug.LogError("The number of X-axis reference objects must match the number of days in a week.");
            return;
        }

        if (lineRenderers.Length != evaluationScores.Length)
        {
            Debug.LogError("The number of LineRenderers must match the number of evaluation datasets.");
            return;
        }

        // Loop through each evaluation dataset
        for (int evalIndex = 0; evalIndex < evaluationScores.Length; evalIndex++)
        {
            // Create an array to hold point positions for the current LineRenderer
            Vector3[] linePoints = new Vector3[xMax];

            // Loop through each day's score
            for (int i = 0; i < xMax; i++)
            {
                // Convert reference GameObject's position to local space of the gridArea
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    gridArea,
                    RectTransformUtility.WorldToScreenPoint(null, YAxisReferences[i].transform.position),
                    null,
                    out localPoint);

                // Extract the X position from the converted point
                float xPos = localPoint.x;

                // Calculate Y position based on the score and yMax
                float yPos = (evaluationScores[evalIndex][i] / yMax) * gridArea.rect.height - (gridArea.rect.height / 2f);

                // Combine X position with calculated Y position
                Vector2 anchoredPos = new Vector2(xPos, yPos);

                // Instantiate the point
                GameObject point = Instantiate(pointPrefab, lineParent);
                point.GetComponent<RectTransform>().anchoredPosition = anchoredPos;

                // Save the world position for the line
                linePoints[i] = point.GetComponent<RectTransform>().position;
            }

            // Set positions in the current LineRenderer
            lineRenderers[evalIndex].positionCount = linePoints.Length;
            lineRenderers[evalIndex].SetPositions(linePoints);
        }
    }

    public void ClearPointsAndLines()
    {
        // Destroy all child objects of the lineParent
        foreach (Transform child in lineParent)
        {
            Destroy(child.gameObject);
        }

        // Reset all LineRenderers
        foreach (LineRenderer lineRenderer in lineRenderers)
        {
            lineRenderer.positionCount = 0;
        }
    }

    public List<string> GetLowScoreSummary(float threshold)
    {
        // Days in order from Monday to Sunday
        string[] daysOfWeek = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        // Parameters to check
        string[] parameters = { "Certificate Checking", "Email Marking", "Grammar Checking", "Sender Checking", "URL Risk Evaluation", "Violations Checking" };

        List<string> summary = new List<string>();

        // Loop through each day
        for (int dayIndex = 0; dayIndex < daysOfWeek.Length; dayIndex++)
        {
            string dayKey = $"{daysOfWeek[dayIndex]}Data"; // Key format for the day

            // Retrieve scores for the day
            if (AnalyticsStorage.DailyEvaluationScores.TryGetValue(dayKey, out float[] dailyScores))
            {
                List<string> lowParameters = new List<string>();

                // Check each parameter's score
                for (int paramIndex = 0; paramIndex < parameters.Length; paramIndex++)
                {
                    // Ignore scores that are 0, and check if the score is below the threshold
                    if (paramIndex < dailyScores.Length && dailyScores[paramIndex] != 0 && dailyScores[paramIndex] < threshold)
                    {
                        lowParameters.Add(parameters[paramIndex]);
                    }
                }

                // If there are any low parameters, create a summary for the day
                if (lowParameters.Count > 0)
                {
                    string parameterList = string.Join(" and ", lowParameters);
                    summary.Add($"It looks like you have low accuracy in {parameterList} on {daysOfWeek[dayIndex]}. You better study those.");
                }
                else
                {
                    Debug.Log("No low accuracy in these days");
                }
            }
            else
            {
                Debug.LogWarning($"No data found for {dayKey}.");
            }
        }

        return summary;
    }

    public void ShowLowScoreSummary()
    {
        float threshold = 50f; // Define the low score threshold
        List<string> summary = GetLowScoreSummary(threshold);

        foreach (string entry in summary)
        {
            Debug.Log(entry);
        }
    }

}
