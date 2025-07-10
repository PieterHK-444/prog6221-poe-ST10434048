using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading;
using ChatbotForm.Core.Services;
using POE_ST10434048.Part_2;

namespace ChatbotForm.Part1
{
    public class Part1
    {
        internal static string _name;
        public static string asciiArt = @"
       _______
     _/       \_
    / |       | \
   /  |__   __|  \
  |__/((o| |o))\__|
  |      | |      |
  |\     |_|     /|
  | \           / |
   \| /  ___  \ |/
    \ | / _ \ | /
     \_________/
      _|_____ |_
 ____|_________|____
/                   \";

        //used Claud AI to learn how to use colour theming in C#
        private const ConsoleColor MainColour = ConsoleColor.Cyan;
        private const ConsoleColor SecondaryColour = ConsoleColor.Green;
        private const ConsoleColor YellowHighlightColour = ConsoleColor.Yellow;
        private const ConsoleColor ErrorColour = ConsoleColor.Red;
        private const ConsoleColor ProgressColour = ConsoleColor.Magenta;

        // Dictionary for keyword-based responses
        public static readonly Dictionary<string, List<string>> KeywordResponses =
            new Dictionary<string, List<string>>();

        public static void entry()
        {
            // Call InitializeKeywords()
            InitializeKeywords();
            ImageDisp();
            PlayAudio();
            Welcome();
            Response();
        }

        //fucntion to find the user's favourite topic
        public static string ExtractFavouriteTopic(string input)
        {
            // The Possible phrases a user might use
            string[] keywords =
            {
                "favourite topic is", "favorite topic is", "favourite topic:", "favorite topic:", "i like",
                "my favourite", "my favorite"
            };

            foreach (var keyword in keywords)
            {
                int index = input.IndexOf(keyword);
                if (index >= 0)
                {
                    // Extract substring after keyword
                    int start = index + keyword.Length;
                    string topic = input.Substring(start).Trim();

                    // Optionally, remove trailing punctuation
                    topic = topic.TrimEnd('.', '!', '?');

                    return topic;
                }
            }

            return null;
        }


        // Initialise keyword dictionary for input handling
        public static void InitializeKeywords()
        {
            KeywordResponses.Add("exit",
                new List<string> { "exit", "quit", "bye", "end", "goodbye", "close", "leave", "stop" });

            // Questions about how the bot is / what it is 
            KeywordResponses.Add("how are you",
                new List<string> { "how are you", "how you doing", "how's it going", "how do you feel" });
            KeywordResponses.Add("purpose",
                new List<string> { "goal", "aim", "objective", "do you do", "what are you", "who are you" });
            KeywordResponses.Add("ask about",
                new List<string> { "help with", "tell me", "topics", "information", "what can you do" });

            // Cybersecurity topics
            KeywordResponses.Add("password",
                new List<string> { "passwords", "passcode", "credential", "login", "strong password" });
            KeywordResponses.Add("phishing",
                new List<string> { "scam", "email scam", "fake email", "pretend", "spam" });
            KeywordResponses.Add("safe browsing",
                new List<string> { "browser safety", "surfing", "secure browsing", "web safety", "safe websites" });
            KeywordResponses.Add("malware",
                new List<string> { "virus", "spyware", "adware", "ransomware", "computer virus" });
            KeywordResponses.Add("social engineering",
                new List<string> { "manipulation", "psychological", "human hacking", "pretexting" });
            KeywordResponses.Add("two factor",
                new List<string> { "2fa", "mfa", "multi factor", "authentication", "verify identity" });
            KeywordResponses.Add("vpn",
                new List<string>
                    { "virtual private network", "secure connection", "hide ip", "encrypt traffic", "proxy" });
            KeywordResponses.Add("data breach",
                new List<string> { "leak", "stolen data", "hack", "exposed information", "compromised data" });
            KeywordResponses.Add("firewall",
                new List<string> { "network protection", "blocking", "filter", "security barrier", "network shield" });
            KeywordResponses.Add("encryption",
                new List<string> { "encrypt", "cipher", "scramble", "cryptography", "secure data" });

            //part 2 addition 
            KeywordResponses.Add("favourite topic",
                new List<string> { "favorite topic", "favourite topic", "my favourite", "my favorite" });
            KeywordResponses.Add("tips",
                new List<string> { "tips", "tip" });
        }

