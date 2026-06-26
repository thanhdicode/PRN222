using System.ComponentModel.DataAnnotations;

namespace MangaWorkflow.Application.DTOs.Users
{
    public class AssignRoleDto
    {
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Please select a role")]
        [Display(Name = "Role")]
        public string RoleCode { get; set; } = "";
    }
}
