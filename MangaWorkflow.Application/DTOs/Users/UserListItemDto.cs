namespace MangaWorkflow.Application.DTOs.Users
{
    public class UserListItemDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = "";
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime? CreatedAt { get; set; }
    }
}
