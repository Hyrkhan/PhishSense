using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Firebase.Firestore;
using System.Collections;
using UnityEngine.SceneManagement;


public class EmailFetcher : MonoBehaviour
{
    [System.Serializable]
    public class Email
    {
        public string senderEmail;    // Sender's email address
        public string emailTextBody;  // The text of the email
        public string emailLink;      // The link inside the email
        public string hint;

        // URL Scanner fields
        public string domainAge;
        public int redirectsFound;

        // SSL Certificates Info
        public string certSubject;
        public string certIssueDate;
        public string certExpiryDate;

        // Security Headers
        public string contentSecurityPolicy;
        public string strictTransportSecurity;
        public string xFrameOptions;

        public string grammarError;
        public string suspiciousSender;
        public string markAnswer;

        public string actionType;
        public string fileType;
        public string websiteURL;
    }

    public List<Email> emails = new List<Email>();  // List to store fetched emails
    public List<Email> phishingEmails = new List<Email>();
    public List<Email> safeEmails = new List<Email>();
    private FirebaseFirestore db;

    // Event to notify when emails are fetched
    public delegate void EmailsFetched();
    public event EmailsFetched OnEmailsFetched;
    public string gameMode = "";
    public gameObjectsScript gameObjectsScript;
    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;  // Initialize Firestore

        gameMode = PlayerPrefs.GetString("GameMode", "Easy");
        StartCoroutine(FetchEasyEmailsFromFirestore());
    }

    // Provide access to the fetched emails
    public List<Email> GetEmails()
    {
        return emails;
    }

    private IEnumerator FetchEasyEmailsFromFirestore()
    {
        int totalEmailsToFetch = 5;
        int phish = 0;
        int safes = 0;

        gameObjectsScript.loadingbuffer.SetActive(true);
        Debug.Log("Fetching phishing emails...");
        yield return StartCoroutine(FetchPhishingEmailsCoroutine(gameMode)); // Wait for phishing emails to be fetched

        Debug.Log("Fetching safe emails...");
        yield return StartCoroutine(FetchSafeEmailsCoroutine(gameMode)); // Wait for safe emails to be fetched


        Debug.Log("Both phishing and safe emails have been fetched. Starting to add them...");

        // Once emails are fetched, proceed to the next step
        for (int i = 0; i < totalEmailsToFetch; i++)
        {
            int typeOfEmail = UnityEngine.Random.Range(1, 3); // 1 for phishing, 2 for safe
            if (typeOfEmail == 1 && phishingEmails.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, phishingEmails.Count);
                emails.Add(phishingEmails[index]);
                phishingEmails.RemoveAt(index); // Remove the selected email to prevent duplicates
                phish++;
            }
            else if (typeOfEmail == 2 && safeEmails.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, safeEmails.Count);
                emails.Add(safeEmails[index]);
                safeEmails.RemoveAt(index); // Remove the selected email to prevent duplicates
                safes++;
            }
        }

        gameObjectsScript.loadingbuffer.SetActive(false);
        Debug.Log($"Total emails added: {emails.Count} (Phishing: {phish}, Safe: {safes})");

        // Notify listeners that emails have been fetched
        OnEmailsFetched?.Invoke();
    }

    private IEnumerator FetchPhishingEmailsCoroutine(string gameMode)
    {
        bool isCompleted = false;

        db.Collection("AllEmails").Document($"{gameMode}Emails").Collection("PhishingEmails")
          .GetSnapshotAsync()
          .ContinueWith(task =>
          {
              if (task.IsCompleted)
              {
                  QuerySnapshot snapshot = task.Result;
                  foreach (DocumentSnapshot document in snapshot.Documents)
                  {
                      Dictionary<string, object> emailData = document.ToDictionary();
                      phishingEmails.Add(ParseEmail(emailData));
                  }
                  Debug.Log($"Total of {phishingEmails.Count} phishing emails fetched.");
              }
              else
              {
                  Debug.LogError("Failed to fetch phishing emails.");
              }
              isCompleted = true; // Mark as completed
          });

        while (!isCompleted) // Wait until fetching is completed
        {
            yield return null;
        }
    }

    private IEnumerator FetchSafeEmailsCoroutine(string gameMode)
    {
        bool isCompleted = false;

        db.Collection("AllEmails").Document($"{gameMode}Emails").Collection("SafeEmails")
          .GetSnapshotAsync()
          .ContinueWith(task =>
          {
              if (task.IsCompleted)
              {
                  QuerySnapshot snapshot = task.Result;
                  foreach (DocumentSnapshot document in snapshot.Documents)
                  {
                      Dictionary<string, object> emailData = document.ToDictionary();
                      safeEmails.Add(ParseEmail(emailData));
                  }
                  Debug.Log($"Total of {safeEmails.Count} safe emails fetched.");
              }
              else
              {
                  Debug.LogError("Failed to fetch safe emails.");
              }
              isCompleted = true; // Mark as completed
          });

        while (!isCompleted) // Wait until fetching is completed
        {
            yield return null;
        }
    }

    private Email ParseEmail(Dictionary<string, object> emailData)
    {
        Email email = new Email
        {
            senderEmail = emailData["senderEmail"].ToString(),
            emailTextBody = emailData["emailTextBody"].ToString(),
            emailLink = emailData["emailLink"].ToString(),
            hint = emailData["hint"].ToString()
        };

        // Handle nested fields
        if (emailData.ContainsKey("urlScanner"))
        {
            Dictionary<string, object> urlScanner = emailData["urlScanner"] as Dictionary<string, object>;
            email.domainAge = urlScanner["domainAge"].ToString();
            email.redirectsFound = System.Convert.ToInt32(urlScanner["redirectsFound"].ToString());

            if (urlScanner.ContainsKey("certificates"))
            {
                Dictionary<string, object> certificates = urlScanner["certificates"] as Dictionary<string, object>;
                email.certSubject = certificates["subject"].ToString();
                email.certIssueDate = certificates["issueDate"].ToString();
                email.certExpiryDate = certificates["expiryDate"].ToString();
            }

            if (urlScanner.ContainsKey("securityHeaders"))
            {
                Dictionary<string, object> headers = urlScanner["securityHeaders"] as Dictionary<string, object>;
                email.contentSecurityPolicy = headers["Content-Security-Policy"].ToString();
                email.strictTransportSecurity = headers["Strict-Transport-Security"].ToString();
                email.xFrameOptions = headers["X-Frame-Options"].ToString();
            }
        }

        if (emailData.ContainsKey("evaluationAnswers"))
        {
            Dictionary<string, object> evaluationAnswers = emailData["evaluationAnswers"] as Dictionary<string, object>;
            email.grammarError = evaluationAnswers["grammarError"].ToString();
            email.suspiciousSender = evaluationAnswers["suspiciousSender"].ToString();
            email.markAnswer = evaluationAnswers["markAnswer"].ToString();
        }

        if (emailData.ContainsKey("vmAction"))
        {
            Dictionary<string, object> vmAction = emailData["vmAction"] as Dictionary<string, object>;
            email.actionType = vmAction["actionType"].ToString();
            email.fileType = vmAction["fileType"].ToString();
            email.websiteURL = vmAction["websiteURL"].ToString();
        }

        return email;
    }


}


