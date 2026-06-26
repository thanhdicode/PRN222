namespace MangaWorkflow.Application.DTOs.Series
{
    public class SeriesListItemDto
    {
        public Guid SeriesId { get; set; }
        public string Title { get; set; } = "";
        public string? Genre { get; set; }
        public string StatusCode { get; set; } = "";
        public string StatusName { get; set; } = "";
        public string? MangakaName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ChapterCount { get; set; }
        public bool IsDeleted { get; set; }
    }
}
