using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class GlossaryScript : MonoBehaviour
{
    public GameObject introTextt;
    
    public GameObject glossaryTextt;
    public TMP_Text glossaryText;

    List<string> glossaryDefinitions = new List<string>
    {
        {"Definition: A security header that specifies trusted sources of content for a webpage. " +
            "It defines which resources, such as scripts, images, or styles, can be loaded to ensure the site’s integrity." +
            "\r\n\r\n\r\nImportance: CSP helps prevent malicious scripts or unauthorized resources from executing on websites, " +
            "reducing the risk of cross-site scripting (XSS) attacks and data breaches." },

        {"Definition: The length of time a domain name has been registered and active. " +
            "Older domains are generally perceived as more trustworthy compared to newly registered ones." +
            "\r\n\r\nImportance: Scammers often use newly created domains for phishing websites, " +
            "making domain age a useful metric for identifying potential threats." },

        {"Definition: A deceptive cyberattack where attackers pose as legitimate entities to trick individuals into sharing " +
            "sensitive information like passwords or credit card details.\r\n\r\nImportance: " +
            "Recognizing phishing attempts is critical to protecting personal and financial data from theft or unauthorized access." },

        {"Definition: The automatic process of sending users from one URL to another, either intentionally or maliciously." +
            "\r\n\r\nImportance: Attackers often use redirects to disguise malicious websites. " +
            "Knowing how to analyze redirects helps avoid being misled by deceptive links." },

        {"Definition: HTTP response headers that improve the security of a web application by specifying rules on how the browser should handle content." +
            "\r\n\r\nImportance: These headers mitigate risks such as cross-site scripting (XSS), clickjacking, " +
            "and other vulnerabilities, making web browsing safer." },

        {"Definition: Actions or events that compromise the confidentiality, integrity, or availability of a system or information." +
            "\r\n\r\nImportance: Detecting and addressing security violations quickly " +
            "can prevent data breaches and ensure a secure digital environment." },

        {"Definition: A digital certificate that establishes a secure and encrypted connection between a user’s browser and a website’s server." +
            "\r\n\r\nImportance: SSL certificates ensure sensitive data, like passwords and payment details, " +
            "are transmitted securely, protecting users from eavesdropping and data theft." },

        {"Definition: A security feature that enforces HTTPS connections, ensuring secure communication between a user’s browser and a website." +
            "\r\n\r\nImportance: HSTS helps prevent man-in-the-middle attacks and ensures that " +
            "sensitive data is always encrypted during transmission." },

        {"Definition: An updated and more secure version of SSL, providing strong encryption and authentication for online communications." +
            "\r\n\r\nImportance: TLS certificates protect sensitive data such as login credentials and credit card numbers, " +
            "enhancing trust in online transactions." },

        {"Definition: Various strategies attackers use to execute phishing attacks, such as:" +
            "\r\n\r\nEmail Phishing: Fake emails mimicking legitimate sources.\r\n\r\nSmishing: Phishing through SMS messages." +
            "\r\n\r\nVishing: Voice-based phishing scams.\r\n\r\nSpear Phishing: Targeted attacks on specific individuals or organizations." +
            "\r\n\r\nImportance: Understanding these techniques helps users identify and defend against phishing in its many forms." },

        {"Definition: The web address used to locate a resource, such as a webpage, on the internet." +
            "\r\n\r\nImportance: Examining URLs can reveal suspicious or fraudulent websites, " +
            "as phishing sites often use similar-looking but fake addresses to deceive users." },

        {"Definition: A software-based environment that mimics a physical computer and runs its own operating system." +
            "\r\n\r\nImportance: Virtual machines allow users to safely test files or applications in isolation, " +
            "minimizing risks to the main system." },

        {"Definition: Malicious software designed to replicate and spread to disrupt, damage, or gain unauthorized access to computer systems." +
            "\r\n\r\nImportance: Identifying and eliminating viruses is crucial to maintaining a secure and functional computing environment." },

        {"Definition: A security header that controls whether a webpage can be displayed in a frame or iframe, " +
            "often to prevent embedding by other sites.\r\n\r\nImportance: This helps prevent clickjacking attacks, " +
            "where malicious sites trick users into clicking hidden links or buttons embedded in legitimate-looking pages." },
    };

    public int index = 0;
    public void DropdownOption(int index)
    {
        if (index > 0)
        {
            introTextt.SetActive(false);
            glossaryTextt.SetActive(true);
            glossaryText.text = glossaryDefinitions[index-1];
        }
        else if (index == 0)
        {
            glossaryTextt.SetActive(false);
            introTextt.SetActive(true);
        }
        
    }

    public void Prevbtn()
    {
        if (index > 0)
        {
            index--;
            DropdownOption(index);
        }
    }

    public void Nextbtn()
    {
        if (index < glossaryDefinitions.Count)
        {
            index++;
            DropdownOption(index);
        }
    }

}
