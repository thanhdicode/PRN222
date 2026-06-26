using System;

namespace MangaWorkflow.Application.DTOs.Tasks
{
    public class TaskDetailDto
    {
        public Guid TaskId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string TypeCode { get; set; } = "";
        public string StatusCode { get; set; } = "";
        public string StatusName { get; set; } = "";
        public DateTime? Deadline { get; set; }
        public bool IsOverdue => Deadline.HasValue && Deadline.Value < DateTime.UtcNow && StatusCode is not ("Approved" or "Rejected" or "Cancelled");
        public Guid PageId { get; set; }
        public string? PageImageUrl { get; set; }
        public string? Instructions { get; set; }
        public Guid? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
    }
}
