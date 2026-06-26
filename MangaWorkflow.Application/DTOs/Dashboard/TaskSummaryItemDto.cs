using System;

namespace MangaWorkflow.Application.DTOs.Dashboard
{
    public class TaskSummaryItemDto
    {
        public Guid TaskId { get; set; }
        public string Description { get; set; } = null!;
        public string StatusCode { get; set; } = null!;
        public DateTime Deadline { get; set; }
    }
}