        //part 2 addition
        private static string ExtractTipsTopic(string input)
        {
            // Check for each topic in the input (excluding "tips")
            foreach (var key in KeywordResponses.Keys)
            {
                if (key == "tips" || key == "exit") continue;
                if (ContainsKeywords(input, key))
                    return key;
            }

            return null;
        }

        //respond to questions using input handling
        private static void Response()
        {
            while (true)
            {
                DrawTitledBorder("Ask Me Anything");
                SetConsoleColor(YellowHighlightColour);
                TypewriterEffect("Do you have any questions for me about cybersecurity?");
                ResetConsoleColor();
                DrawBorder();

                string response = Console.ReadLine();
                if (string.IsNullOrEmpty(response))
                    continue;

                string responseLower = response.ToLower();
                //part 2 addition
                string sentiment = SentimentCheck.DetectSentiment(responseLower);
                string empathyMsg = SentimentCheck.GetEmpathyMessage(sentiment);

                if (!string.IsNullOrEmpty(empathyMsg))
                {
                    SetConsoleColor(SecondaryColour);
                    TypewriterEffect(empathyMsg);
                    ResetConsoleColor();
                }


                // Check for exit commands
                if (ContainsKeywords(responseLower, "exit"))
                {
                    DrawTitledBorder("Goodbye");
                    SetConsoleColor(MainColour);
                    TypewriterEffect("Thank you for using the Cybersecurity Awareness Bot. Stay safe online!");
                    ResetConsoleColor();
                    DrawBorder();
                    return;
                }

                DrawTypingAnimation("Thinking", 1);

                //part 2 addition
                // Check if the user is sharing their favourite topic
                if (responseLower.Contains("favourite topic") || responseLower.Contains("favorite topic") ||
                    responseLower.Contains("i like") || responseLower.Contains("my favourite") ||
                    responseLower.Contains("my favorite"))
                {
                    string topic = ExtractFavouriteTopic(responseLower);
                    if (!string.IsNullOrEmpty(topic))
                    {
                        UserMemory.StoreFavouriteTopic(topic);
                        DrawTitledBorder("Favourite Topic Stored");
                        SetConsoleColor(MainColour);
                        TypewriterEffect($"Thanks! I've saved your favourite topic as '{topic}'.");
                        ResetConsoleColor();
                        DrawBorder();
                        continue;
                    }
                }

                //part 2 addition
                if (ContainsKeywords(responseLower, "tips"))
                {
                    string topic = ExtractTipsTopic(responseLower);
                    DrawTitledBorder("Tips");
                    SetConsoleColor(MainColour);
                    if (!string.IsNullOrEmpty(topic))
                    {
                        Part2.DisplayTips(topic);
                    }
                    else
                    {
                        TypewriterEffect("Please specify a topic to get tips on (e.g., 'tips on passwords').");
                    }

                    ResetConsoleColor();
                    DrawBorder();
                    continue;
                }


                //part 2 addition 
                // Find the first matching key
                string matchedKey = KeywordResponses.Keys
                    .FirstOrDefault(key => ContainsKeywords(responseLower, key));
                string followup;
                switch (matchedKey)
                {
                    case "how are you":
                        DrawTitledBorder("Bot Status");
                        SetConsoleColor(MainColour);
                        TypewriterEffect(
                            "I'm doing great, thanks for asking! I'm here to help you learn about cybersecurity.");
                        ResetConsoleColor();
                        DrawBorder();
                        break;

                    case "purpose":
                        DrawTitledBorder("Bot Purpose");
                        SetConsoleColor(MainColour);
                        TypewriterEffect(
                            "My purpose is to help you learn about cybersecurity topics in a conversational way. I can answer questions about various aspects of staying safe online.");
                        ResetConsoleColor();
                        DrawBorder();
                        break;

                    case "ask about":
                        DrawTitledBorder("Available Topics");
                        SetConsoleColor(MainColour);
                        TypewriterEffect(
                            "You can ask me about: password security, phishing attacks, safe browsing habits, malware protection, social engineering, two-factor authentication, VPNs, data breaches, firewalls, and encryption.");
                        ResetConsoleColor();
                        DrawBorder();
                        break;

                    case "password":
                        ShowPasswordInfo();
                        Part2.SetTopic("password");
                        TypewriterEffect(
                            $"You can ask a follow up question on {Part2.LastTopic} or press enter to talk about a different topic.");
                        followup = Console.ReadLine();
                        ProcessFollowUp(followup);
                        break;

                    case "phishing":
                        ShowPhishingInfo();
                        Part2.SetTopic("phishing");
                        TypewriterEffect(
                            $"You can ask a follow up question on {Part2.LastTopic} or press enter to talk about a different topic.");
                        followup = Console.ReadLine();
                        ProcessFollowUp(followup);
                        break;

                    case "safe browsing":
                        ShowSafeBrowsingInfo();
                        Part2.SetTopic("safe browsing");
                        TypewriterEffect(
                            $"You can ask a follow up question on {Part2.LastTopic} or press enter to talk about a different topic.");
                        followup = Console.ReadLine();
                        ProcessFollowUp(followup);
                        break;

                    case "malware":
                        ShowMalwareInfo();
                        Part2.SetTopic("malware");
                        TypewriterEffect(
                            $"You can ask a follow up question on {Part2.LastTopic} or press enter" +
                            $"to talk about a different topic.");
                        followup = Console.ReadLine();
                        ProcessFollowUp(followup);
                        break;

                    case "social engineering":
                        ShowSocialEngineeringInfo();
                        Part2.SetTopic("social engineering");
                        TypewriterEffect(
                            $"You can ask a follow up question on {Part2.LastTopic} or press enter to talk about a different topic.");
                        followup = Console.ReadLine();
                        ProcessFollowUp(followup);
                        break;

                    case "two factor":
                        Show2FaInfo();
                        Part2.SetTopic("two factor");
                        TypewriterEffect(
                            $"You can ask a follow up question on {Part2.LastTopic} or press enter to talk about a different topic.");
                        followup = Console.ReadLine();
                        ProcessFollowUp(followup);
                        break;

                    case "vpn":
                        ShowVpnInfo();
                        Part2.SetTopic("vpn");
                        TypewriterEffect(
                            $"You can ask a follow up question on {Part2.LastTopic} or press enter to talk about a different topic.");
                        Console.WriteLine();
                        followup = Console.ReadLine();
                        Part2.HandleFollowUp(followup);
                        ProcessFollowUp(followup);
                        break;

                    case "data breach":
                        ShowDataBreachInfo();
                        Part2.SetTopic("data breach");
                        TypewriterEffect(
                            $"You can ask a follow up question on {Part2.LastTopic} or press enter to talk about a different topic.");
                        followup = Console.ReadLine();
                        Part2.HandleFollowUp(followup);
                        ProcessFollowUp(followup);
                        break;

                    case "firewall":
                        ShowFirewallInfo();
                        Part2.SetTopic("firewall");
                        TypewriterEffect(
                            $"You can ask a follow up question on {Part2.LastTopic} or press enter to talk about a different topic.");
                        followup = Console.ReadLine();
                        Part2.HandleFollowUp(followup);
                        break;

                    case "encryption":
                        ShowEncryptionInfo();
                        Part2.SetTopic("encryption");
                        TypewriterEffect(
                            $"You can ask a follow up question on {Part2.LastTopic} or press enter to talk about a different topic.");
                        followup = Console.ReadLine();
                        Part2.HandleFollowUp(followup);
                        break;

                    default:
                        DrawTitledBorder("Not Understood");
                        SetConsoleColor(ErrorColour);
                        TypewriterEffect(
                            "I'm not sure I understand that question. You can ask me about cybersecurity topics like password safety, phishing, malware, and more. Or type 'ask about' to see all topics I can help with.");
                        ResetConsoleColor();
                        DrawBorder();
                        break;
                }
            }
        }

