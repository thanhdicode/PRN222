namespace MangaWorkflow.Application.DTOs.Rankings
{
    public class RankingListItemDto
    {
        public Guid RankingRecordId { get; set; }
        public Guid SeriesId { get; set; }
        public string SeriesTitle { get; set; } = "";
        public string IssueNumber { get; set; } = "";
        public int RankPosition { get; set; }
        public int? PreviousRankPosition { get; set; }
        public int VoteCount { get; set; }
        public string Trend { get; set; } = "";
        public string TrendDisplay { get; set; } = "";
        public DateTime CalculatedAt { get; set; }
    }
}
