using System;
using System.Collections.Generic;

namespace POE_ST10434048.Part_2
{
    public abstract class SentimentCheck 
    {
        //dictionary for multiple feelings
        private static readonly Dictionary<string, string> SentimentKeywords = new Dictionary<string, string>
        {
            {"worried", "worried"},
            {"concerned", "worried"},
            {"anxious", "worried"},
            {"overwhelmed", "worried"},
            {"nervous", "worried"},
            {"frustrated", "frustrated"},
            {"confused", "frustrated"},
            {"stuck", "frustrated"},
            {"curious", "curious"},
            {"interested", "curious"},
            {"excited", "curious"}
        };

        private static readonly Dictionary<int, string> WorResponse = new Dictionary<int, string>
        {
            {1,"I understand cybersecurity can feel overwhelming. Don't worry—I'm here to help and guide you step by step!"},
            {2,"I understand cybersecurity can feel overwhelming. Don’t worry—I'm here to help and guide you step by step!"},
            {3,"It's completely normal to feel worried about cybersecurity. Let me help you navigate through it with confidence."}
        };
        private static readonly Dictionary<int, string> CurResponse = new Dictionary<int, string>
        {
            {1,"It's great that you’re curious about cybersecurity! I’m here to answer your questions and guide you."},
            {2,"Curiosity is the first step to learning. Feel free to ask anything—I’m here to help you understand better."},
            {3,"I love your curiosity! Let’s explore cybersecurity together and make it easy to grasp."}
        };
        private static readonly Dictionary<int, string> FruResponse = new Dictionary<int, string>
        {
            {1,"I know how frustrating cybersecurity challenges can be. Let’s tackle this together—I’m here to support you."},
            {2,"It’s okay to feel frustrated sometimes. I’ll help simplify things and find the best solution for you."},
            {3,"Cybersecurity can be tricky, but don’t worry—I’m here to make it easier and clearer for you."}
        };
        
        
        //returns the sentiment found in the user input, allows for expansion 
        public static string DetectSentiment(string input)
        {
            foreach (var pair in SentimentKeywords)
            {
                if (input.Contains(pair.Key))
                    return pair.Value;
            }
            return null;
        }
       static Random random = new Random();

        // Returns an empathy message, created a generalised statement that can be used for the 3 different emotions presented
        public static string GetEmpathyMessage(string sentiment)
        {
            switch (sentiment)
            {
                case "worried":
                    // Keys are 1,2,3 inclusive
                    int randomWIndex = random.Next(1, WorResponse.Count + 1); 
                    return WorResponse[randomWIndex];

                case "frustrated":
                    int randomFIndex = random.Next(1, FruResponse.Count + 1);
                    return FruResponse[randomFIndex];

                case "curious":
                    int randomCIndex = random.Next(1, CurResponse.Count + 1);
                    return CurResponse[randomCIndex];

                default:
                    return "I'm here to help with any questions you have!";
            }
        }

    }
}