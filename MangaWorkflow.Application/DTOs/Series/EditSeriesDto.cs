using System.ComponentModel.DataAnnotations;

namespace MangaWorkflow.Application.DTOs.Series
{
    public class EditSeriesDto
    {
        public Guid SeriesId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(300, ErrorMessage = "Title cannot exceed 300 characters")]
        [Display(Name = "Series Title")]
        public string Title { get; set; } = "";

        [MaxLength(100, ErrorMessage = "Genre cannot exceed 100 characters")]
        public string? Genre { get; set; }

        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        [MaxLength(500)]
        [Display(Name = "Cover Image URL")]
        public string? CoverImageUrl { get; set; }
    }
}
