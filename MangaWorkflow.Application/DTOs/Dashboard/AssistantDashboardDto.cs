using System.Collections.Generic;

namespace MangaWorkflow.Application.DTOs.Dashboard
{
    public class AssistantDashboardDto
    {
        public int ActiveTasksCount { get; set; }
        public int OverdueTasksCount { get; set; }
        public List<TaskSummaryItemDto> RecentTasks { get; set; } = new();
    }
}
