using System;
using System.Collections.Generic;
using System.Linq;
using ChatbotForm.Core.Models;
using ChatbotForm.Core.Services;
using ChatbotForm.Part1;

namespace POE_ST10434048.Part_2
{
    public abstract class Part2 : Part1
    {
        //stores the last topic discussed 
        public static string LastTopic;

        //dictionaries for tips 
        private static readonly Dictionary<int, string> PasswordTips = new Dictionary<int, string>()
        {
            { 0, "Use unique passwords for every account to prevent a breach on one site from compromising others" },
            { 1, "Routinely change your passwords, ideally every three months, to reduce the risk from leaked credentials" },
            { 2, "Never share your passwords and avoid writing them down in accessible places." },
            { 3, "Use a password manager to generate and store complex passwords securely." },
            { 4, "Make sure no one is watching when you enter your password, especially in public spaces." }
        };

        private static readonly Dictionary<int, string> PhishingTips = new Dictionary<int, string>()
        {
            { 0, "Be cautious with unsolicited emails, texts, or calls—assume unknown contacts could be scammers." },
            { 1, "Learn to spot phishing attempts: look for suspicious sender addresses, poor grammar, and urgent requests for personal data." },
            { 2, "Never share sensitive information like passwords or banking details via email or unsecured links." },
            { 3, "Always verify suspicious messages by contacting the sender through official channels, not by replying directly." },
            { 4, "Use email filters and firewalls to block known phishing sources and malicious content." }
        };

        private static readonly Dictionary<int, string> SafeBrowsingTips = new Dictionary<int, string>()
        {
            { 0, "Keep your browser and plugins up to date to patch security vulnerabilities." },
            { 1, "Use browsers with advanced privacy and security features, including pop-up blockers and anti-tracking tools." },
            { 2, "Enable private browsing or incognito mode to reduce tracking and protect your privacy." },
            { 3, "Avoid clicking on suspicious links or downloading files from untrusted sites." },
            { 4, "Use a VPN to encrypt your browsing activity, especially on public Wi-Fi." }
        };

        private static readonly Dictionary<int, string> MalwareTips = new Dictionary<int, string>()
        {
            { 0, "Install reputable antivirus software and keep it updated to detect and remove malware threats." },
            { 1, "Only download apps and files from trusted sources, such as official app stores." },
            { 2, "Avoid clicking on unverified links in emails, messages, or suspicious websites." },
            { 3, "Keep your operating system and all applications up to date to patch security holes." },
            { 4, "Regularly back up important data to recover quickly if malware strikes." }
        };

        private static readonly Dictionary<int, string> SocialEngineeringTips = new Dictionary<int, string>()
        {
            { 0, "Be skeptical of unsolicited requests for sensitive information, even if they appear to come from trusted sources." },
            { 1, "Educate yourself and others about common social engineering tactics, such as pretexting or baiting." },
            { 2, "Never reveal personal or financial information over the phone or online unless you initiated the contact." },
            { 3, "Verify identities before complying with requests for confidential data." },
            { 4, "Report suspicious behavior to your IT or security team immediately." }
        };

        private static readonly Dictionary<int, string> TwoFactorAuthenticationTips = new Dictionary<int, string>()
        {
            { 0, "Enable 2FA on all accounts that support it for an extra layer of security." },
            { 1, "Use authenticator apps or hardware tokens instead of SMS when possible for stronger protection." },
            { 2, "Never share your 2FA codes with anyone, even if they claim to be from customer support." },
            { 3, "Regularly review and update your 2FA settings and backup options." },
            { 4, "Be alert for phishing attempts that try to trick you into providing your 2FA code." }
        };

        private static readonly Dictionary<int, string> VpnTips = new Dictionary<int, string>()
        {
            { 0, "Use a VPN to encrypt your internet traffic, especially on public or unsecured Wi-Fi networks." },
            { 1, "Choose a reputable VPN provider that does not log your activity." },
            { 2, "Always connect to the VPN before accessing sensitive accounts or files remotely." },
            { 3, "Avoid free VPNs, as they may compromise your privacy or sell your data." },
            { 4, "Regularly update your VPN software to ensure you have the latest security features." }
        };

