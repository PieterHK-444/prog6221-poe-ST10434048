using System;
using System.Text.Json.Serialization;

namespace ChatbotForm.Core.Models
{
    public class SecurityTask
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("taskName")]
        public string TaskName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonPropertyName("reminderDate")]
        public DateTime ReminderDate { get; set; }

        [JsonPropertyName("isCompleted")]
        public bool IsCompleted { get; set; }

        [JsonPropertyName("completedDate")]
        public DateTime? CompletedDate { get; set; }

        public SecurityTask()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            IsCompleted = false;
        }

        public SecurityTask(string taskName, string description, DateTime reminderDate) : this()
        {
            TaskName = taskName;
            Description = description;
            ReminderDate = reminderDate;
        }

        public void MarkAsCompleted()
        {
            IsCompleted = true;
            CompletedDate = DateTime.Now;
        }

        public bool IsOverdue()
        {
            return !IsCompleted && ReminderDate < DateTime.Now;
        }

        public bool IsDueSoon()
        {
            return !IsCompleted && ReminderDate <= DateTime.Now.AddDays(1) && ReminderDate > DateTime.Now;
        }

        public override string ToString()
        {
            string status = IsCompleted ? "✅ Completed" : (IsOverdue() ? "⚠️ Overdue" : "⏰ Pending");
            return $"{TaskName} - {Description} ({status})";
        }
    }
} 