        //part 2 addition 
        // Add this improved version of ProcessFollowUp to your Program class
        private static void ProcessFollowUp(string followup)
        {
            // If the input is empty, return
            if (string.IsNullOrWhiteSpace(followup))
                return;

            // Check if the user wants to change topics
            string newTopic = CheckForNewTopic(followup.ToLower());
            if (newTopic != null)
            {
                // User wants to talk about a different topic
                ShowTopicInfo(newTopic);
                return;
            }

            // Detect sentiment and show an empathy message
            string sentiment = SentimentCheck.DetectSentiment(followup.ToLower());
            string empathyMsg = SentimentCheck.GetEmpathyMessage(sentiment);
            if (!string.IsNullOrEmpty(empathyMsg))
            {
                SetConsoleColor(SecondaryColour);
                TypewriterEffect(empathyMsg);
                ResetConsoleColor();
            }

            // Check if the user is requesting tips
            if (followup.ToLower().Contains("tips") || followup.ToLower().Contains("tip"))
            {
                DrawTitledBorder("Tips");
                SetConsoleColor(MainColour);
                Part2.DisplayTips(Part2.LastTopic);
                ResetConsoleColor();
                DrawBorder();
                return;
            }

            // Handle the follow-up question and display the response
            string followUpResponse = Part2.HandleFollowUp(followup);
            if (!string.IsNullOrWhiteSpace(followUpResponse))
            {
                DrawTitledBorder($"More About {Part2.LastTopic}");
                SetConsoleColor(MainColour);
                TypewriterEffect(followUpResponse);
                ResetConsoleColor();
                DrawBorder();

                // Offer to continue the conversation
                SetConsoleColor(YellowHighlightColour);
                TypewriterEffect(
                    $"Would you like to know more about {Part2.LastTopic}, get a tip, or learn about something else?");
                ResetConsoleColor();
                string nextInput = Console.ReadLine();
                ProcessFollowUp(nextInput); // Recursively process the next input
            }
            else
            {
                // If no specific follow-up response was generated, but input wasn't empty
                DrawTitledBorder("Need Help?");
                SetConsoleColor(MainColour);
                TypewriterEffect(
                    $"If you'd like to learn about a different topic, just mention it. Or you can ask me for more details about {Part2.LastTopic}.");
                ResetConsoleColor();
                DrawBorder();
            }
        }

