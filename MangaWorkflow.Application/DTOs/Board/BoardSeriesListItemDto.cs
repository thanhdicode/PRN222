namespace MangaWorkflow.Application.DTOs.Board
{
    public class BoardSeriesListItemDto
    {
        public Guid SeriesId { get; set; }
        public string Title { get; set; } = "";
        public string? Genre { get; set; }
        public string? CoverImageUrl { get; set; }
        public string MangakaName { get; set; } = "";
        public string StatusCode { get; set; } = "";
        public string StatusName { get; set; } = "";
        public DateTime? SubmittedAt { get; set; }
        public int VoteCount { get; set; }
    }
}
