namespace MangaWorkflow.Application.DTOs.Chapters
{
    public class ChapterDetailDto
    {
        public Guid ChapterId { get; set; }
        public Guid SeriesId { get; set; }
        public string SeriesTitle { get; set; } = "";
        public int ChapterNumber { get; set; }
        public string Title { get; set; } = "";
        public string? Synopsis { get; set; }
        public string StatusCode { get; set; } = "";
        public string StatusName { get; set; } = "";
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int TotalPages { get; set; }
        public int TotalTasks { get; set; }
        public int ApprovedTasks { get; set; }
        public int OverdueTasks { get; set; }
    }
}
