using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MangaWorkflow.Application.DTOs.Pages
{
    public class CreatePageDto
    {
        public Guid ChapterId { get; set; }

        [Required(ErrorMessage = "Page number is required")]
        [Range(1, 9999, ErrorMessage = "Page number must be between 1 and 9999")]
        [Display(Name = "Page Number")]
        public int PageNumber { get; set; }

        [MaxLength(500)]
        [Display(Name = "Image URL (if not uploading)")]
        public string? FileUrl { get; set; }

        [Display(Name = "Upload Image File")]
        public IFormFile? UploadedFile { get; set; }
    }
}