        private static readonly Dictionary<int, string> DataBreachTips = new Dictionary<int, string>()
        {
            { 0, "Monitor your accounts for signs of unauthorized access or suspicious activity." },
            { 1, "Change passwords immediately if you suspect a breach, and enable 2FA where possible." },
            { 2, "Use unique passwords for each account to minimize the impact of a breach." },
            { 3, "Stay informed about breaches by subscribing to security alerts or using breach notification services." },
            { 4, "Limit the amount of personal information you share online to reduce your risk exposure." }
        };

        private static readonly Dictionary<int, string> FirewallTips = new Dictionary<int, string>()
        {
            { 0, "Enable firewalls on all devices to block unauthorized inbound and outbound traffic." },
            { 1, "Regularly review and update firewall rules to ensure only necessary connections are allowed." },
            { 2, "Use both hardware (router-based) and software (device-based) firewalls for layered protection." },
            { 3, "Monitor firewall logs for unusual activity that could indicate an attack." },
            { 4, "Combine firewalls with other security measures, such as antivirus and intrusion detection systems, for comprehensive defense."
            }
        };

        private static readonly Dictionary<int, string> EncryptionTips = new Dictionary<int, string>()
        {
            { 0, "Encrypt sensitive files and communications to protect data from unauthorized access." },
            { 1, "Use full-disk encryption on laptops and mobile devices to secure data if the device is lost or stolen." },
            { 2, "Prefer encrypted messaging and email services for confidential communications." },
            { 3, "Regularly update encryption software to address vulnerabilities." },
            { 4, "Never share encryption keys or passwords except through secure, trusted channels." }
        };

