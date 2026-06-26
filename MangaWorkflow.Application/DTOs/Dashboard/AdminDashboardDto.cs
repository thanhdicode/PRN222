namespace MangaWorkflow.Application.DTOs.Dashboard
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalSeries { get; set; }
        public int ActiveTasks { get; set; }
        public int PendingSubmissions { get; set; }
    }
}
