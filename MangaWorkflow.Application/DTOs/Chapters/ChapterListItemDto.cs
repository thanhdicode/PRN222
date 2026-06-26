namespace MangaWorkflow.Application.DTOs.Chapters
{
    public class ChapterListItemDto
    {
        public Guid ChapterId { get; set; }
        public Guid SeriesId { get; set; }
        public string SeriesTitle { get; set; } = "";
        public int ChapterNumber { get; set; }
        public string Title { get; set; } = "";
        public string StatusCode { get; set; } = "";
        public string StatusName { get; set; } = "";
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PageCount { get; set; }
    }
}
