using System.ComponentModel.DataAnnotations;

namespace MangaWorkflow.Application.DTOs.Chapters
{
    public class CreateChapterDto : IValidatableObject
    {
        public Guid SeriesId { get; set; }

        [Required(ErrorMessage = "Chapter number is required")]
        [Range(1, 9999, ErrorMessage = "Chapter number must be between 1 and 9999")]
        [Display(Name = "Chapter Number")]
        public int ChapterNumber { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(300, ErrorMessage = "Title cannot exceed 300 characters")]
        public string Title { get; set; } = "";

        [MaxLength(2000, ErrorMessage = "Synopsis cannot exceed 2000 characters")]
        public string? Synopsis { get; set; }

        [Display(Name = "Deadline")]
        public DateTime? Deadline { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Deadline.HasValue && Deadline.Value <= DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Deadline must be a future date.",
                    new[] { nameof(Deadline) });
            }
        }
    }
}