//public void FetchEasyEmails()
//{
//    db.Collection("AllEmails").Document("EasyEmails").Collection("PhishingEmails").GetSnapshotAsync().ContinueWith(task =>
//    {
//        if (task.IsCompleted)
//        {
//            QuerySnapshot snapshot = task.Result;
//            Debug.Log("Fetching emails from EasyEmails -> PhishingEmails...");

//            foreach (DocumentSnapshot document in snapshot.Documents)
//            {
//                Dictionary<string, object> emailData = document.ToDictionary();

//                Email email = new Email
//                {
//                    senderEmail = emailData["senderEmail"].ToString(),
//                    emailTextBody = emailData["emailTextBody"].ToString(),
//                    emailLink = emailData["emailLink"].ToString(),
//                    hint = emailData["hint"].ToString()
//                };

//                // Handle urlScanner fields, which are nested
//                if (emailData.ContainsKey("urlScanner"))
//                {
//                    Dictionary<string, object> urlScanner = emailData["urlScanner"] as Dictionary<string, object>;

//                    email.domainAge = urlScanner["domainAge"].ToString();
//                    email.redirectsFound = System.Convert.ToInt32(urlScanner["redirectsFound"].ToString());

//                    if (urlScanner.ContainsKey("certificates"))
//                    {
//                        Dictionary<string, object> certificates = urlScanner["certificates"] as Dictionary<string, object>;

//                        email.certSubject = certificates["subject"].ToString();
//                        email.certIssueDate = certificates["issueDate"].ToString();
//                        email.certExpiryDate = certificates["expiryDate"].ToString();
//                    }

//                    if (urlScanner.ContainsKey("securityHeaders"))
//                    {
//                        Dictionary<string, object> headers = urlScanner["securityHeaders"] as Dictionary<string, object>;

//                        email.contentSecurityPolicy = headers["Content-Security-Policy"].ToString();
//                        email.strictTransportSecurity = headers["Strict-Transport-Security"].ToString();
//                        email.xFrameOptions = headers["X-Frame-Options"].ToString();
//                    }
//                }

//                if (emailData.ContainsKey("evaluationAnswers"))
//                {
//                    Dictionary<string, object> evaluationAnswers = emailData["evaluationAnswers"] as Dictionary<string, object>;

//                    email.grammarError = evaluationAnswers["grammarError"].ToString();
//                    email.suspiciousSender = evaluationAnswers["suspiciousSender"].ToString();
//                    email.markAnswer = evaluationAnswers["markAnswer"].ToString();
//                }

//                if (emailData.ContainsKey("vmAction"))
//                {
//                    Dictionary<string, object> vmAction = emailData["vmAction"] as Dictionary<string, object>;

//                    email.actionType = vmAction["actionType"].ToString();
//                    email.fileType = vmAction["fileType"].ToString();
//                    email.websiteURL = vmAction["websiteURL"].ToString();
//                }

//                emails.Add(email);  // Add each email to the list
//            }

//            Debug.Log($"Fetched {emails.Count} emails from EasyEmails -> PhishingEmails");

//            // Trigger event to notify that emails have been fetched
//            OnEmailsFetched?.Invoke();
//        }
//        else
//        {
//            Debug.LogError("Failed to fetch emails from Firestore");
//        }
//    });
//}