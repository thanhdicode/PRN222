using System.ComponentModel.DataAnnotations;

namespace MangaWorkflow.Application.DTOs.Board
{
    public class SubmitVoteDto
    {
        public Guid SeriesId { get; set; }

        [Required(ErrorMessage = "Please select a vote option")]
        [Display(Name = "Vote")]
        public string VoteValueCode { get; set; } = ""; // Approve, Reject, NeedRevision, Abstain

        [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }
    }
}
