using System;
using System.ComponentModel.DataAnnotations;

namespace MangaWorkflow.Application.DTOs.Submissions
{
    public class ReviewSubmissionDto
    {
        public Guid SubmissionId { get; set; }
        [Required] public string Decision { get; set; } = ""; // Approved, Rejected, RevisionRequired
        [MaxLength(1000)] public string? Reason { get; set; }
    }
}
