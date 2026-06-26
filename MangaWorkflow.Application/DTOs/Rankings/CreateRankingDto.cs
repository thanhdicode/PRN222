using System.ComponentModel.DataAnnotations;

namespace MangaWorkflow.Application.DTOs.Rankings
{
    public class CreateRankingDto
    {
        [Required(ErrorMessage = "Series is required")]
        public Guid SeriesId { get; set; }

        [Required(ErrorMessage = "Issue number is required")]
        [MaxLength(20, ErrorMessage = "Issue number cannot exceed 20 characters")]
        [Display(Name = "Issue Number")]
        public string IssueNumber { get; set; } = "";

        [Required(ErrorMessage = "Rank position is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Rank position must be at least 1")]
        [Display(Name = "Rank Position")]
        public int RankPosition { get; set; }

        [Required(ErrorMessage = "Vote count is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Vote count cannot be negative")]
        [Display(Name = "Vote Count")]
        public int VoteCount { get; set; }

        [Display(Name = "Trend")]
        public string? TrendCode { get; set; } // Up, Down, Stable, New
    }
}
