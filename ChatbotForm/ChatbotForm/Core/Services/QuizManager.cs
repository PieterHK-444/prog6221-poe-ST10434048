using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatbotForm.Core.Services
{
    public enum QuizQuestionType
    {
        MultipleChoice,
        TrueFalse
    }

    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; } // For MC, 2 for T/F
        public int CorrectAnswer { get; set; } // 0-based index
        public string Explanation { get; set; }
        public QuizQuestionType Type { get; set; }

        public QuizQuestion(string question, List<string> options, int correctAnswer, string explanation, QuizQuestionType type)
        {
            Question = question;
            Options = options;
            CorrectAnswer = correctAnswer;
            Explanation = explanation;
            Type = type;
        }
    }

    public static class QuizManager
    {
        private static readonly List<QuizQuestion> CyberQuiz = new List<QuizQuestion>
        {
            // 1. Password Safety (MC)
            new QuizQuestion(
                "What makes a strong password?",
                new List<string> { "Only lowercase letters", "Your pet's name", "A mix of letters, numbers, and symbols", "Your birthdate" },
                2,
                "A strong password uses a mix of uppercase, lowercase, numbers, and symbols.",
                QuizQuestionType.MultipleChoice
            ),
            // 2. Phishing (MC)
            new QuizQuestion(
                "Which is a sign of a phishing email?",
                new List<string> { "Personalized greeting", "Urgent request for info", "No spelling errors", "Comes from your own email" },
                1,
                "Phishing emails often create urgency to trick you into acting quickly.",
                QuizQuestionType.MultipleChoice
            ),
            // 3. Safe Browsing (T/F)
            new QuizQuestion(
                "You should always click links in emails to check if they're safe. (True/False)",
                new List<string> { "True", "False" },
                1,
                "Never click suspicious links. Always verify the sender first.",
                QuizQuestionType.TrueFalse
            ),
            // 4. Social Engineering (MC)
            new QuizQuestion(
                "What is social engineering?",
                new List<string> { "A type of malware", "Manipulating people to reveal info", "A firewall technique", "A password manager" },
                1,
                "Social engineering tricks people into giving up confidential information.",
                QuizQuestionType.MultipleChoice
            ),
            // 5. Password Safety (T/F)
            new QuizQuestion(
                "It's safe to use the same password for multiple accounts. (True/False)",
                new List<string> { "True", "False" },
                1,
                "Using unique passwords for each account keeps you safer.",
                QuizQuestionType.TrueFalse
            ),
            // 6. General (MC)
            new QuizQuestion(
                "Which of these is NOT a good cybersecurity habit?",
                new List<string> { "Updating software", "Sharing passwords", "Using 2FA", "Backing up data" },
                1,
                "Never share your passwords with anyone.",
                QuizQuestionType.MultipleChoice
            ),
            // 7. Phishing (T/F)
            new QuizQuestion(
                "Phishing can only happen through email. (True/False)",
                new List<string> { "True", "False" },
                1,
                "Phishing can happen via text, phone calls, or social media too!",
                QuizQuestionType.TrueFalse
            ),
            // 8. Safe Browsing (MC)
            new QuizQuestion(
                "What does HTTPS in a website URL mean?",
                new List<string> { "The site is under maintenance", "The site is secure", "The site is fake", "The site is slow" },
                1,
                "HTTPS means your connection is encrypted and more secure.",
                QuizQuestionType.MultipleChoice
            ),
            // 9. Social Engineering (T/F)
            new QuizQuestion(
                "You should always trust someone who says they're from IT support. (True/False)",
                new List<string> { "True", "False" },
                1,
                "Always verify the identity of anyone asking for sensitive info.",
                QuizQuestionType.TrueFalse
            ),
            // 10. General (MC)
            new QuizQuestion(
                "Which is the safest way to store your passwords?",
                new List<string> { "Write them on a sticky note", "Save in a text file", "Use a password manager", "Email them to yourself" },
                2,
                "Password managers securely store and encrypt your passwords.",
                QuizQuestionType.MultipleChoice
            )
        };

        public static List<QuizQuestion> GetQuiz()
        {
            return CyberQuiz;
        }

        public static string GetQuizInstructions()
        {
            return $"Welcome to the Cybersecurity Quiz!\n\n" +
                   "You will be asked 10 questions. Some are multiple-choice, some are true/false.\n" +
                   "Type the number (1, 2, 3, or 4) for MC, or 1 for True, 2 for False.\n" +
                   "Type 'quit' at any time to exit the quiz.\n\n" +
                   "Let's begin!\n";
        }

        public static int CalculateScore(List<int> userAnswers, List<QuizQuestion> questions)
        {
            int correct = 0;
            for (int i = 0; i < userAnswers.Count && i < questions.Count; i++)
            {
                if (userAnswers[i] == questions[i].CorrectAnswer)
                {
                    correct++;
                }
            }
            return correct;
        }

        public static string GetScoreMessage(int score, int totalQuestions)
        {
            double percentage = (double)score / totalQuestions * 100;
            
            return percentage switch
            {
                >= 90 => "Great job! You're a cybersecurity pro!",
                >= 70 => "Good job! You have a solid foundation in cybersecurity.",
                >= 50 => "Not bad! Keep learning to stay safe online.",
                _ => "Keep practicing! Cybersecurity is important for everyone."
            };
        }
    }
} 