using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnalyticsFetcher;

public class gridPlotting : MonoBehaviour
{
    public RectTransform gridArea; // Assign the grid area (e.g., a UI Panel)
    public GameObject pointPrefab; // Prefab for the points
    public RectTransform lineParent; // Parent object to hold the line points
    public LineRenderer lineRenderer; // LineRenderer to connect points

    // Fixed data for testing
    private float[] scores = new float[7];
    private float yMax = 100f; // Max Y-axis value

    public GameObject[] YAxisReferences;


    public void startPlot()
    {
        scores[0] = AnalyticsStorage.DayScores["MondayData"];
        scores[1] = AnalyticsStorage.DayScores["TuesdayData"];
        scores[2] = AnalyticsStorage.DayScores["WednesdayData"];
        scores[3] = AnalyticsStorage.DayScores["ThursdayData"];
        scores[4] = AnalyticsStorage.DayScores["FridayData"];
        scores[5] = AnalyticsStorage.DayScores["SaturdayData"];
        scores[6] = AnalyticsStorage.DayScores["SundayData"];
        PlotPoints();
        Debug.Log("plotted week");
    }

    void PlotPoints()
    {
        ClearPointsAndLines();

        if (YAxisReferences.Length != scores.Length)
        {
            Debug.LogError("The number of X-axis reference objects must match the number of scores.");
            return;
        }

        // Create an array to hold point positions for LineRenderer
        Vector3[] linePoints = new Vector3[scores.Length];

        // Loop through the data
        for (int i = 0; i < scores.Length; i++)
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
            float yPos = (scores[i] / yMax) * gridArea.rect.height - (gridArea.rect.height / 2f);

            // Combine X position with calculated Y position
            Vector2 anchoredPos = new Vector2(xPos, yPos);

            // Instantiate the point
            GameObject point = Instantiate(pointPrefab, lineParent);
            point.GetComponent<RectTransform>().anchoredPosition = anchoredPos;

            // Save the world position for the line
            linePoints[i] = point.GetComponent<RectTransform>().position;
        }

        // Set positions in LineRenderer
        lineRenderer.positionCount = linePoints.Length;
        lineRenderer.SetPositions(linePoints);
    }

    public void ClearPointsAndLines()
    {
        // Destroy all child objects of the lineParent
        foreach (Transform child in lineParent)
        {
            Destroy(child.gameObject);
        }

        // Reset the LineRenderer
        lineRenderer.positionCount = 0;
    }

    public List<string> GetLowScoreDays(float threshold)
    {
        // Map the scores to their respective days
        string[] daysOfWeek = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        List<string> lowScoreDays = new List<string>();

        for (int i = 0; i < scores.Length; i++)
        {
            if (scores[i] < threshold && scores[i] != 0)
            {
                lowScoreDays.Add(daysOfWeek[i]);
            }
        }

        return lowScoreDays;
    }

    public void ShowLowScoreSummary()
    {
        float threshold = 50f; // Define the low score threshold
        List<string> lowScoreDays = GetLowScoreDays(threshold);

        if (lowScoreDays.Count > 0)
        {
            Debug.Log("Low scores detected on: " + string.Join(", ", lowScoreDays));
        }
        else
        {
            Debug.Log("No low scores this week!");
        }
    }

}


