using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using ChatbotForm.Core.Models;
using POE_ST10434048.Part_2;

namespace ChatbotForm.Core.Services
{
    public static class TaskManager
    {
        private static List<SecurityTask> _tasks = new List<SecurityTask>();
        private static readonly string TasksFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "security_tasks.json");

        static TaskManager()
        {
            LoadTasks();
        }

        public static SecurityTask CreateTask(string taskName, string description, DateTime reminderDate)
        {
            var task = new SecurityTask(taskName, description, reminderDate);
            _tasks.Add(task);
            SaveTasks();
            
            // Log the activity
            ActivityLogger.LogTaskCreated(taskName, UserMemory.UserName ?? "Unknown");
            ActivityLogger.LogReminderSet(taskName, reminderDate, UserMemory.UserName ?? "Unknown");
            
            return task;
        }

        public static SecurityTask CreateTaskFromInput(string input)
        {
            // Parse input like "remind me to change password in 7 days"
            // or "create task: change password, description: update my password, reminder: 7 days"
            
            string taskName = "";
            string description = "";
            DateTime reminderDate = DateTime.Now;

            // Try to extract task name and description
            if (input.Contains("remind me to"))
            {
                int startIndex = input.IndexOf("remind me to") + "remind me to".Length;
                int endIndex = input.IndexOf(" in ");
                if (endIndex > startIndex)
                {
                    taskName = input.Substring(startIndex, endIndex - startIndex).Trim();
                    description = $"Reminder to {taskName}";
                }
                else
                {
                    // No time specified - extract just the task name
                    taskName = input.Substring(startIndex).Trim();
                    description = $"Reminder to {taskName}";
                }
            }
            else if (input.Contains("create task:"))
            {
                // Parse structured input
                var lines = input.Split(',');
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith("create task:"))
                        taskName = trimmed.Substring("create task:".Length).Trim();
                    else if (trimmed.StartsWith("description:"))
                        description = trimmed.Substring("description:".Length).Trim();
                }
            }

            // Extract time information
            reminderDate = ParseTimeInput(input);

            if (string.IsNullOrEmpty(taskName))
            {
                taskName = "Security Task";
            }

            if (string.IsNullOrEmpty(description))
            {
                description = taskName;
            }

            return CreateTask(taskName, description, reminderDate);
        }

        public static bool NeedsTimeSpecification(string input)
        {
            // Check if the input contains time-related keywords
            string inputLower = input.ToLower();
            
            if (inputLower.Contains("remind me to"))
            {
                // If it contains "remind me to" but no time specification
                return !inputLower.Contains(" in ") && 
                       !inputLower.Contains(" tomorrow") && 
                       !inputLower.Contains(" today") &&
                       !inputLower.Contains(" next week");
            }
            
            if (inputLower.Contains("create task:"))
            {
                // Check if structured input has reminder specification
                return !inputLower.Contains("reminder:");
            }
            
            return false;
        }

        public static string ExtractTaskNameFromInput(string input)
        {
            string taskName = "";
            
            if (input.ToLower().Contains("remind me to"))
            {
                int startIndex = input.ToLower().IndexOf("remind me to") + "remind me to".Length;
                string after = input.Substring(startIndex).Trim();
                // Remove any trailing time keywords if present
                int inIndex = after.ToLower().IndexOf(" in ");
                if (inIndex >= 0)
                {
                    taskName = after.Substring(0, inIndex).Trim();
                }
                else
                {
                    taskName = after.Trim();
                }
            }
            else if (input.ToLower().Contains("create task:"))
            {
                var lines = input.Split(',');
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (trimmed.ToLower().StartsWith("create task:"))
                    {
                        taskName = trimmed.Substring("create task:".Length).Trim();
                        break;
                    }
                }
            }
            
            return taskName;
        }

        public static SecurityTask CreateTaskWithTime(string taskName, string timeInput)
        {
            string description = $"Reminder to {taskName}";
            DateTime reminderDate = ParseTimeInput($"remind me to {taskName} {timeInput}");
            
            return CreateTask(taskName, description, reminderDate);
        }

        public static DateTime ParseTimeInput(string input)
        {
            var now = DateTime.Now;
            
            // Check for specific time 
            if (input.Contains("in ") && input.Contains(" days"))
            {
                var match = Regex.Match(input, @"in (\d+) days?");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int days))
                {
                    return now.AddDays(days);
                }
            }
            
            if (input.Contains("in ") && input.Contains(" hours"))
            {
                var match = Regex.Match(input, @"in (\d+) hours?");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int hours))
                {
                    return now.AddHours(hours);
                }
            }
            
            if (input.Contains("in ") && input.Contains(" minutes"))
            {
                var match = Regex.Match(input, @"in (\d+) minutes?");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int minutes))
                {
                    return now.AddMinutes(minutes);
                }
            }
            
            if (input.Contains("in ") && input.Contains(" weeks"))
            {
                var match = Regex.Match(input, @"in (\d+) weeks?");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int weeks))
                {
                    return now.AddDays(weeks * 7);
                }
            }

            // Default to 1 day from now
            return now.AddDays(1);
        }

        public static List<SecurityTask> GetAllTasks()
        {
            return _tasks.OrderBy(t => t.ReminderDate).ToList();
        }

        public static List<SecurityTask> GetPendingTasks()
        {
            return _tasks.Where(t => !t.IsCompleted).OrderBy(t => t.ReminderDate).ToList();
        }

        public static List<SecurityTask> GetOverdueTasks()
        {
            return _tasks.Where(t => t.IsOverdue()).OrderBy(t => t.ReminderDate).ToList();
        }

        public static List<SecurityTask> GetDueSoonTasks()
        {
            return _tasks.Where(t => t.IsDueSoon()).OrderBy(t => t.ReminderDate).ToList();
        }

        public static SecurityTask GetTaskById(Guid id)
        {
            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        public static SecurityTask GetTaskByName(string taskName)
        {
            return _tasks.FirstOrDefault(t => 
                t.TaskName.Equals(taskName, StringComparison.OrdinalIgnoreCase));
        }

        public static bool MarkTaskAsCompleted(Guid taskId)
        {
            var task = GetTaskById(taskId);
            if (task != null && !task.IsCompleted)
            {
                task.MarkAsCompleted();
                SaveTasks();
                
                // Log the activity
                ActivityLogger.LogTaskCompleted(task.TaskName, UserMemory.UserName ?? "Unknown");
                
                return true;
            }
            return false;
        }

        public static bool MarkTaskAsCompleted(string taskName)
        {
            var task = GetTaskByName(taskName);
            if (task != null && !task.IsCompleted)
            {
                task.MarkAsCompleted();
                SaveTasks();
                
                // Log the activity
                ActivityLogger.LogTaskCompleted(task.TaskName, UserMemory.UserName ?? "Unknown");
                
                return true;
            }
            return false;
        }

        public static bool DeleteTask(Guid taskId)
        {
            var task = GetTaskById(taskId);
            if (task != null)
            {
                string taskName = task.TaskName; // Store before removal
                _tasks.Remove(task);
                SaveTasks();
                
                // Log the activity
                ActivityLogger.LogTaskDeleted(taskName, UserMemory.UserName ?? "Unknown");
                
                return true;
            }
            return false;
        }

        public static bool DeleteTask(string taskName)
        {
            var task = _tasks.FirstOrDefault(t => t.TaskName.Equals(taskName, StringComparison.OrdinalIgnoreCase));
            if (task != null)
            {
                _tasks.Remove(task);
                SaveTasks();
                
                // Log the activity
                ActivityLogger.LogTaskDeleted(taskName, UserMemory.UserName ?? "Unknown");
                return true;
            }
            return false;
        }

        public static bool RemoveAllTasks()
        {
            if (_tasks.Count == 0)
            {
                return false; // No tasks to remove
            }

            int taskCount = _tasks.Count;
            _tasks.Clear();
            SaveTasks();
            
            // Log the activity
            ActivityLogger.LogAllTasksRemoved(taskCount, UserMemory.UserName ?? "Unknown");
            return true;
        }

        public static string GetTasksSummary()
        {
            var total = _tasks.Count;
            var completed = _tasks.Count(t => t.IsCompleted);
            var pending = total - completed;
            var overdue = GetOverdueTasks().Count;
            var dueSoon = GetDueSoonTasks().Count;

            return $"📊 Task Summary:\n" +
                   $"• Total Tasks: {total}\n" +
                   $"• Completed: {completed}\n" +
                   $"• Pending: {pending}\n" +
                   $"• Overdue: {overdue}\n" +
                   $"• Due Soon: {dueSoon}";
        }

        public static string FormatTaskList(List<SecurityTask> tasks, string title = "Tasks")
        {
            if (!tasks.Any())
            {
                return $"📋 {title}: No tasks found.";
            }

            var result = $"📋 {title}:\n\n";
            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                result += $"{i + 1}. {task}\n\n";
            }
            return result;
        }

        public static void CheckAndNotifyOverdueTasks()
        {
            var overdueTasks = GetOverdueTasks();
            if (overdueTasks.Any())
            {
                // This could be integrated with the UI to show notifications
                Console.WriteLine("⚠️ You have overdue security tasks!");
                foreach (var task in overdueTasks)
                {
                    Console.WriteLine($"- {task.TaskName}: {task.Description}");
                }
            }
        }

        private static void LoadTasks()
        {
            try
            {
                if (File.Exists(TasksFilePath))
                {
                    var json = File.ReadAllText(TasksFilePath);
                    _tasks = JsonSerializer.Deserialize<List<SecurityTask>>(json) ?? new List<SecurityTask>();
                }
            }
            catch (Exception)
            {
                _tasks = new List<SecurityTask>();
            }
        }

        private static void SaveTasks()
        {
            try
            {
                var json = JsonSerializer.Serialize(_tasks, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(TasksFilePath, json);
            }
            catch (Exception)
            {
                // Handle save errors silently for now
            }
        }
    }
} 