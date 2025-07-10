using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChatbotForm.Core.Services
{
    public class NLPProcessor
    {
        // Intent categories for the chatbot
        public enum Intent
        {
            Unknown,
            CreateTask,
            ViewTasks,
            CompleteTask,
            DeleteTask,
            TaskSummary,
            StartQuiz,
            ViewChatLogs,
            ViewActivityLog,
            GetTips,
            DiscussTopic,
            Exit,
            Help
        }

        // Dictionary of intent patterns with variations
        private static readonly Dictionary<Intent, List<string>> IntentPatterns = new Dictionary<Intent, List<string>>
        {
            [Intent.CreateTask] = new List<string>
            {
                "remind me to", "create task", "add task", "set reminder", "schedule task",
                "need to", "should", "want to", "have to", "must", "need a reminder",
                "add a reminder", "set up", "create reminder", "add reminder"
            },
            
            [Intent.ViewTasks] = new List<string>
            {
                "show tasks", "view tasks", "list tasks", "my tasks", "see tasks",
                "what tasks", "display tasks", "show my tasks", "task list",
                "overdue tasks", "pending tasks", "active tasks"
            },
            
            [Intent.CompleteTask] = new List<string>
            {
                "complete task", "mark complete", "done", "finished", "completed",
                "task done", "mark as done", "finish task", "complete", "mark done"
            },
            
            [Intent.DeleteTask] = new List<string>
            {
                "delete task", "remove task", "cancel task", "drop task",
                "remove reminder", "delete reminder", "cancel reminder"
            },
            
            [Intent.TaskSummary] = new List<string>
            {
                "task summary", "tasks summary", "summary", "task stats",
                "task statistics", "task overview", "task report"
            },
            
            [Intent.StartQuiz] = new List<string>
            {
                "start quiz", "begin quiz", "take quiz", "quiz", "test me",
                "cybersecurity quiz", "security quiz", "start test", "begin test"
            },
            
            [Intent.ViewChatLogs] = new List<string>
            {
                "show chat logs", "view chat logs", "chat history", "conversation history",
                "chat logs", "message history", "show messages", "view messages"
            },
            
            [Intent.ViewActivityLog] = new List<string>
            {
                "show activity log", "view activity log", "activity history", "activities",
                "what have you done", "my activity", "activity log", "show activities"
            },
            
            [Intent.GetTips] = new List<string>
            {
                "tips", "tip", "advice", "suggestions", "recommendations",
                "best practices", "how to", "guidance", "help with"
            },
            
            [Intent.DiscussTopic] = new List<string>
            {
                "password", "phishing", "malware", "vpn", "firewall", "encryption",
                "two factor", "2fa", "social engineering", "data breach", "safe browsing"
            },
            
            [Intent.Exit] = new List<string>
            {
                "exit", "quit", "bye", "goodbye", "close", "end", "stop", "leave"
            },
            
            [Intent.Help] = new List<string>
            {
                "help", "what can you do", "commands", "options", "menu",
                "how to use", "what do you do", "capabilities"
            }
        };

        // Cybersecurity keywords for topic detection
        private static readonly Dictionary<string, List<string>> TopicKeywords = new Dictionary<string, List<string>>
        {
            ["password"] = new List<string> { "password", "passcode", "credential", "login", "strong password", "weak password" },
            ["phishing"] = new List<string> { "phishing", "scam", "email scam", "fake email", "pretend", "spam", "suspicious email" },
            ["malware"] = new List<string> { "malware", "virus", "spyware", "adware", "ransomware", "computer virus", "trojan" },
            ["vpn"] = new List<string> { "vpn", "virtual private network", "secure connection", "hide ip", "encrypt traffic", "proxy" },
            ["firewall"] = new List<string> { "firewall", "network protection", "blocking", "filter", "security barrier", "network shield" },
            ["encryption"] = new List<string> { "encryption", "encrypt", "cipher", "scramble", "cryptography", "secure data" },
            ["two factor"] = new List<string> { "two factor", "2fa", "mfa", "multi factor", "authentication", "verify identity" },
            ["social engineering"] = new List<string> { "social engineering", "manipulation", "psychological", "human hacking", "pretexting" },
            ["data breach"] = new List<string> { "data breach", "leak", "stolen data", "hack", "exposed information", "compromised data" },
            ["safe browsing"] = new List<string> { "safe browsing", "browser safety", "surfing", "secure browsing", "web safety", "safe websites" }
        };

        /// <summary>
        /// Detects the intent of user input using keyword matching
        /// </summary>
        public static Intent DetectIntent(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Intent.Unknown;

            string inputLower = input.ToLower().Trim();
            string[] inputWords = inputLower.Split(new char[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            // Check each intent pattern with word boundary matching for better precision
            foreach (var intent in IntentPatterns)
            {
                // For exit commands, be extra careful to avoid false matches
                if (intent.Key == Intent.Exit)
                {
                    string[] exitWords = { "exit", "quit", "bye", "end", "goodbye", "close", "leave", "stop" };
                    if (exitWords.Any(exitWord => inputWords.Contains(exitWord)))
                    {
                        return intent.Key;
                    }
                }
                else
                {
                    // For other intents, check if any pattern matches
                    if (intent.Value.Any(pattern => inputLower.Contains(pattern)))
                    {
                        return intent.Key;
                    }
                }
            }

            return Intent.Unknown;
        }

        /// <summary>
        /// Extracts task name from user input for task creation
        /// </summary>
        public static string ExtractTaskName(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            string inputLower = input.ToLower();
            
            // Common patterns for task creation
            string[] patterns = {
                "remind me to (.*)",
                "create task (.*)",
                "add task (.*)",
                "set reminder (.*)",
                "schedule task (.*)",
                "need to (.*)",
                "should (.*)",
                "want to (.*)",
                "have to (.*)",
                "must (.*)",
                "need a reminder (.*)",
                "add a reminder (.*)",
                "set up (.*)",
                "create reminder (.*)",
                "add reminder (.*)"
            };

            foreach (string pattern in patterns)
            {
                var match = Regex.Match(inputLower, pattern);
                if (match.Success && match.Groups.Count > 1)
                {
                    string taskName = match.Groups[1].Value.Trim();
                    // Clean up the task name
                    taskName = CleanTaskName(taskName);
                    return string.IsNullOrWhiteSpace(taskName) ? null : taskName;
                }
            }

            // Fallback: try to extract after common trigger words
            string[] triggers = { "remind me to", "create task", "add task", "set reminder", "need to", "should", "want to", "have to", "must" };
            foreach (string trigger in triggers)
            {
                int index = inputLower.IndexOf(trigger);
                if (index >= 0)
                {
                    string afterTrigger = input.Substring(index + trigger.Length).Trim();
                    if (!string.IsNullOrWhiteSpace(afterTrigger))
                    {
                        string taskName = CleanTaskName(afterTrigger);
                        return string.IsNullOrWhiteSpace(taskName) ? null : taskName;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Extracts topic from user input for tips or discussions
        /// </summary>
        public static string ExtractTopic(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            string inputLower = input.ToLower();

            // Check each topic and its keywords
            foreach (var topic in TopicKeywords)
            {
                if (topic.Value.Any(keyword => inputLower.Contains(keyword)))
                {
                    return topic.Key;
                }
            }

            return null;
        }

        /// <summary>
        /// Extracts time information from user input
        /// </summary>
        public static string ExtractTimeInfo(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            string inputLower = input.ToLower();

            // Common time patterns
            string[] timePatterns = {
                @"(\d{1,2}:\d{2}\s*(?:am|pm)?)", // 3:30, 3:30pm
                @"(\d{1,2}\s*(?:am|pm))", // 3pm, 3 am
                @"(tomorrow)", // tomorrow
                @"(today)", // today
                @"(next week)", // next week
                @"(in \d+\s*(?:hours?|days?|weeks?))", // in 2 hours, in 3 days
                @"(\d+\s*(?:hours?|days?|weeks?)\s*from now)", // 2 hours from now
                @"(morning)", // morning
                @"(afternoon)", // afternoon
                @"(evening)", // evening
                @"(night)", // night
                @"(tonight)", // tonight
                @"(this evening)", // this evening
                @"(this afternoon)", // this afternoon
                @"(this morning)" // this morning
            };

            foreach (string pattern in timePatterns)
            {
                var match = Regex.Match(inputLower, pattern);
                if (match.Success && match.Groups.Count > 1)
                {
                    return match.Groups[1].Value.Trim();
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if input contains any cybersecurity-related keywords
        /// </summary>
        public static bool ContainsCybersecurityKeywords(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string inputLower = input.ToLower();

            // Check all topic keywords
            foreach (var topic in TopicKeywords.Values)
            {
                if (topic.Any(keyword => inputLower.Contains(keyword)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Cleans up task name by removing common endings and punctuation
        /// </summary>
        private static string CleanTaskName(string taskName)
        {
            if (string.IsNullOrWhiteSpace(taskName))
                return null;

            // Remove common endings that might be added
            string[] endingsToRemove = {
                " please", " thanks", " thank you", " now", " later", " soon",
                " asap", " immediately", " right away", " today", " tomorrow"
            };

            string cleaned = taskName;
            foreach (string ending in endingsToRemove)
            {
                if (cleaned.EndsWith(ending))
                {
                    cleaned = cleaned.Substring(0, cleaned.Length - ending.Length);
                }
            }

            // Remove trailing punctuation
            cleaned = cleaned.TrimEnd('.', '!', '?', ',', ';', ':');

            return cleaned.Trim();
        }

        /// <summary>
        /// Gets confidence score for intent detection (simple implementation)
        /// </summary>
        public static double GetIntentConfidence(string input, Intent intent)
        {
            if (string.IsNullOrWhiteSpace(input) || intent == Intent.Unknown)
                return 0.0;

            string inputLower = input.ToLower();
            var patterns = IntentPatterns[intent];
            
            int matches = patterns.Count(pattern => inputLower.Contains(pattern));
            return matches > 0 ? (double)matches / patterns.Count : 0.0;
        }

        /// <summary>
        /// Checks if input is a question (ends with question mark or contains question words)
        /// </summary>
        public static bool IsQuestion(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string inputLower = input.ToLower().Trim();
            
            // Ends with question mark
            if (inputLower.EndsWith("?"))
                return true;

            // Contains question words
            string[] questionWords = { "what", "when", "where", "who", "why", "how", "which", "can you", "could you", "would you", "do you", "does", "is", "are", "was", "were" };
            return questionWords.Any(word => inputLower.StartsWith(word + " ") || inputLower.Contains(" " + word + " "));
        }

        /// <summary>
        /// Normalizes user input for better matching
        /// </summary>
        public static string NormalizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            // Convert to lowercase
            string normalized = input.ToLower();
            
            // Remove extra whitespace
            normalized = Regex.Replace(normalized, @"\s+", " ");
            
            // Remove common filler words
            string[] fillerWords = { "um", "uh", "like", "you know", "i mean", "basically", "actually" };
            foreach (string filler in fillerWords)
            {
                normalized = normalized.Replace(" " + filler + " ", " ");
            }

            return normalized.Trim();
        }
    }
} 