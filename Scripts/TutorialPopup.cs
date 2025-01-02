using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
    public Toggle disableNotificationToggle; // Reference to the checkbox
    public GameObject popupPanel; // The popup panel

    private const string DisableTutorialKey = "DisableTutorial";
    public MenuButtons buttons;
    public GameObject Panel;

    void Start()
    {

        // Check if the tutorial notification should be shown
        if (PlayerPrefs.GetInt(DisableTutorialKey, 0) == 1)
        {
            CloseNotice();
        }
        else
        {
            popupPanel.SetActive(true);
            Panel.SetActive(true);
        }

        // Set the toggle state based on saved preferences
        disableNotificationToggle.isOn = PlayerPrefs.GetInt(DisableTutorialKey, 0) == 1;

        // Add a listener to save the toggle state when it changes
        disableNotificationToggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void OnToggleValueChanged(bool isChecked)
    {
        // Save the toggle state to PlayerPrefs
        PlayerPrefs.SetInt(DisableTutorialKey, isChecked ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void StartTutorial()
    {
        Debug.Log("Starting the tutorial...");
        buttons.TutorialModeScene();
        CloseNotice();
    }

    public void SkipTutorial()
    {
        Debug.Log("Skipping the tutorial...");
        CloseNotice();
    }
    public void CloseNotice()
    {
        popupPanel.SetActive(false);
        Panel.SetActive(false);
    }
}
