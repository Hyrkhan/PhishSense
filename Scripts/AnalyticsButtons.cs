using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnalyticsButtons : MonoBehaviour
{
    public GameObject weeklyLineGraph;
    public GameObject progressionGraph;


    public GameObject easymodeGraphs;
    public GameObject normalmodeGraphs;
    public GameObject hardmodeGraphs;

    public AnalyticsFetcher analyticsFetcher;
    public TMP_Dropdown gameModeDropdown;

    public GameObject analyticsScreen;
    public GameObject historyScreen;
    public gameHistory gameHistory;

    public void GoAnalytics()
    {
        analyticsScreen.SetActive(true);
        historyScreen.SetActive(false);
    }

    public void GoHistory()
    {
        analyticsScreen.SetActive(false);
        historyScreen.SetActive(true);
        gameHistory.getHistory();
    }

    public void WeeklyLinegraph()
    {
        weeklyLineGraph.SetActive(true);
        progressionGraph.SetActive(false);
    }
    public void ProgressionGraph()
    {
        weeklyLineGraph.SetActive(false);
        progressionGraph.SetActive(true);
    }

    public void DropdownOption(int index)
    {
        switch (index)
        {
            case 0:
                WeeklyLinegraph();
                break;
            case 1:
                ProgressionGraph();
                break;
        }
    }
    
    public void EasyMode()
    {
        easymodeGraphs.SetActive(true);
        normalmodeGraphs.SetActive(false);
        hardmodeGraphs.SetActive(false);
    }

    public void NormalMode()
    {
        normalmodeGraphs.SetActive(true);
        easymodeGraphs.SetActive(false);
        hardmodeGraphs.SetActive(false);
    }

    public void HardMode()
    {
        hardmodeGraphs.SetActive(true);
        easymodeGraphs.SetActive(false);
        normalmodeGraphs.SetActive(false);
    }

    public void GameModeDropdown()
    {
        int index = gameModeDropdown.value;
        switch (index)
        {
            case 0:
                analyticsFetcher.FetchAnalytics("Easy");
                Debug.Log("plotted easies");
                break;
            case 1:
                analyticsFetcher.FetchAnalytics("Normal");
                Debug.Log("plotted normals");
                break;
            case 2:
                analyticsFetcher.FetchAnalytics("Hard");
                break;
        }
    }
}
