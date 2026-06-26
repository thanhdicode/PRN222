namespace MangaWorkflow.Application.DTOs.Board
{
    public class VoteSummaryDto
    {
        public int ApproveCount { get; set; }
        public int RejectCount { get; set; }
        public int NeedRevisionCount { get; set; }
        public int AbstainCount { get; set; }
        public int TotalVotes { get; set; }
    }
}