        private static readonly Dictionary<string, Dictionary<string, string>> TopicDetails =
            new Dictionary<string, Dictionary<string, string>>()
            {
                {
                    "password", new Dictionary<string, string>()
                    {
                        {
                            "more",
                            "Strong passwords should be at least 12 characters long, contain a mix of uppercase, lowercase, numbers, and special characters, and avoid personal information. Change your passwords regularly and use different passwords for different accounts."
                        },
                        {
                            "example",
                            "A strong password example format is: 'Phrase-Number-Symbol' like 'BlueOcean-42-!&'. But don't use this exact one!"
                        },
                        {
                            "how",
                            "To create a strong password: use a passphrase, add numbers and symbols, don't use sequential characters, and check its strength with password strength meters."
                        },
                        {
                            "avoid",
                            "Avoid using common words, personal information (birthdays, names), sequential patterns (123456, qwerty), and reusing passwords across multiple sites."
                        }
                    }
                },
                {
                    "phishing",
                    new Dictionary<string, string>()
                    {
                        {
                            "more",
                            "Phishing attacks often create a sense of urgency, contain suspicious attachments, have generic greetings, and request sensitive information. Always verify the sender's email address and look for poor spelling or grammar."
                        },
                        {
                            "example",
                            "A common phishing example is an email claiming your account will be suspended unless you 'verify' your information by clicking on a malicious link."
                        },
                        {
                            "how",
                            "To protect yourself from phishing: verify sender addresses, don't click suspicious links, never share sensitive information via email, and use email filters and security tools."
                        },
                        {
                            "avoid",
                            "Avoid clicking links in unexpected emails, downloading suspicious attachments, providing personal information via email, and responding to urgent requests without verification."
                        }
                    }
                },
                {
                    "safe browsing",
                    new Dictionary<string, string>()
                    {
                        {
                            "more",
                            "Safe browsing involves using updated browsers, enabling security features, recognizing secure websites (HTTPS), using adblockers, and being cautious about downloads and permissions."
                        },
                        {
                            "example",
                            "An example of safe browsing is checking that a shopping site uses HTTPS (look for the padlock icon) before entering payment information."
                        },
                        {
                            "how",
                            "Practice safe browsing by updating your browser, using security extensions, checking for HTTPS, clearing cookies regularly, and using incognito mode when needed."
                        },
                        {
                            "avoid",
                            "Avoid visiting websites with security warnings, entering personal information on non-HTTPS sites, downloading files from untrusted sources, and using outdated browsers."
                        }
                    }
                },
                {
                    "malware",
                    new Dictionary<string, string>()
                    {
                        {
                            "more",
                            "Malware includes viruses, worms, trojans, ransomware, spyware, and adware. It can steal information, damage files, display unwanted ads, encrypt your data for ransom, or give hackers remote access to your device."
                        },
                        {
                            "example",
                            "Ransomware is a type of malware that encrypts your files and demands payment for the decryption key, like the notorious WannaCry attack."
                        },
                        {
                            "how",
                            "Protect against malware by installing reputable antivirus software, keeping your system updated, avoiding suspicious downloads, and regularly scanning your computer."
                        },
                        {
                            "avoid",
                            "Avoid downloading files from untrusted sources, clicking on suspicious ads, opening email attachments from unknown senders, and using pirated software."
                        }
                    }
                },
                {
                    "social engineering",
                    new Dictionary<string, string>()
                    {
                        {
                            "more",
                            "Social engineering manipulates people into breaking security protocols by exploiting human psychology rather than technical hacking. Common techniques include pretexting, baiting, quid pro quo, and tailgating."
                        },
                        {
                            "example",
                            "An example is when someone calls pretending to be IT support, claiming there's an issue with your account and asking for your password to 'fix' it."
                        },
                        {
                            "how",
                            "Defend against social engineering by verifying identities through official channels, being skeptical of unsolicited requests, following security protocols, and reporting suspicious activities."
                        },
                        {
                            "avoid",
                            "Avoid sharing sensitive information without verification, granting access based on appearance or claims alone, clicking on enticing but suspicious offers, and allowing tailgating into secure areas."
                        }
                    }
                },
                {
                    "two factor authentication",
                    new Dictionary<string, string>()
                    {
                        {
                            "more",
                            "Two-factor authentication adds a second verification layer using something you know (password), something you have (phone), or something you are (fingerprint). This significantly improves security even if your password is compromised."
                        },
                        {
                            "example",
                            "An example is when you log into your email and receive a text message with a code that you must enter to complete the login process."
                        },
                        {
                            "how",
                            "Enable 2FA on all important accounts through account security settings. Use authenticator apps when possible as they're more secure than SMS codes."
                        },
                        {
                            "avoid",
                            "Avoid using the same device for both factors, sharing verification codes with others, ignoring 2FA setup options, and using SMS 2FA when more secure options are available."
                        }
                    }
                },
                {
                    "vpn",
                    new Dictionary<string, string>()
                    {
                        {
                            "more",
                            "VPNs encrypt your internet connection, hiding your IP address and online activities from potential snoopers, ISPs, and websites. They're essential on public WiFi and can bypass geo-restrictions."
                        },
                        {
                            "example",
                            "When you connect to a coffee shop WiFi with a VPN, your banking information remains encrypted and secure even if someone is monitoring the network."
                        },
                        {
                            "how",
                            "To use a VPN effectively, choose a reputable provider, connect before accessing sensitive information, use kill switch features, and keep the VPN software updated."
                        },
                        {
                            "avoid",
                            "Avoid free VPNs that might sell your data, using outdated VPN protocols, keeping the VPN off on public networks, and assuming VPNs make you completely anonymous."
                        }
                    }
                },
                {
                    "data breach",
                    new Dictionary<string, string>()
                    {
                        {
                            "more",
                            "Data breaches occur when unauthorized parties access sensitive data. They can expose personal, financial, or confidential information affecting individuals and organizations."
                        },
                        {
                            "example",
                            "A major data breach example was the Equifax breach in 2017, which exposed personal information of 147 million people, including Social Security numbers and birth dates."
                        },
                        {
                            "how",
                            "Respond to a data breach by changing passwords immediately, monitoring accounts for suspicious activity, freezing credit if needed, and using breach notification services."
                        },
                        {
                            "avoid",
                            "Avoid ignoring breach notifications, reusing passwords across sites, sharing too much personal information online, and neglecting to check if your information has been compromised."
                        }
                    }
                },
                {
                    "firewall",
                    new Dictionary<string, string>()
                    {
                        {
                            "more",
                            "Firewalls monitor and filter network traffic based on security rules. They can be hardware devices, software programs, or both, creating a barrier between trusted and untrusted networks."
                        },
                        {
                            "example",
                            "A personal firewall example is Windows Defender Firewall, which blocks unauthorized access attempts to your computer while allowing legitimate traffic."
                        },
                        {
                            "how",
                            "Configure firewalls by enabling them on all devices, setting appropriate rules, updating regularly, and monitoring logs for suspicious activity."
                        },
                        {
                            "avoid",
                            "Avoid disabling firewalls for convenience, creating overly permissive rules, ignoring firewall alerts, and failing to configure both incoming and outgoing traffic filters."
                        }
                    }
                },
                {
                    "encryption",
                    new Dictionary<string, string>()
                    {
                        {
                            "more",
                            "Encryption converts data into coded text that can only be deciphered with the correct key. It protects data at rest (stored) and in transit (being sent), making it unreadable to unauthorized users."
                        },
                        {
                            "example",
                            "When you see HTTPS in your browser's address bar, it means the connection is encrypted, protecting information like passwords and credit card details."
                        },
                        {
                            "how",
                            "Implement encryption by using HTTPS websites, enabling full-disk encryption on devices, using encrypted messaging apps, and encrypting sensitive files."
                        },
                        {
                            "avoid",
                            "Avoid sending sensitive information over unencrypted connections, storing encryption keys insecurely, using outdated encryption protocols, and neglecting to encrypt mobile devices."
                        }
                    }
                }
            };

