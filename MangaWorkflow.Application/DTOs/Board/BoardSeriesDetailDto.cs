namespace MangaWorkflow.Application.DTOs.Board
{
    public class VoteSummaryItemDto
    {
        public string BoardMemberName { get; set; } = "";
        public string VoteCode { get; set; } = "";
        public string VoteName { get; set; } = "";
        public string? Comment { get; set; }
        public DateTime VotedAt { get; set; }
    }

    public class BoardSeriesDetailDto
    {
        public Guid SeriesId { get; set; }
        public string Title { get; set; } = "";
        public string? Genre { get; set; }
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public Guid MangakaId { get; set; }
        public string MangakaName { get; set; } = "";
        public string StatusCode { get; set; } = "";
        public string StatusName { get; set; } = "";
        public DateTime? SubmittedAt { get; set; }
        public int ChapterCount { get; set; }
        public List<VoteSummaryItemDto> Votes { get; set; } = new();
        public bool CurrentMemberHasVoted { get; set; }
        public string? CurrentMemberVote { get; set; }
    }
}
