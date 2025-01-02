using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class detectionScript : MonoBehaviour
{
    public gameObjectsScript gameObjectsScript;
    private string copyLink = "";

    private string actionType = "";
    private string websiteURL = "";
    private string fileType = "";

    private string searchingLink = "";
    private int currentEmailIndex;
    public scanResultScript scanResultScript;

    public void CopyLink()
    {
        copyLink = gameObjectsScript.emailLinkDisplay.text;
        //Debug.Log($"Link has been copied: {copyLink}");
    }
    public void PasteLink1()
    {
        gameObjectsScript.searchBox.text = copyLink;
    }
    public void PasteLink2()
    {
        gameObjectsScript.VMsearchBox.text = copyLink;
        //gameObjectsScript.Zoom_VMSearchBox.text = copyLink;
        searchingLink = copyLink;
    }
    public void Scan()
    {
        gameObjectsScript.ScanbaseScreen.SetActive( false );
        gameObjectsScript.ScanresultScreen.SetActive( true );
        //gameObjectsScript.maximizebutton.SetActive(true);
    }
    public void ScanAnother()
    {
        gameObjectsScript.ScanresultScreen.SetActive( false );
        gameObjectsScript.ScanbaseScreen.SetActive( true );
        //gameObjectsScript.maximizebutton.SetActive ( false );
    }
    public void OpenChrome()
    {
        gameObjectsScript.VMChromeScreen.SetActive(!gameObjectsScript.VMChromeScreen.activeSelf);
        //gameObjectsScript.Zoom_VMChromeScreen.SetActive(!gameObjectsScript.Zoom_VMChromeScreen.activeSelf);
    }
    public void OpenFiles()
    {
        gameObjectsScript.VMFilesScreen.SetActive(!gameObjectsScript.VMFilesScreen.activeSelf);
        //gameObjectsScript.Zoom_VMFilesScreen.SetActive(!gameObjectsScript.Zoom_VMFilesScreen.activeSelf);
    }
    public void BacktoChrome()
    {
        gameObjectsScript.phishingWarningPict.SetActive(false);
        gameObjectsScript.safeLegitWebsitePict.SetActive(false);
        //gameObjectsScript.zoom_phishingWarningPict.SetActive(false);
        //gameObjectsScript.zoom_safeLegitWebsitePict.SetActive(false);

        gameObjectsScript.VMWebsite.SetActive(false);
        //gameObjectsScript.Zoom_VMWebsite.SetActive(false);
        gameObjectsScript.VMChromeWebsite.SetActive(true);
        //gameObjectsScript.Zoom_VMChromeWebsite.SetActive(true);
    }
    

    public void SetVMParameters(string action, string fileTypes, string url)
    {
        actionType = action;
        fileType = fileTypes;
        websiteURL = url;
    }
     

    public void SearchLink()
    {
        var checkresult = scanResultScript.CheckTheLink(searchingLink);

        if (checkresult.Item1 == true)
        {
            currentEmailIndex = checkresult.Item2;
            scanResultScript.SetVMParams(currentEmailIndex);

            gameObjectsScript.VMChromeWebsite.SetActive(false);
            //gameObjectsScript.Zoom_VMChromeWebsite.SetActive(false);
            gameObjectsScript.VMWebsiteURL.text = websiteURL;
            //gameObjectsScript.Zoom_VMWebsiteURL.text = websiteURL;
            gameObjectsScript.VMWebsite.SetActive(true);
            //gameObjectsScript.Zoom_VMWebsite.SetActive(true);

            Invoke("DelayedCheckWebsite", 1f);
            CheckForLegitness();
        }
        else
        {
            Debug.Log("No match for the link.");
            return;
        }
    }

    private void DelayedCheckWebsite()
    {
        checkWebsite(fileType);
    }

    public void checkWebsite(string fileTypess)
    {
        if (fileTypess == "virus")
        {
            string key = PlayerPrefs.GetString("GameMode", "Unknown"); // Default to "Unknown" if key doesn't exist

            switch (key)
            {
                case "Easy":
                    gameObjectsScript.DownloadPopup.SetActive(true);
                    //gameObjectsScript.Zoom_DownloadPopup.SetActive(true);
                    Invoke("ClosePopup", 2f);
                    AddVirusToFileManager();
                    break;

                case "Normal":
                    gameObjectsScript.normalPopup.SetActive(true);
                    //gameObjectsScript.Zoom_normalPopup.SetActive(true);
                    Invoke("ClosePopup", 2f);
                    AddVirusToFileManager();
                    break;

                case "Hard":
                    AddVirusToFileManager();
                    break;

                default:
                    Debug.LogWarning("Unknown game mode: " + key);
                    break;
            }
        }
        else if (fileTypess == "safe")
        {
            // Handle "safe" case logic here (if needed)
        }
        else
        {
            gameObjectsScript.normalPopup.SetActive(true);
            //gameObjectsScript.Zoom_normalPopup.SetActive(true);
            Invoke("ClosePopup", 2f);
            AddFileToFileManager(fileTypess);
        }

    }

    public void CheckForLegitness()
    {
        if (actionType == "fakeWebsite")
        {
            gameObjectsScript.phishingWarningPict.SetActive(true);
            //gameObjectsScript.zoom_phishingWarningPict.SetActive(true);
        }
        else if (actionType == "safeWebsite")
        {
            gameObjectsScript.safeLegitWebsitePict.SetActive(true);
            //gameObjectsScript.zoom_safeLegitWebsitePict.SetActive(true);
        }
    }

    private void AddVirusToFileManager()
    {
        GameObject newVirusContainer = Instantiate(gameObjectsScript.virusContainerPrefab, gameObjectsScript.virusContainerParent);
        //GameObject newZoomVirusContainer = Instantiate(gameObjectsScript.Zoom_virusContainerPrefab, gameObjectsScript.Zoom_virusContainerParent);


        Debug.Log("Virus file added to file manager!");
    }

    private void AddFileToFileManager(string filename)
    {
        GameObject newFile = Instantiate(gameObjectsScript.virusContainerPrefab, gameObjectsScript.virusContainerParent);
        //GameObject newZoomFile = Instantiate(gameObjectsScript.Zoom_virusContainerPrefab, gameObjectsScript.Zoom_virusContainerParent);

        TMP_Text normfilename = newFile.transform.Find("virusExeFile").Find("filename").GetComponent<TMP_Text>();
        //TMP_Text zoom_normfilename = newZoomFile.transform.Find("virusExeFileZoom").Find("filename").GetComponent<TMP_Text>();

        normfilename.text = $"file.{filename}";
        //zoom_normfilename.text = $"file.{filename}";
    }

    private void ClosePopup()
    {
        gameObjectsScript.DownloadPopup.SetActive(false);
        //gameObjectsScript.Zoom_DownloadPopup.SetActive(false);
        gameObjectsScript.normalPopup.SetActive(false);
       //gameObjectsScript.Zoom_normalPopup.SetActive(false);
    }

}
