using System.ComponentModel.DataAnnotations;

namespace MangaWorkflow.Application.DTOs.Users
{
    public class EditUserDto
    {
        public Guid UserId { get; set; }

        [MaxLength(200)]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [MaxLength(500)]
        [Display(Name = "Avatar URL")]
        public string? AvatarUrl { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