        // Method to handle follow-up questions more comprehensively
        public static string HandleFollowUp(string userInput)
        {
            if (string.IsNullOrEmpty(userInput) || string.IsNullOrEmpty(LastTopic))
                return null;

            userInput = userInput.ToLower();

            // Check if the user is asking for a tip on the current topic
            if (userInput.Contains("tip") || userInput.Contains("tips"))
            {
                return DisplayTips(LastTopic); // Return the tip string instead of displaying directly
            }

            // Standardise topic name for dictionary lookup
            string topicKey = LastTopic.ToLower();

            // Check if we have details for this topic
            if (!TopicDetails.ContainsKey(topicKey))
                return $"I don't have additional information about {LastTopic} at the moment.";

            // Check for key follow-up indicators
            foreach (var detailType in TopicDetails[topicKey].Keys)
            {
                if (userInput.Contains(detailType) ||
                    (detailType == "more" && IsGenericMoreQuestion(userInput)) ||
                    (detailType == "how" && IsHowToQuestion(userInput)) ||
                    (detailType == "avoid" && IsAvoidQuestion(userInput)) ||
                    (detailType == "example" && IsExampleQuestion(userInput)))
                {
                    return TopicDetails[topicKey][detailType];
                }
            }

            // Default response if we couldn't match the follow-up to a specific type
            return
                $"I'm not sure what specific aspect of {LastTopic} you're asking about. You can ask for more information, examples, how to implement it, or what to avoid.";
        }

        //used AI to generate different forms of questions like generic questions, how to and so on
        //detect question types
        private static bool IsGenericMoreQuestion(string input)
        {
            return input.Contains("tell me more") ||
                   input.Contains("additional information") ||
                   input.Contains("explain more") ||
                   input.Contains("details");
        }

        private static bool IsHowToQuestion(string input)
        {
            return input.Contains("how do i") ||
                   input.Contains("how to") ||
                   input.Contains("how can i") ||
                   input.Contains("steps to") ||
                   input.Contains("way to");
        }

        private static bool IsAvoidQuestion(string input)
        {
            return input.Contains("avoid") ||
                   input.Contains("prevent") ||
                   input.Contains("don't") ||
                   input.Contains("risk") ||
                   input.Contains("dangerous");
        }

        private static bool IsExampleQuestion(string input)
        {
            return input.Contains("example") ||
                   input.Contains("instance") ||
                   input.Contains("show me") ||
                   input.Contains("such as") ||
                   input.Contains("illustration");
        }

        // Method to set the topic being discussed
        public static void SetTopic(string topic)
        {
            switch (topic.ToLower())
            {
                // looking at different switch cases, the topic can take 
                case "password":
                case "passwords":
                    LastTopic = "password";
                    break;

                case "phishing":
                case "scam":
                case "email scam":
                    LastTopic = "phishing";
                    break;

                case "safe browsing":
                case "browser safety":
                case "web safety":
                    LastTopic = "safe browsing";
                    break;

                case "malware":
                case "virus":
                case "spyware":
                case "ransomware":
                    LastTopic = "malware";
                    break;

                case "social engineering":
                case "manipulation":
                case "human hacking":
                    LastTopic = "social engineering";
                    break;

                case "two factor":
                case "2fa":
                case "two factor authentication":
                case "mfa":
                    LastTopic = "two factor authentication";
                    break;

                case "vpn":
                case "virtual private network":
                    LastTopic = "vpn";
                    break;

                case "data breach":
                case "leak":
                case "stolen data":
                    LastTopic = "data breach";
                    break;

                case "firewall":
                case "network protection":
                    LastTopic = "firewall";
                    break;

                case "encryption":
                case "encrypt":
                case "cryptography":
                    LastTopic = "encryption";
                    break;

                default:
                    LastTopic = topic;
                    break;
            }
        }


