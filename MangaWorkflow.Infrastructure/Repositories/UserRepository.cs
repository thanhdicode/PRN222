using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;

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
    }
}
