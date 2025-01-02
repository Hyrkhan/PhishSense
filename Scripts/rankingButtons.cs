using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class rankingButtons : MonoBehaviour
{
    public GameObject weeklyscoreboard;
    public GameObject weeklygrammarboard;
    public GameObject weeklysenderboard;
    public GameObject weeklyriskboard;
    public GameObject weeklyviolationsboard;
    public GameObject weeklycertboard;
    public GameObject weeklyemailmarkboard;

    public TMP_Dropdown rankingDropdown;

    public GameObject dailyscoreboard;
    public GameObject dailygrammarboard;
    public GameObject dailysenderboard;
    public GameObject dailyriskboard;
    public GameObject dailyviolationsboard;
    public GameObject dailycertboard;
    public GameObject dailyemailmarkboard;

    public TMP_Dropdown gamemodes;

    public rankingScript rankingScript;
    public DailyLeaderboard dailyleaderboard;

    [Space]
    public List<Scrollbar> Overallscrollbars;
    public List<Scrollbar> Dailyscrollbars;

    void Start()
    {
        // Ensure the dropdown's value change is hooked to the RankingDropdown method
        rankingDropdown.onValueChanged.AddListener(delegate { RankingDropdown(); });
    }

    public void ResetScrollbars(int index)
    {
        // Check if the index is valid for the Overallscrollbars list
        if (index >= 0 && index < Overallscrollbars.Count)
        {
            Scrollbar scrollbar = Overallscrollbars[index];
            if (scrollbar != null)
            {
                scrollbar.value = 1f; // Reset to the top/left
                Debug.Log("Scroll bar at index " + index + " reset in Overallscrollbars");
            }
        }

        // Check if the index is valid for the Dailyscrollbars list
        if (index >= 0 && index < Dailyscrollbars.Count)
        {
            Scrollbar scrollbar2 = Dailyscrollbars[index];
            if (scrollbar2 != null)
            {
                scrollbar2.value = 1f; // Reset to the top/left
                Debug.Log("Scroll bar at index " + index + " reset in Dailyscrollbars");
            }
        }
    }


    public void RankingGameModes()
    {
        int index = gamemodes.value;

        switch (index)
        {
            case 0:
                
                rankingScript.DisplayGameModeLeaderboard("Easy");
                dailyleaderboard.DisplayDailyboard("Easy");
                break;
            case 1:
                rankingScript.DisplayGameModeLeaderboard("Normal");
                dailyleaderboard.DisplayDailyboard("Normal");
                break;
            case 2:
                rankingScript.DisplayGameModeLeaderboard("Hard");
                dailyleaderboard.DisplayDailyboard("Hard");
                break;
        }
    }
    private IEnumerator ResetScrollbarAfterDelay(int index)
    {
        // Wait for one frame to make sure everything is fully active
        yield return null;

        // Now reset the scrollbar
        ResetScrollbars(index);
    }

    public void RankingDropdown()
    {   
        int index = rankingDropdown.value;

        switch (index)
        {
            case 0:
                ShowWeeklyScore();
                break;
            case 1:
                ShowWeeklyGrammarCheck();
                break;
            case 2:
                ShowWeeklySuspiciousSender();
                break;
            case 3:
                ShowWeeklyURLRisk();
                break;
            case 4:
                ShowWeeklyViolations();
                break;
            case 5:
                ShowWeeklyCertificateStatus();
                break;
            case 6:
                ShowWeeklyEmailMark();
                break;
        }
    }

    public void ShowWeeklyScore()
    {
        weeklyscoreboard.SetActive(true);
        weeklygrammarboard.SetActive(false);
        weeklysenderboard.SetActive(false);
        weeklyriskboard.SetActive(false);
        weeklyviolationsboard.SetActive(false);
        weeklycertboard.SetActive(false);
        weeklyemailmarkboard.SetActive(false);

        dailyscoreboard.SetActive(true);
        dailygrammarboard.SetActive(false);
        dailysenderboard.SetActive(false);
        dailyriskboard.SetActive(false);
        dailyviolationsboard.SetActive(false);
        dailycertboard.SetActive(false);
        dailyemailmarkboard.SetActive(false);
        StartCoroutine(ResetScrollbarAfterDelay(0));
    }

    public void ShowWeeklyGrammarCheck()
    {
        weeklyscoreboard.SetActive(false);
        weeklygrammarboard.SetActive(true);
        weeklysenderboard.SetActive(false);
        weeklyriskboard.SetActive(false);
        weeklyviolationsboard.SetActive(false);
        weeklycertboard.SetActive(false);
        weeklyemailmarkboard.SetActive(false);

        dailyscoreboard.SetActive(false);
        dailygrammarboard.SetActive(true);
        dailysenderboard.SetActive(false);
        dailyriskboard.SetActive(false);
        dailyviolationsboard.SetActive(false);
        dailycertboard.SetActive(false);
        dailyemailmarkboard.SetActive(false);
        StartCoroutine(ResetScrollbarAfterDelay(1));
    }

    public void ShowWeeklySuspiciousSender()
    {
        weeklyscoreboard.SetActive(false);
        weeklygrammarboard.SetActive(false);
        weeklysenderboard.SetActive(true);
        weeklyriskboard.SetActive(false);
        weeklyviolationsboard.SetActive(false);
        weeklycertboard.SetActive(false);
        weeklyemailmarkboard.SetActive(false);

        dailyscoreboard.SetActive(false);
        dailygrammarboard.SetActive(false);
        dailysenderboard.SetActive(true);
        dailyriskboard.SetActive(false);
        dailyviolationsboard.SetActive(false);
        dailycertboard.SetActive(false);
        dailyemailmarkboard.SetActive(false);
        StartCoroutine(ResetScrollbarAfterDelay(2));
    }

    public void ShowWeeklyURLRisk()
    {
        weeklyscoreboard.SetActive(false);
        weeklygrammarboard.SetActive(false);
        weeklysenderboard.SetActive(false);
        weeklyriskboard.SetActive(true);
        weeklyviolationsboard.SetActive(false);
        weeklycertboard.SetActive(false);
        weeklyemailmarkboard.SetActive(false);

        dailyscoreboard.SetActive(false);
        dailygrammarboard.SetActive(false);
        dailysenderboard.SetActive(false);
        dailyriskboard.SetActive(true);
        dailyviolationsboard.SetActive(false);
        dailycertboard.SetActive(false);
        dailyemailmarkboard.SetActive(false);
        StartCoroutine(ResetScrollbarAfterDelay(3));
    }

    public void ShowWeeklyViolations()
    {
        weeklyscoreboard.SetActive(false);
        weeklygrammarboard.SetActive(false);
        weeklysenderboard.SetActive(false);
        weeklyriskboard.SetActive(false);
        weeklyviolationsboard.SetActive(true);
        weeklycertboard.SetActive(false);
        weeklyemailmarkboard.SetActive(false);

        dailyscoreboard.SetActive(false);
        dailygrammarboard.SetActive(false);
        dailysenderboard.SetActive(false);
        dailyriskboard.SetActive(false);
        dailyviolationsboard.SetActive(true);
        dailycertboard.SetActive(false);
        dailyemailmarkboard.SetActive(false);
        StartCoroutine(ResetScrollbarAfterDelay(4));
    }

    public void ShowWeeklyCertificateStatus()
    {
        weeklyscoreboard.SetActive(false);
        weeklygrammarboard.SetActive(false);
        weeklysenderboard.SetActive(false);
        weeklyriskboard.SetActive(false);
        weeklyviolationsboard.SetActive(false);
        weeklycertboard.SetActive(true);
        weeklyemailmarkboard.SetActive(false);

        dailyscoreboard.SetActive(false);
        dailygrammarboard.SetActive(false);
        dailysenderboard.SetActive(false);
        dailyriskboard.SetActive(false);
        dailyviolationsboard.SetActive(false);
        dailycertboard.SetActive(true);
        dailyemailmarkboard.SetActive(false);
        StartCoroutine(ResetScrollbarAfterDelay(5));
    }

    public void ShowWeeklyEmailMark()
    {
        weeklyscoreboard.SetActive(false);
        weeklygrammarboard.SetActive(false);
        weeklysenderboard.SetActive(false);
        weeklyriskboard.SetActive(false);
        weeklyviolationsboard.SetActive(false);
        weeklycertboard.SetActive(false);
        weeklyemailmarkboard.SetActive(true);

        dailyscoreboard.SetActive(false);
        dailygrammarboard.SetActive(false);
        dailysenderboard.SetActive(false);
        dailyriskboard.SetActive(false);
        dailyviolationsboard.SetActive(false);
        dailycertboard.SetActive(false);
        dailyemailmarkboard.SetActive(true);
        StartCoroutine(ResetScrollbarAfterDelay(6));
    }

}