        //part 2 addition
        // Helper method to check if the user wants to change topics
        private static string CheckForNewTopic(string input)
        {
            // List of topics to check for
            string[] topicKeywords =
            {
                "password", "phishing", "safe browsing", "malware",
                "social engineering", "two factor", "vpn",
                "data breach", "firewall", "encryption"
            };

            // Check if the user mentions a topic different from the current one
            foreach (var topic in topicKeywords)
            {
                if (ContainsKeywords(input, topic) &&
                    !string.IsNullOrEmpty(Part2.LastTopic) &&
                     !Part2.LastTopic.ToLower().Contains(topic))
                {
                    return topic;
                }
            }

            return null;
        }

        //part 2 addition 
        //allows for changing the topic to have a more seamless conversation
        private static void ShowTopicInfo(string topic)
        {
            DrawTitledBorder($"Changing Topic to {topic}");
            SetConsoleColor(MainColour);
            TypewriterEffect($"Let me tell you about {topic} instead.");
            ResetConsoleColor();

            switch (topic)
            {
                case "password":
                    ShowPasswordInfo();
                    break;
                case "phishing":
                    ShowPhishingInfo();
                    break;
                case "safe browsing":
                    ShowSafeBrowsingInfo();
                    break;
                case "malware":
                    ShowMalwareInfo();
                    break;
                case "social engineering":
                    ShowSocialEngineeringInfo();
                    break;
                case "two factor":
                    Show2FaInfo();
                    break;
                case "vpn":
                    ShowVpnInfo();
                    break;
                case "data breach":
                    ShowDataBreachInfo();
                    break;
                case "firewall":
                    ShowFirewallInfo();
                    break;
                case "encryption":
                    ShowEncryptionInfo();
                    break;
            }

            Part2.SetTopic(topic);
            TypewriterEffect(
                $"You can ask a follow up question on {Part2.LastTopic} or press enter to talk about a different topic.");
            string followup = Console.ReadLine();
            ProcessFollowUp(followup);
        }


