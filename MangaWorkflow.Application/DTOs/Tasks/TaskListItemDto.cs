using System;

namespace MangaWorkflow.Application.DTOs.Tasks
{
    public class TaskListItemDto
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; } = "";
        public string TypeCode { get; set; } = "";
        public string TypeName { get; set; } = "";
        public string StatusCode { get; set; } = "";
        public string StatusName { get; set; } = "";
        public DateTime? Deadline { get; set; }
        public bool IsOverdue => Deadline.HasValue && Deadline.Value < DateTime.UtcNow && StatusCode is not ("Approved" or "Rejected" or "Cancelled");
        public string? SeriesTitle { get; set; }
        public string? ChapterTitle { get; set; }
        public int? PageNumber { get; set; }
    }
}
