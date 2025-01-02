using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;
using System;
using System.ComponentModel;
using static EmailFetcher;
using System.Text.RegularExpressions;

public class scanResultScript : MonoBehaviour
{

    public TMP_InputField linkBeingScanned;
    public TMP_Text resultLink;
    public TMP_Text resultVerdict;
    public TMP_Text resultViolations;

    // url scanner fields
    public TMP_Text domainAge;
    public TMP_Text redirectsFound;
    // ssl certificates info
    public TMP_Text certSubject;
    public TMP_Text certIssue;
    public TMP_Text certExpiry;
    // security headers info
    public TMP_Text contentSecurityPolicy;
    public TMP_Text strictTransportSecurity;
    public TMP_Text xFrameOptions;

    //public TMP_Text zoom_resultLink;
    //public TMP_Text zoom_resultVerdict;
    //public TMP_Text zoom_resultViolations;

    // url scanner fields
    //public TMP_Text zoom_domainAge;
    //public TMP_Text zoom_redirectsFound;
    // ssl certificates info
    //public TMP_Text zoom_certSubject;
    //public TMP_Text zoom_certIssue;
    //public TMP_Text zoom_certExpiry;
    // security headers info
    //public TMP_Text zoom_contentSecurityPolicy;
    //public TMP_Text zoom_strictTransportSecurity;
    //public TMP_Text zoom_xFrameOptions;

    private DateTime todayDate = DateTime.Today;

    private int currentEmailIndex;
    private List<EmailFetcher.Email> emails;
    public detectionScript detectionScript;


    public void SetEmails(List<EmailFetcher.Email> fetchedEmails)
    {
        emails = fetchedEmails;
    }

    /*
    public void SetPlaceholderParams()
    {
        var email = emails[currentEmailIndex];

        placeHolder_domainAge = email.domainAge;
        placeHolder_redirectsFound = email.redirectsFound;
        placeHolder_certSubject = email.certSubject;
        placeHolder_certIssue = email.certIssueDate;
        placeHolder_certExpiry = email.certExpiryDate;
        state_contentSecurityPolicy = email.contentSecurityPolicy;
        state_strictTransportSecurity = email.strictTransportSecurity;
        state_xFrameOptions = email.xFrameOptions;
        grammarError = email.grammarError;
        suspiciousSender = email.suspiciousSender;
        markAns = email.markAnswer;
    }*/

    public (bool, int) CheckTheLink(string linkbeingscanned)
    {
        string trimmedInputLink = linkbeingscanned.Trim();
        string sanitizedInputLink = Regex.Replace(trimmedInputLink, @"\p{C}+", "");
        int index = 0;
        foreach (var email in emails)
        {
            if (email.emailLink == sanitizedInputLink)
            {
                return (true, index);
            }
            else
            {
                index++;
            }
        }
        Debug.Log($"Input doesnt match email link");
        return (false, index);
    }


    public void DisplayResult()
    {
        var checkresult = CheckTheLink(linkBeingScanned.text);

        if (checkresult.Item1 == true)
        {
            currentEmailIndex = checkresult.Item2;
        }
        else
        {
            return;
        }

        var email = emails[currentEmailIndex];

        resultLink.text = linkBeingScanned.text;
        domainAge.text = email.domainAge;
        redirectsFound.text = $"{email.redirectsFound}";

        certSubject.text = email.certSubject;
        certIssue.text = email.certIssueDate;
        certExpiry.text = email.certExpiryDate;

        contentSecurityPolicy.text = email.contentSecurityPolicy;
        strictTransportSecurity.text = email.strictTransportSecurity;
        xFrameOptions.text = email.xFrameOptions;
        resultVerdict.text = CalculateSecurityRisk(currentEmailIndex);
        resultViolations.text = $"{CalculateSecurityViolations(currentEmailIndex)}";

        //zoom_resultLink.text = resultLink.text;
        //zoom_domainAge.text = domainAge.text;
        //zoom_redirectsFound.text = redirectsFound.text;

        //zoom_certSubject.text = certSubject.text;
        //zoom_certIssue.text = certIssue.text;
        //zoom_certExpiry.text = certExpiry.text;

        //zoom_contentSecurityPolicy.text = contentSecurityPolicy.text;
        //zoom_strictTransportSecurity.text = strictTransportSecurity.text;
        //zoom_xFrameOptions.text = xFrameOptions.text;
        //zoom_resultVerdict.text = resultVerdict.text;
        //zoom_resultViolations.text = resultViolations.text;

        detectionScript.Scan();
    }

