using System;

namespace MangaWorkflow.Application.DTOs.Submissions
{
    public class PendingSubmissionDto
    {
        public Guid SubmissionId { get; set; }
        public Guid TaskId { get; set; }
        public string TaskTitle { get; set; } = "";
        public string AssistantName { get; set; } = "";
        public DateTime SubmittedAt { get; set; }
        public string? FileUrl { get; set; }
    }
}
