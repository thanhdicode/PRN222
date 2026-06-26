using MangaWorkflow.Application.DTOs.Users;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserListItemDto>> GetUsersAsync(string? keyword, string? roleCode, CancellationToken ct = default)
        {
            var users = await _userRepository.GetAllAsync(keyword, roleCode, ct);
            return users.Select(u => new UserListItemDto
            {
                UserId = u.UserId,
                Email = u.Email,
                FullName = u.FullName,
                AvatarUrl = u.AvatarUrl,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                Roles = u.UserRoles.Select(ur => ur.Role.RoleCode).ToList()
            }).ToList();
        }

        public async Task<UserDetailDto?> GetUserDetailAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId, ct);
            if (user == null) return null;

            return new UserDetailDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Roles = user.UserRoles.Select(ur => ur.Role.RoleCode).ToList()
            };
        }

        public async Task CreateUserAsync(CreateUserDto dto, CancellationToken ct = default)
        {
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = dto.Email,
                FullName = dto.FullName ?? dto.Email,
                // TODO: Replace plain-text with BCrypt hashing for production
                PasswordHash = dto.Password,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user, ct);

            // Assign role if provided
            if (!string.IsNullOrWhiteSpace(dto.RoleCode))
            {
                var roles = await _userRepository.GetAllRolesAsync(ct);
                var role = roles.FirstOrDefault(r => r.RoleCode == dto.RoleCode);
                if (role != null)
                {
                    user.UserRoles.Add(new UserRole
                    {
                        UserId = user.UserId,
                        RoleId = role.RoleId,
                        AssignedAt = DateTime.UtcNow
                    });
                }
            }

            await _userRepository.SaveChangesAsync(ct);
        }

        public async Task UpdateUserAsync(EditUserDto dto, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(dto.UserId, ct);
            if (user == null)
                throw new InvalidOperationException($"User {dto.UserId} not found.");

            user.FullName = dto.FullName ?? user.FullName;
            user.AvatarUrl = dto.AvatarUrl;
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.SaveChangesAsync(ct);
        }

        public async Task AssignRoleAsync(AssignRoleDto dto, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(dto.UserId, ct);
            if (user == null)
                throw new InvalidOperationException($"User {dto.UserId} not found.");

            var roles = await _userRepository.GetAllRolesAsync(ct);
            var role = roles.FirstOrDefault(r => r.RoleCode == dto.RoleCode);
            if (role == null)
                throw new InvalidOperationException($"Role '{dto.RoleCode}' not found.");

            // Avoid duplicate role assignment
            bool alreadyHasRole = user.UserRoles.Any(ur => ur.RoleId == role.RoleId);
            if (!alreadyHasRole)
            {
                user.UserRoles.Add(new UserRole
                {
                    UserId = user.UserId,
                    RoleId = role.RoleId,
                    AssignedAt = DateTime.UtcNow
                });
                await _userRepository.SaveChangesAsync(ct);
            }
        }

        public async Task SetUserStatusAsync(Guid userId, bool isActive, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(userId, ct);
            if (user == null)
                throw new InvalidOperationException($"User {userId} not found.");

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.SaveChangesAsync(ct);
        }

        public async Task<List<Role>> GetRolesAsync(CancellationToken ct = default)
        {
            return await _userRepository.GetAllRolesAsync(ct);
        }
    }
}
