using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<int> CountUsersAsync(CancellationToken cancellationToken = default);
        Task<List<User>> GetAllAsync(string? keyword, string? roleCode, CancellationToken ct = default);
        Task<User?> GetByIdWithRolesAsync(Guid userId, CancellationToken ct = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task AddAsync(User user, CancellationToken ct = default);
        Task<List<Role>> GetAllRolesAsync(CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
