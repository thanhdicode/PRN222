using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MangaWorkflowDbContext _context;
        public UserRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users.CountAsync(cancellationToken);
        }

        public async Task<List<User>> GetAllAsync(string? keyword, string? roleCode, CancellationToken ct = default)
        {
            var query = _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(u =>
                    u.Email.Contains(keyword) ||
                    u.FullName.Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(roleCode))
            {
                query = query.Where(u => u.UserRoles.Any(ur => ur.Role.RoleCode == roleCode));
            }

            return await query.OrderBy(u => u.FullName).ToListAsync(ct);
        }

        public async Task<User?> GetByIdWithRolesAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId, ct);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user, ct);
        }

        public async Task<List<Role>> GetAllRolesAsync(CancellationToken ct = default)
        {
            return await _context.Roles.OrderBy(r => r.RoleName).ToListAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}

