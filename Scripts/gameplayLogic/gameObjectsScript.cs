using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class gameObjectsScript : MonoBehaviour
{
    //GameMode Script
    [Space]
    [Header("GameMode Script")]
    public GameObject emailCont;
    public GameObject evalCont;
    public GameObject urlScanCont;
    public GameObject vmscreenCont;
    public TMP_Text gamemodetext;
    public GameObject gameWelcomeScreen;
    public GameObject gameScreen;
    public GameObject detectionScreen;
    public GameObject detectionGameScreen;
    public GameObject detectionScanScreen;
    public GameObject detectionVMScreen;
    public GameObject gameOverScreen;
    public GameObject emailReviewScreen;

    //emailNavScript
    [Space]
    [Header("emailNav Script")]
    public TMP_Text senderEmailDisplay; 
    public TMP_Text emailBodyDisplay;   
    public TMP_Text emailLinkDisplay;
    public TMP_Text emailNumber;

    //EvaluateForm Script
    [Space]
    [Header("EvaluateForm Script")]
    public GameObject evaluationDisplay;

    //hintsScript
    [Space]
    [Header("hints Script")]
    public GameObject hintDisplay;
    public TMP_Text hintText;

    //detectionScript
    [Space]
    [Header("detection Script")]
    public TMP_InputField searchBox;
    public GameObject ScanbaseScreen;
    public GameObject ScanresultScreen;

    public TMP_InputField VMsearchBox;
    //public TMP_InputField Zoom_VMSearchBox;
    public GameObject VMChromeScreen;
    //public GameObject Zoom_VMChromeScreen;
    public GameObject VMFilesScreen;
    //public GameObject Zoom_VMFilesScreen;

    public GameObject VMChromeWebsite;
    //public GameObject Zoom_VMChromeWebsite;
    public GameObject VMWebsite;
    //public GameObject Zoom_VMWebsite;
    public TMP_Text VMWebsiteURL;
    //public TMP_Text Zoom_VMWebsiteURL;
    public GameObject DownloadPopup;
    //public GameObject Zoom_DownloadPopup;
    public GameObject normalPopup;
    //public GameObject Zoom_normalPopup;
   

    [SerializeField] public GameObject virusContainerPrefab;
    [SerializeField] public Transform virusContainerParent;
    //[SerializeField] public GameObject Zoom_virusContainerPrefab;
    //[SerializeField] public Transform Zoom_virusContainerParent;

    public GameObject maximizebutton;
    public GameObject minimizebutton;
    public GameObject backToEmailsButton;

    public GameObject phishingWarningPict;
    public GameObject safeLegitWebsitePict;
    //public GameObject zoom_phishingWarningPict;
    //public GameObject zoom_safeLegitWebsitePict;

    //otherButtons Script    
    [Space]
    [Header("otherButtons Script")]
    public GameObject emailScreen;        

    public TMP_Text fullscreenSenderEmailText;
    public TMP_Text fullscreenEmailBodyText;
    public TMP_Text fullscreenEmailLinkText;

    //public GameObject zoom_resultScreen;
    public GameObject resultScreen;
    public GameObject detectionButtons;
    //public GameObject Zoom_VMScreen;

    //emailReview
    [Space]
    [Header("emailReview Script")]
    public GameObject reviewBaseScreen;
    public GameObject reviewScreen;
    public TMP_Text reviewUserAnswer;
    public TMP_Text reviewCheckAnswer;
    public TMP_Text pageNumber;
    public TMP_Text questionLabel;
    public TMP_Text correctAnswerExplanation;

    //emailReview CheckDisplay
    [Space]
    [Header("emailReview CheckDisplay")]
    public TMP_Text ReviewEmailBodyText;
    public TMP_Text ReviewSenderEmail;
    public TMP_Text ReviewRiskLevel;
    public TMP_Text ReviewViolations;
    public TMP_Text ReviewCertIssue;
    public TMP_Text ReviewCertExpiry;
    public GameObject ReviewSafeMark;
    public GameObject ReviewPhishingMark;


    [Space]
    [Header("Panels")]
    public GameObject coverPanel;
    public GameObject loadingbuffer;
    public GameObject windowsmenu;
    public GameObject warningPopup;
}