        //lists for the different tips on the topics, creating both an original list for repopulation and a current list for using and removing so as to not have duplicate tips displayed
        private static readonly List<string> OriginalPasswordTipsList = PasswordTips.Values.ToList();
        private static List<string> _passwordTipsList = new List<string>(OriginalPasswordTipsList);

        private static readonly List<string> OriginalPhishingTipsList = PhishingTips.Values.ToList();
        private static List<string> _phishingTipsList = new List<string>(OriginalPhishingTipsList);

        private static readonly List<string> OriginalSafeBrowsingTipsList = SafeBrowsingTips.Values.ToList();
        private static List<string> _safeBrowsingTipsList = new List<string>(OriginalSafeBrowsingTipsList);

        private static readonly List<string> OriginalMalwareTipsList = MalwareTips.Values.ToList();
        private static List<string> _malwareTipsList = new List<string>(OriginalMalwareTipsList);

        private static readonly List<string> OriginalSocialTipsList = SocialEngineeringTips.Values.ToList();
        private static List<string> _socialTipsList = new List<string>(OriginalSocialTipsList);

        private static readonly List<string> OriginalTfaTipsList = TwoFactorAuthenticationTips.Values.ToList();
        private static List<string> _tfaTipsList = new List<string>(OriginalTfaTipsList);

        private static readonly List<string> OriginalVpnTipsList = VpnTips.Values.ToList();
        private static List<string> _vpnTipsList = new List<string>(OriginalVpnTipsList);

        private static readonly List<string> OriginalDataTipsList = DataBreachTips.Values.ToList();
        private static List<string> _dataTipsList = new List<string>(OriginalDataTipsList);

        private static readonly List<string> OriginalFirewallTipsList = FirewallTips.Values.ToList();
        private static List<string> _firewallTipsList = new List<string>(OriginalFirewallTipsList);

        private static readonly List<string> OriginalEncryptionTipsList = EncryptionTips.Values.ToList();
        private static List<string> _encryptionTipsList = new List<string>(OriginalEncryptionTipsList);


