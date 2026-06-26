using System;

namespace MangaWorkflow.Application.DTOs.Dashboard
{
    public class ChapterProgressItemDto
    {
        public string SeriesTitle { get; set; } = null!;
        public string ChapterTitle { get; set; } = null!;
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public DateTime? PublishedDate { get; set; }
    }
}
