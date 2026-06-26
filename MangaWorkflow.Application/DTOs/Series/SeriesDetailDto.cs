namespace MangaWorkflow.Application.DTOs.Series
{
    public class SeriesDetailDto
    {
        public Guid SeriesId { get; set; }
        public string Title { get; set; } = "";
        public string? AlternativeTitle { get; set; }
        public string? Genre { get; set; }
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public string StatusCode { get; set; } = "";
        public string StatusName { get; set; } = "";
        public Guid MangakaId { get; set; }
        public string MangakaName { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int ChapterCount { get; set; }
        public bool IsDeleted { get; set; }
    }
}
