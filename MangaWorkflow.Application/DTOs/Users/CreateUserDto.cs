using System.ComponentModel.DataAnnotations;

namespace MangaWorkflow.Application.DTOs.Users
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(200)]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [MaxLength(200)]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Display(Name = "Role")]
        public string? RoleCode { get; set; }
    }
}
