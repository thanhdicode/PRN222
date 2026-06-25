namespace MangaWorkflow.Application.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalUsers { get; set; }
        public int TotalSeries { get; set; }
        public int TotalChapters { get; set; }
        public int TotalTasks { get; set; }
        public int UnreadNotifications { get; set; }
    }
}
