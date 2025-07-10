using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ChatbotForm.Core.Services
{
    public class ActivityLogEntry
    {
        public Guid Id { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserName { get; set; }
        public string Category { get; set; } // "Task", "Reminder", "Quiz", "General"

        public ActivityLogEntry()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.Now;
        }

        public ActivityLogEntry(string action, string description, string userName, string category) : this()
        {
            Action = action;
            Description = description;
            UserName = userName;
            Category = category;
        }

        public override string ToString()
        {
            return $"[{Timestamp:MMM dd, HH:mm}] {Action}: {Description}";
        }
    }

    public static class ActivityLogger
    {
        private static List<ActivityLogEntry> _activityLogs = new List<ActivityLogEntry>();
        private static readonly string ActivityLogsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "activity_logs.json");
        private static readonly int MaxDisplayEntries = 10; // Show last 10 actions by default

        static ActivityLogger()
        {
            LoadActivityLogs();
        }

        public static void LogActivity(string action, string description, string userName = null, string category = "General")
        {
            var entry = new ActivityLogEntry(action, description, userName ?? "Unknown", category);
            _activityLogs.Add(entry);
            SaveActivityLogs();
        }

        // Task-related activities
        public static void LogTaskCreated(string taskName, string userName)
        {
            LogActivity("Task Created", $"Created task: {taskName}", userName, "Task");
        }

        public static void LogTaskCompleted(string taskName, string userName)
        {
            LogActivity("Task Completed", $"Marked task as completed: {taskName}", userName, "Task");
        }

        public static void LogTaskDeleted(string taskName, string userName)
        {
            LogActivity("Task Deleted", $"Deleted task: {taskName}", userName, "Task");
        }

        public static void LogAllTasksRemoved(int taskCount, string userName)
        {
            LogActivity("All Tasks Removed", $"Removed all {taskCount} tasks", userName, "Task");
        }

        public static void LogTaskUpdated(string taskName, string userName)
        {
            LogActivity("Task Updated", $"Updated task: {taskName}", userName, "Task");
        }

        // Reminder-related activities
        public static void LogReminderSet(string taskName, DateTime reminderDate, string userName)
        {
            LogActivity("Reminder Set", $"Set reminder for '{taskName}' on {reminderDate:MMM dd, yyyy 'at' HH:mm}", userName, "Reminder");
        }

        public static void LogReminderTriggered(string taskName, string userName)
        {
            LogActivity("Reminder Triggered", $"Reminder triggered for task: {taskName}", userName, "Reminder");
        }

        // Quiz-related activities
        public static void LogQuizStarted(string quizType, string userName)
        {
            LogActivity("Quiz Started", $"Started {quizType} quiz", userName, "Quiz");
        }

        public static void LogQuizCompleted(string quizType, int score, string userName)
        {
            LogActivity("Quiz Completed", $"Completed {quizType} quiz with score: {score}", userName, "Quiz");
        }

        public static void LogQuizAbandoned(string quizType, string userName)
        {
            LogActivity("Quiz Abandoned", $"Abandoned {quizType} quiz", userName, "Quiz");
        }

        // General activities
        public static void LogUserLogin(string userName)
        {
            LogActivity("User Login", $"User '{userName}' started a new session", userName, "General");
        }

        public static void LogTopicDiscussed(string topic, string userName)
        {
            LogActivity("Topic Discussed", $"Discussed cybersecurity topic: {topic}", userName, "General");
        }

        public static void LogTipsRequested(string topic, string userName)
        {
            LogActivity("Tips Requested", $"Requested tips for topic: {topic}", userName, "General");
        }

        public static void LogFavouriteTopicSet(string topic, string userName)
        {
            LogActivity("Favourite Topic Set", $"Set favourite topic to: {topic}", userName, "General");
        }

        // Retrieval methods
        public static List<ActivityLogEntry> GetRecentActivities(int count = 10)
        {
            return _activityLogs.OrderByDescending(a => a.Timestamp).Take(count).ToList();
        }

        public static List<ActivityLogEntry> GetActivitiesByCategory(string category, int count = 10)
        {
            return _activityLogs.Where(a => a.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                               .OrderByDescending(a => a.Timestamp)
                               .Take(count)
                               .ToList();
        }

        public static List<ActivityLogEntry> GetActivitiesByUser(string userName, int count = 10)
        {
            return _activityLogs.Where(a => a.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase))
                               .OrderByDescending(a => a.Timestamp)
                               .Take(count)
                               .ToList();
        }

        public static List<ActivityLogEntry> GetActivitiesByDate(DateTime date, int count = 10)
        {
            return _activityLogs.Where(a => a.Timestamp.Date == date.Date)
                               .OrderByDescending(a => a.Timestamp)
                               .Take(count)
                               .ToList();
        }

        public static List<ActivityLogEntry> GetAllActivities()
        {
            return _activityLogs.OrderByDescending(a => a.Timestamp).ToList();
        }

        // Display methods
        public static string FormatActivityLog(List<ActivityLogEntry> activities, string title = "Activity Log")
        {
            if (!activities.Any())
            {
                return $"📋 {title}: No activities found.";
            }

            var result = $"📋 {title}:\n\n";
            for (int i = 0; i < activities.Count; i++)
            {
                var activity = activities[i];
                result += $"{i + 1}. {activity}\n";
            }
            return result;
        }

        public static string GetActivitySummary()
        {
            var total = _activityLogs.Count;
            var tasks = _activityLogs.Count(a => a.Category == "Task");
            var reminders = _activityLogs.Count(a => a.Category == "Reminder");
            var quizzes = _activityLogs.Count(a => a.Category == "Quiz");
            var general = _activityLogs.Count(a => a.Category == "General");
            var today = GetActivitiesByDate(DateTime.Today).Count;

            return $"📊 Activity Summary:\n" +
                   $"• Total Activities: {total}\n" +
                   $"• Task Activities: {tasks}\n" +
                   $"• Reminder Activities: {reminders}\n" +
                   $"• Quiz Activities: {quizzes}\n" +
                   $"• General Activities: {general}\n" +
                   $"• Today's Activities: {today}";
        }

        // Management methods
        public static void ClearActivityLogs()
        {
            _activityLogs.Clear();
            SaveActivityLogs();
        }

        public static void ClearOldActivities(int daysToKeep = 30)
        {
            var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
            _activityLogs.RemoveAll(a => a.Timestamp < cutoffDate);
            SaveActivityLogs();
        }

        public static void ExportActivityLog(string filePath)
        {
            try
            {
                var json = JsonSerializer.Serialize(_activityLogs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to export activity log: {ex.Message}");
            }
        }

        private static void LoadActivityLogs()
        {
            try
            {
                if (File.Exists(ActivityLogsFilePath))
                {
                    var json = File.ReadAllText(ActivityLogsFilePath);
                    _activityLogs = JsonSerializer.Deserialize<List<ActivityLogEntry>>(json) ?? new List<ActivityLogEntry>();
                }
            }
            catch (Exception)
            {
                _activityLogs = new List<ActivityLogEntry>();
            }
        }

        private static void SaveActivityLogs()
        {
            try
            {
                var json = JsonSerializer.Serialize(_activityLogs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ActivityLogsFilePath, json);
            }
            catch (Exception)
            {
                // Handle save errors silently for now
            }
        }
    }
} 