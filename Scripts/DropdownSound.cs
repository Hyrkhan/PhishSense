using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class DropdownSound : MonoBehaviour
{
    public List<TMP_Dropdown> dropdowns; // List of TMP_Dropdown references
    public AudioSource audioSource; // AudioSource to play the sound

    void Start()
    {
        // Ensure that we have dropdowns and the audio source assigned
        if (dropdowns == null || dropdowns.Count == 0 || audioSource == null)
        {
            Debug.LogError("Dropdown list or AudioSource is not assigned!");
            return;
        }

        // Loop through each dropdown in the list and add listeners
        foreach (var dropdown in dropdowns)
        {
            if (dropdown != null)
            {
                // Add listener for value changed
                dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

                // Add EventTrigger for PointerClick (to detect when dropdown is opened)
                EventTrigger trigger = dropdown.gameObject.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((eventData) => OnDropdownOpened());
                trigger.triggers.Add(entry);
            }
        }
    }

    // Play sound when any dropdown is opened
    private void OnDropdownOpened()
    {
        if (audioSource != null)
        {
            audioSource.Play(); // Play the sound when the dropdown is opened
        }
    }

    // Play sound when any dropdown value changes
    private void OnDropdownValueChanged(int value)
    {
        if (audioSource != null)
        {
            audioSource.Play(); // Play the sound when an option is selected
        }
    }
}