        //method to check if user input contains specific keywords
        public static bool ContainsKeywords(string input, string keyCategory)
        {
            // For exit commands, be more precise to avoid false matches
            if (keyCategory == "exit")
            {
                // Use word boundaries to avoid matching "know" as "no"
                string[] exitWords = { "exit", "quit", "bye", "end", "goodbye", "close", "leave", "stop" };
                string[] inputWords = input.ToLower().Split(new char[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
                
                return exitWords.Any(exitWord => inputWords.Contains(exitWord));
            }

            // For other categories, check if the exact key category is contained
            if (input.ToLower().Contains(keyCategory.ToLower()))
                return true;

            // Check all related keywords with word boundaries for better precision
            if (KeywordResponses.TryGetValue(keyCategory, out var response))
            {
                string[] inputWords = input.ToLower().Split(new char[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
                return response.Any(keyword => inputWords.Contains(keyword.ToLower()));
            }

            return false;
        }

        // Information on malware topics
        private static void ShowMalwareInfo()
        {
            DrawTitledBorder("Malware Protection");
            SetConsoleColor(MainColour);
            TypewriterEffect("Loading malware information...");
            ResetConsoleColor();
            DrawProgressBar(100);

            DrawTitledBorder("Malware Information");
            SetConsoleColor(MainColour);
            TypewriterEffect(
                "Malware (malicious software) includes viruses, worms, trojans, ransomware, and spyware designed to damage or gain unauthorized access to systems.");
            Console.WriteLine();
            TypewriterEffect(
                "To protect yourself: keep software updated, use reputable antivirus programs, don't click suspicious links, and be cautious with downloaded files.");
            ResetConsoleColor();
            DrawBorder();
        }

        // Information on social engineering topic
        private static void ShowSocialEngineeringInfo()
        {
            DrawTitledBorder("Social Engineering");
            SetConsoleColor(MainColour);
            TypewriterEffect("Loading social engineering information...");
            ResetConsoleColor();
            DrawProgressBar(100);

            DrawTitledBorder("Social Engineering Information");
            SetConsoleColor(MainColour);
            TypewriterEffect(
                "Social engineering attacks manipulate people into breaking security procedures or revealing confidential information.");
            Console.WriteLine();
            TypewriterEffect(
                "Common tactics include pretexting (creating a fabricated scenario), baiting, quid pro quo offers, and tailgating (following someone into a restricted area).");
            Console.WriteLine();
            TypewriterEffect(
                "Always verify identities before sharing sensitive information and be skeptical of unusual requests or too-good-to-be-true offers.");
            ResetConsoleColor();
            DrawBorder();
        }

        // Information on two-factor authentication topic
        private static void Show2FaInfo()
        {
            DrawTitledBorder("Two-Factor Authentication");
            SetConsoleColor(MainColour);
            TypewriterEffect("Loading 2FA information...");
            ResetConsoleColor();
            DrawProgressBar(100);

            DrawTitledBorder("Two-Factor Authentication Information");
            SetConsoleColor(MainColour);
            TypewriterEffect("Two-factor authentication (2FA) adds an extra layer of security beyond just a password.");
            Console.WriteLine();
            TypewriterEffect(
                "It requires two of the following: something you know (password), something you have (phone), or something you are (fingerprint).");
            Console.WriteLine();
            TypewriterEffect(
                "Enable 2FA on all important accounts, especially email, banking, and social media accounts for significantly improved security.");
            ResetConsoleColor();
            DrawBorder();
        }

        // Information on VPN topic
        private static void ShowVpnInfo()
        {
            DrawTitledBorder("VPN Security");
            SetConsoleColor(MainColour);
            TypewriterEffect("Loading VPN information...");
            ResetConsoleColor();
            DrawProgressBar(100);

            DrawTitledBorder("VPN Information");
            SetConsoleColor(MainColour);
            TypewriterEffect(
                "A Virtual Private Network (VPN) encrypts your internet connection and hides your online activities and location.");
            Console.WriteLine();
            TypewriterEffect(
                "VPNs are especially important when using public Wi-Fi networks, as they protect your data from potential eavesdroppers on the network.");
            Console.WriteLine();
            TypewriterEffect(
                "Choose reputable VPN providers that don't log your activities and offer strong encryption protocols.");
            ResetConsoleColor();
            DrawBorder();
        }

        // Information on data breaches topics
        private static void ShowDataBreachInfo()
        {
            DrawTitledBorder("Data Breaches");
            SetConsoleColor(MainColour);
            TypewriterEffect("Loading data breach information...");
            ResetConsoleColor();
            DrawProgressBar(100);

            DrawTitledBorder("Data Breach Information");
            SetConsoleColor(MainColour);
            TypewriterEffect(
                "A data breach occurs when unauthorized individuals gain access to sensitive, protected, or confidential data.");
            Console.WriteLine();
            TypewriterEffect(
                "If your data is involved in a breach: change passwords immediately, monitor accounts for suspicious activity, consider credit freezes, and be alert for phishing attempts.");
            Console.WriteLine();
            TypewriterEffect(
                "Use services like HaveIBeenPwned.com to check if your email has been involved in known data breaches.");
            ResetConsoleColor();
            DrawBorder();
        }

        // Information on firewalls topic
        private static void ShowFirewallInfo()
        {
            DrawTitledBorder("Firewall Protection");
            SetConsoleColor(MainColour);
            TypewriterEffect("Loading firewall information...");
            ResetConsoleColor();
            DrawProgressBar(100);

            DrawTitledBorder("Firewall Information");
            SetConsoleColor(MainColour);
            TypewriterEffect(
                "A firewall is a network security device that monitors and filters incoming and outgoing network traffic based on security policies.");
            Console.WriteLine();
            TypewriterEffect(
                "Personal firewalls protect individual devices, while network firewalls protect entire networks of computers and devices.");
            Console.WriteLine();
            TypewriterEffect(
                "Always keep your firewall enabled and properly configured to block unauthorized access while permitting legitimate communications.");
            ResetConsoleColor();
            DrawBorder();
        }

        // Information on encryption topic
        private static void ShowEncryptionInfo()
        {
            DrawTitledBorder("Encryption");
            SetConsoleColor(MainColour);
            TypewriterEffect("Loading encryption information...");
            ResetConsoleColor();
            DrawProgressBar(100);

            DrawTitledBorder("Encryption Information");
            SetConsoleColor(MainColour);
            TypewriterEffect(
                "Encryption converts data into a coded format that can only be read with the correct decryption key.");
            Console.WriteLine();
            TypewriterEffect(
                "It protects sensitive information during storage (at rest) and when being transmitted (in transit).");
            Console.WriteLine();
            TypewriterEffect(
                "Use encrypted communication apps, enable device encryption, and look for HTTPS in website URLs as it indicates encrypted connections.");
            ResetConsoleColor();
            DrawBorder();
        }

        // Information on Passwords
        private static void ShowPasswordInfo()
        {
            DrawTitledBorder("Password Safety");
            SetConsoleColor(MainColour);
            TypewriterEffect("Loading password safety information...");
            ResetConsoleColor();
            DrawProgressBar(100);

            DrawTitledBorder("Password Information");
            SetConsoleColor(MainColour);
            TypewriterEffect(
                "Password safety involves using secure and robust passwords to protect sensitive data and systems from unauthorized access.");
            Console.WriteLine();
            TypewriterEffect("It includes strategies like creating strong passwords, " +
                             "avoiding weak or repetitive ones, and using tools like password managers to securely store credentials");
            Console.WriteLine();
            ResetConsoleColor();
            SetConsoleColor(YellowHighlightColour);
            TypewriterEffect("Please enter your password: ");
            ResetConsoleColor();
            DrawBorder();

            // Security awareness test - checks if user enters a password
            var password = Console.ReadLine();
            if (!string.IsNullOrEmpty(password))
            {
                DrawTitledBorder("Security Warning");
                SetConsoleColor(ErrorColour);
                TypewriterEffect("If you have entered your password that means it's not safe because " +
                                 "you have given your password to a chatbot and not read the previous information.");
                ResetConsoleColor();
                DrawBorder();
            }
        }

        //information on Phishing
        private static void ShowPhishingInfo()
        {
            DrawTitledBorder("Phishing Awareness");
            SetConsoleColor(MainColour);
            TypewriterEffect("Loading phishing information...");
            ResetConsoleColor();
            DrawProgressBar(100);

            DrawTitledBorder("Phishing Information");
            SetConsoleColor(MainColour);
            TypewriterEffect(
                "Phishing is a cyberattack where attackers impersonate trusted entities to deceive individuals into revealing sensitive information, " +
                "such as login credentials or financial details.");
            Console.WriteLine();
            TypewriterEffect(
                "These attacks often use emails, texts, or fake websites and rely on social engineering techniques to exploit human psychology");
            ResetConsoleColor();
            DrawBorder();
        }

        // Information on Safe Browsing 
        private static void ShowSafeBrowsingInfo()
        {
            DrawTitledBorder("Safe Browsing");
            SetConsoleColor(MainColour);
            TypewriterEffect("Loading safe browsing information...");
            ResetConsoleColor();
            DrawProgressBar(100);

            DrawTitledBorder("Safe Browsing Information");
            SetConsoleColor(MainColour);
            TypewriterEffect("Safe browsing refers to technologies, like Google Safe Browsing, " +
                             "that identify and warn users about malicious websites containing phishing or malware threats.");
            Console.WriteLine();
            TypewriterEffect("It helps protect users by flagging harmful sites before they can cause damage");
            ResetConsoleColor();
            DrawBorder();
        }

        //display welcome message
        private static void Welcome()
        {
            DrawTitledBorder("Welcome");
            SetConsoleColor(YellowHighlightColour);
            TypewriterEffect("Hello! Welcome to the Cybersecurity Awareness Bot");
            Console.WriteLine();
            TypewriterEffect("What should I call you?");
            ResetConsoleColor();
            DrawBorder();

            // Basic input validation for name
            var number = "1,2,3,4,5,6,7,8,9";
            _name = Console.ReadLine();
            UserMemory.StoreName(_name);
            if (string.IsNullOrEmpty(_name) || _name.Any(c => number.Contains(c)))
            {
                DrawTitledBorder("Invalid Name");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine("Names don't contain numbers.");
                Console.ResetColor();
                DrawBorder();
                Welcome();
            }
            else
            {
                DrawTitledBorder("Setting Up");
                SetConsoleColor(MainColour);
                TypewriterEffect($"Glad to meet you {_name}! I'm loading your personalized experience...");
                ResetConsoleColor();
                DrawProgressBar(100);

                DrawTitledBorder("Setup Complete");
                SetConsoleColor(MainColour);
                TypewriterEffect($"Setup complete! I'm ready to help you learn about cybersecurity, {_name}!");
                ResetConsoleColor();
                DrawBorder();
            }
        }

        //function for displaying ASCII art
        private static void ImageDisp()
        {
            DrawTitledBorder("Cybersecurity Bot");
            SetConsoleColor(SecondaryColour);
            Console.WriteLine(
                "       _______\n     _/       \\_\n    / |       | \\\n   /  |__   __|  \\\n  |__/((o| |o))\\__|\n  |      | |      |\n  |\\     |_|     /|\n  | \\           / |\n   \\| /  ___  \\ |/\n    \\ | / _ \\ | /\n     \\_________/\n      _|_____|_\n ____|_________|____\n/                   \\");
            ResetConsoleColor();
            DrawBorder();
        }

        // function for playing audio 
        internal static void PlayAudio()
        {
            //insert .WAV file
            // Load and play the sound file from the application directory
            SoundPlayer play = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + @"Sounds\Welcome.wav");
            play.Play();
        }

        // Method to implement typewriter effect
        //Takes in a string and has a delay 
        protected static void TypewriterEffect(string text, int delay = 20)
        {
            //repeats the loop until reaching the end of the string, printing out each character after a short delay
            foreach (char c in text)
            {
                Console.Write(c); //displays each character in the text string 
                Thread.Sleep(delay); // Controls the speed of typing
            }
        }

        // Method to draw a border
        private static void DrawBorder()
        {
            int width = Console.WindowWidth - 1;
            Console.WriteLine();

            SetConsoleColor(SecondaryColour);
            Console.Write("╔");
            for (int i = 0; i < width - 2; i++)
            {
                Console.Write("═");
            }

            Console.WriteLine("╗");

            Console.Write("╚");
            for (int i = 0; i < width - 2; i++)
            {
                Console.Write("═");
            }

            Console.WriteLine("╝");
            ResetConsoleColor();

            Console.WriteLine();
        }

        // Method to draw a border with a title
        private static void DrawTitledBorder(string title)
        {
            int width = Console.WindowWidth - 1;
            Console.WriteLine();

            SetConsoleColor(SecondaryColour);

            // Top border with title
            Console.Write("╔");

            // Calculate centre position for title
            int titlePosition = (width - title.Length - 2) / 2;

            // Draw first part of top border
            for (int i = 0; i < titlePosition; i++)
            {
                Console.Write("═");
            }

            // Draw title
            SetConsoleColor(YellowHighlightColour);
            Console.Write(" " + title + " ");
            SetConsoleColor(SecondaryColour);

            // Draw remaining part of top border
            for (int i = 0; i < width - titlePosition - title.Length - 4; i++)
            {
                Console.Write("═");
            }

            Console.WriteLine("╗");

            // Bottom border
            Console.Write("╚");
            for (int i = 0; i < width - 2; i++)
            {
                Console.Write("═");
            }

            Console.WriteLine("╝");

            ResetConsoleColor();
            Console.WriteLine();
        }

        // New method to display a progress bar
        //imitates loading
        private static void DrawProgressBar(int totalSteps)
        {
            int width = Console.WindowWidth - 20; // Leave some margin on either side 
            SetConsoleColor(ProgressColour);

            Console.WriteLine();
            Console.Write("  [");

            for (int i = 0; i < totalSteps; i++)
            {
                // Calculate how many steps correspond to the width
                int barPosition = i * width / totalSteps;

                // Clear the line and redraw the progress bar
                Console.SetCursorPosition(3, Console.CursorTop);

                for (var j = 0; j < width; j++)
                {
                    Console.Write(j <= barPosition ? "█" : " ");
                }

                // Calculate and display percentage
                int percentage = (i + 1) * 100 / totalSteps;
                Console.Write($"] {percentage}%");

                Thread.Sleep(10); // Speed of progress bar
            }

            Console.WriteLine("\n");
            ResetConsoleColor();
        }

        // Method to show "thinking" animation
        private static void DrawTypingAnimation(string message, int seconds)
        {
            SetConsoleColor(YellowHighlightColour);
            int originalTop = Console.CursorTop;

            DrawTitledBorder("Processing");
            Console.Write("  " + message);

            for (int i = 0; i < seconds * 4; i++) // 4 iterations per second
            {
                //used AI to learn how to set the cursor position and then used a switch case to delay the output of dots for thinking
                Console.SetCursorPosition(2 + message.Length, originalTop);

                // Cycle through dot patterns
                switch (i % 4)
                {
                    case 0: Console.Write("   "); break;
                    case 1: Console.Write(".  "); break;
                    case 2: Console.Write(".. "); break;
                    case 3: Console.Write("..."); break;
                }

                Thread.Sleep(500);
            }

            Console.WriteLine();
            DrawBorder();
            ResetConsoleColor();
        }

        // Method to set console colour
        private static void SetConsoleColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        // Method to reset console colour
        private static void ResetConsoleColor()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}