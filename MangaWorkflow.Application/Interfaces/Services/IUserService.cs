using MangaWorkflow.Application.DTOs.Users;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<List<UserListItemDto>> GetUsersAsync(string? keyword, string? roleCode, CancellationToken ct = default);
        Task<UserDetailDto?> GetUserDetailAsync(Guid userId, CancellationToken ct = default);
        Task CreateUserAsync(CreateUserDto dto, CancellationToken ct = default);
        Task UpdateUserAsync(EditUserDto dto, CancellationToken ct = default);
        Task AssignRoleAsync(AssignRoleDto dto, CancellationToken ct = default);
        Task SetUserStatusAsync(Guid userId, bool isActive, CancellationToken ct = default);
        Task<List<Role>> GetRolesAsync(CancellationToken ct = default);
    }
}
