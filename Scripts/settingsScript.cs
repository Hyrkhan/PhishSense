using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class settingsScript : MonoBehaviour
{
    public GameObject settingsContainer;

    public void GoSettings()
    {
        if (settingsContainer.activeSelf == false)
        {
            settingsContainer.SetActive(true);
        }
        else if (settingsContainer.activeSelf == true)
        {
            settingsContainer.SetActive(false);
        }
    }
}
