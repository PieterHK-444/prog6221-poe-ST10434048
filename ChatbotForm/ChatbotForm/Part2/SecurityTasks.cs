using System;

namespace ChatbotForm.Part2
{
    public class SecurityTask
    {
        public Guid Id { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; }
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

        public string GetStatusText()
        {
            if (IsCompleted)
                return "✅ Completed";
            else if (IsOverdue())
                return "⚠️ Overdue";
            else if (IsDueSoon())
                return "🔔 Due Soon";
            else
                return "📅 Pending";
        }

        public override string ToString()
        {
            string status = GetStatusText();
            string reminderText = ReminderDate.ToString("MMM dd, yyyy 'at' HH:mm");
            
            return $"[{status}] {TaskName} - {Description} (Reminder: {reminderText})";
        }
    }
} 