        //Random Generator
private static readonly Random Random = new Random();

//display random tips - modified for Windows Forms
private static string ShowRandomPhishingTips()
{
    if (_phishingTipsList.Count == 0)
    {
        _phishingTipsList = new List<string>(OriginalPhishingTipsList);
        return "No more tips available. Tips have been refreshed.";
    }

    var randomIndex = Random.Next(_phishingTipsList.Count);
    string tip = _phishingTipsList[randomIndex];
    _phishingTipsList.RemoveAt(randomIndex);
    return tip;
}

private static string ShowRandomPasswordTips()
{
    if (_passwordTipsList.Count == 0)
    {
        _passwordTipsList = new List<string>(OriginalPasswordTipsList);
        return "No more tips available. Tips have been refreshed.";
    }

    int randomIndex = Random.Next(_passwordTipsList.Count);
    string tip = _passwordTipsList[randomIndex];
    _passwordTipsList.RemoveAt(randomIndex);
    return tip;
}

private static string ShowRandomSafeBrowsingTips()
{
    if (_safeBrowsingTipsList.Count == 0)
    {
        _safeBrowsingTipsList = new List<string>(OriginalSafeBrowsingTipsList);
        return "No more tips available. Tips have been refreshed.";
    }

    int randomIndex = Random.Next(_safeBrowsingTipsList.Count);
    string tip = _safeBrowsingTipsList[randomIndex];
    _safeBrowsingTipsList.RemoveAt(randomIndex);
    return tip;
}

private static string ShowRandomMalwareTips()
{
    if (_malwareTipsList.Count == 0)
    {
        _malwareTipsList = new List<string>(OriginalMalwareTipsList);
        return "No more tips available. Tips have been refreshed.";
    }

    int randomIndex = Random.Next(_malwareTipsList.Count);
    string tip = _malwareTipsList[randomIndex];
    _malwareTipsList.RemoveAt(randomIndex);
    return tip;
}

private static string ShowRandomSocialEngineeringTips()
{
    if (_socialTipsList.Count == 0)
    {
        _socialTipsList = new List<string>(OriginalSocialTipsList);
        return "No more tips available. Tips have been refreshed.";
    }

    int randomIndex = Random.Next(_socialTipsList.Count);
    string tip = _socialTipsList[randomIndex];
    _socialTipsList.RemoveAt(randomIndex);
    return tip;
}

private static string ShowRandomTwoFactorAuthenticationTips()
{
    if (_tfaTipsList.Count == 0)
    {
        _tfaTipsList = new List<string>(OriginalTfaTipsList);
        return "No more tips available. Tips have been refreshed.";
    }

    int randomIndex = Random.Next(_tfaTipsList.Count);
    string tip = _tfaTipsList[randomIndex];
    _tfaTipsList.RemoveAt(randomIndex);
    return tip;
}

private static string ShowRandomVpnTips()
{
    if (_vpnTipsList.Count == 0)
    {
        _vpnTipsList = new List<string>(OriginalVpnTipsList);
        return "No more tips available. Tips have been refreshed.";
    }

    int randomIndex = Random.Next(_vpnTipsList.Count);
    string tip = _vpnTipsList[randomIndex];
    _vpnTipsList.RemoveAt(randomIndex);
    return tip;
}

private static string ShowRandomDataBreachTips()
{
    if (_dataTipsList.Count == 0)
    {
        _dataTipsList = new List<string>(OriginalDataTipsList);
        return "No more tips available. Tips have been refreshed.";
    }

    int randomIndex = Random.Next(_dataTipsList.Count);
    string tip = _dataTipsList[randomIndex];
    _dataTipsList.RemoveAt(randomIndex);
    return tip;
}

private static string ShowRandomFirewallTips()
{
    if (_firewallTipsList.Count == 0)
    {
        _firewallTipsList = new List<string>(OriginalFirewallTipsList);
        return "No more tips available. Tips have been refreshed.";
    }

    int randomIndex = Random.Next(_firewallTipsList.Count);
    string tip = _firewallTipsList[randomIndex];
    _firewallTipsList.RemoveAt(randomIndex);
    return tip;
}

private static string ShowRandomEncryptionTips()
{
    if (_encryptionTipsList.Count == 0)
    {
        _encryptionTipsList = new List<string>(OriginalEncryptionTipsList);
        return "No more tips available. Tips have been refreshed.";
    }

    int randomIndex = Random.Next(_encryptionTipsList.Count);
    string tip = _encryptionTipsList[randomIndex];
    _encryptionTipsList.RemoveAt(randomIndex);
    return tip;
}

public static string DisplayTips(string topic)
{
    // Standardize topic name
    string topicLower = topic.ToLower();

    // Map from various inputs to standard topic names
    switch (topicLower)
    {
        case "password":
        case "passwords":
        case "credential":
        case "login":
            return ShowRandomPasswordTips();

        case "phishing":
        case "scam":
        case "email scam":
        case "fake email":
            return ShowRandomPhishingTips();

        case "safe browsing":
        case "browser safety":
        case "surfing":
        case "secure browsing":
        case "web safety":
            return ShowRandomSafeBrowsingTips();

        case "malware":
        case "virus":
        case "spyware":
        case "ransomware":
            return ShowRandomMalwareTips();

        case "social engineering":
        case "manipulation":
        case "psychological":
        case "human hacking":
            return ShowRandomSocialEngineeringTips();

        case "two factor authentication":
        case "two factor":
        case "2fa":
        case "mfa":
        case "authentication":
            return ShowRandomTwoFactorAuthenticationTips();

        case "vpn":
        case "virtual private network":
        case "secure connection":
            return ShowRandomVpnTips();

        case "data breach":
        case "leak":
        case "stolen data":
        case "exposed information":
            return ShowRandomDataBreachTips();

        case "firewall":
        case "network protection":
        case "security barrier":
            return ShowRandomFirewallTips();

        case "encryption":
        case "encrypt":
        case "cipher":
        case "cryptography":
            return ShowRandomEncryptionTips();

        default:
            return $"I don't have tips on {topic}. You can ask about topics like passwords, phishing, malware, encryption, and more.";
    }
}
    }
}