    public string CalculateSecurityRisk(int index)
    {
        int risklevel = (CalculateSecurityViolations(index) + CalculateOtherRisk(index));
        if (risklevel < 4)
        {
            return "Low";
        }
        else if (risklevel >= 4 && risklevel <= 6)
        {
            return "Medium";
        }
        else
        {
            return "High";
        }
    }

    public int CalculateSecurityViolations(int index)
    {
        var email = emails[index];
        int violations = 0;
        if (email.contentSecurityPolicy == "Not Set")
        {
            violations++;
        }
        if (email.strictTransportSecurity == "Not Set")
        {
            violations++;
        }
        if (email.xFrameOptions == "Not Set")
        {
            violations++;
        }
        return violations;
    }

    public DateTime ParsingDates(string date, string format = "yyyy-MM-dd")
    {
        return ParseDate(date, format);
    }

    public int CalculateOtherRisk(int index)
    {
        var email = emails[index];
        int riskLevel = 0;
        var certExpiryDate = ParsingDates(email.certExpiryDate);
        var certIssueDate = ParsingDates(email.certIssueDate);
        var domainAgeDate = ParsingDates(email.domainAge);

        string certResult = CheckCertificate(certIssueDate, certExpiryDate, todayDate);
        string domainResult = CheckDomainAge(domainAgeDate, todayDate);
        
        if (certResult == "Expired")
        {
            riskLevel += 2;
        }
        if (domainResult == "Too young")
        {
            riskLevel++;
        }
        if (email.redirectsFound > 3)
        {
            riskLevel++;
        }
        if (CheckSuspiciousSender(index) == "Yes")
        {
            riskLevel += 2;
        }
        if(CheckGrammar(index) == "Yes")
        {
            riskLevel++;
        }
        return riskLevel;
    }

    static DateTime ParseDate(string dateString, string format)
    {
        return DateTime.ParseExact(dateString.Trim(), format, CultureInfo.InvariantCulture);
    }
  
    public string CheckCertificate(DateTime issueDate, DateTime expiryDate, DateTime today)
    {
        if (expiryDate < today)
        {
            return "Expired";
        }
        else
        {
            return "Valid";
        }
    }
    public string CheckDomainAge(DateTime domainAge, DateTime today)
    {
        TimeSpan ageDifference = today - domainAge;
        if (ageDifference.Days < 365)
        {
            return "Too young";
        }
        else
        {
            return "Acceptable";
        }
    }
    public string CheckGrammar(int index)
    {
        var email = emails[index];
        return email.grammarError;
    }
    public string CheckSuspiciousSender(int index)
    {
        var email = emails[index];
        return email.suspiciousSender;
    }
    public string CheckMarkAnswer(int index)
    {
        var email = emails[index];
        return email.markAnswer;
    }
    public string CallCertificateResult(int index)
    {
        var email = emails[index];
        var certExpiryDate = ParsingDates(email.certExpiryDate);
        var certIssueDate = ParsingDates(email.certIssueDate);
        return CheckCertificate(certIssueDate, certExpiryDate, todayDate);
    }

    public string CheckWebsiteType(int index)
    {
        var email = emails[index];
        return email.actionType;
    }

    public string CheckFileDownload(int index)
    {
        var email = emails[index];
        return email.fileType;
    }

    public void SetVMParams(int emailindex)
    {
        var email = emails[emailindex];
        detectionScript.SetVMParameters(email.actionType, email.fileType, email.websiteURL);
    }
}
