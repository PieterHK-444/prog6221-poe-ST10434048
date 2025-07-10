using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ChatbotForm.Core.Services
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public string Sender { get; set; } // "User" or "Bot"
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string SessionId { get; set; }
        public string Topic { get; set; } // Optional: what topic was being discussed

        public ChatMessage()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.Now;
        }

        public ChatMessage(string sender, string message, string sessionId = null, string topic = null) : this()
        {
            Sender = sender;
            Message = message;
            SessionId = sessionId ?? Guid.NewGuid().ToString();
            Topic = topic;
        }

        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss}] {Sender}: {Message}";
        }
    }

    public static class ChatLogger
    {
        private static List<ChatMessage> _chatLogs = new List<ChatMessage>();
        private static readonly string ChatLogsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chat_logs.json");
        private static string _currentSessionId = Guid.NewGuid().ToString();

        static ChatLogger()
        {
            LoadChatLogs();
        }

        public static void StartNewSession()
        {
            _currentSessionId = Guid.NewGuid().ToString();
        }

        public static void LogUserMessage(string message, string topic = null)
        {
            var chatMessage = new ChatMessage("User", message, _currentSessionId, topic);
            _chatLogs.Add(chatMessage);
            SaveChatLogs();
        }

        public static void LogBotMessage(string message, string topic = null)
        {
            var chatMessage = new ChatMessage("Bot", message, _currentSessionId, topic);
            _chatLogs.Add(chatMessage);
            SaveChatLogs();
        }

        public static List<ChatMessage> GetCurrentSessionLogs()
        {
            return _chatLogs.Where(c => c.SessionId == _currentSessionId)
                           .OrderBy(c => c.Timestamp)
                           .ToList();
        }

        public static List<ChatMessage> GetAllLogs()
        {
            return _chatLogs.OrderByDescending(c => c.Timestamp).ToList();
        }

        public static List<ChatMessage> GetLogsByDate(DateTime date)
        {
            return _chatLogs.Where(c => c.Timestamp.Date == date.Date)
                           .OrderBy(c => c.Timestamp)
                           .ToList();
        }

        public static List<ChatMessage> GetLogsByTopic(string topic)
        {
            return _chatLogs.Where(c => !string.IsNullOrEmpty(c.Topic) && 
                                       c.Topic.Equals(topic, StringComparison.OrdinalIgnoreCase))
                           .OrderBy(c => c.Timestamp)
                           .ToList();
        }

        public static List<ChatMessage> GetLogsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _chatLogs.Where(c => c.Timestamp.Date >= startDate.Date && 
                                       c.Timestamp.Date <= endDate.Date)
                           .OrderBy(c => c.Timestamp)
                           .ToList();
        }

        public static string GetChatSummary()
        {
            var totalMessages = _chatLogs.Count;
            var userMessages = _chatLogs.Count(c => c.Sender == "User");
            var botMessages = _chatLogs.Count(c => c.Sender == "Bot");
            var uniqueSessions = _chatLogs.Select(c => c.SessionId).Distinct().Count();
            var todayMessages = GetLogsByDate(DateTime.Today).Count;

            return $"📊 Chat Summary:\n" +
                   $"• Total Messages: {totalMessages}\n" +
                   $"• User Messages: {userMessages}\n" +
                   $"• Bot Messages: {botMessages}\n" +
                   $"• Total Sessions: {uniqueSessions}\n" +
                   $"• Today's Messages: {todayMessages}";
        }

        public static string FormatChatLog(List<ChatMessage> messages, string title = "Chat Log")
        {
            if (!messages.Any())
            {
                return $"💬 {title}: No messages found.";
            }

            var result = $"💬 {title}:\n\n";
            foreach (var message in messages)
            {
                result += $"{message}\n";
            }
            return result;
        }

        public static void ClearCurrentSession()
        {
            _chatLogs.RemoveAll(c => c.SessionId == _currentSessionId);
            SaveChatLogs();
        }

        public static void ClearAllLogs()
        {
            _chatLogs.Clear();
            SaveChatLogs();
        }

        public static void ExportChatLog(string filePath)
        {
            try
            {
                var json = JsonSerializer.Serialize(_chatLogs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to export chat log: {ex.Message}");
            }
        }

        public static void ImportChatLog(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var importedLogs = JsonSerializer.Deserialize<List<ChatMessage>>(json) ?? new List<ChatMessage>();
                    _chatLogs.AddRange(importedLogs);
                    SaveChatLogs();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to import chat log: {ex.Message}");
            }
        }

        private static void LoadChatLogs()
        {
            try
            {
                if (File.Exists(ChatLogsFilePath))
                {
                    var json = File.ReadAllText(ChatLogsFilePath);
                    _chatLogs = JsonSerializer.Deserialize<List<ChatMessage>>(json) ?? new List<ChatMessage>();
                }
            }
            catch (Exception)
            {
                _chatLogs = new List<ChatMessage>();
            }
        }

        private static void SaveChatLogs()
        {
            try
            {
                var json = JsonSerializer.Serialize(_chatLogs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ChatLogsFilePath, json);
            }
            catch (Exception)
            {
                // Handle save errors silently for now
            }
        }
    }
} 