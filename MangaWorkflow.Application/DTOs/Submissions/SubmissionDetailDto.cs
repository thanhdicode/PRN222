using System;

namespace MangaWorkflow.Application.DTOs.Submissions
{
    public class SubmissionDetailDto
    {
        public Guid SubmissionId { get; set; }
        public Guid TaskId { get; set; }
        public string TaskTitle { get; set; } = "";
        public string AssistantName { get; set; } = "";
        public DateTime SubmittedAt { get; set; }
        public string? FileUrl { get; set; }
        public string? Notes { get; set; }
        public string StatusCode { get; set; } = "";
    }
}
