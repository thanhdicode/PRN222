using System;

namespace MangaWorkflow.Application.DTOs.Tasks
{
    public class TaskDeadlineReminderDto
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid AssignedToUserId { get; set; }
        public DateTime Deadline { get; set; }
    }
}
