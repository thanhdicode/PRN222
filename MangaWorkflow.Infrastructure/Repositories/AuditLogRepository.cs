using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;
using MangaWorkflow.Infrastructure.Persistence;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly MangaWorkflowDbContext _context;

        public AuditLogRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AuditLog log, CancellationToken ct = default)
        {
            await _context.AuditLogs.AddAsync(log, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<AuditLog>> GetByUserAsync(Guid userId, int take = 50, CancellationToken ct = default)
        {
            return await _context.AuditLogs
                .Where(a => a.ActorUserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .Take(take)
                .ToListAsync(ct);
        }

        public async Task<List<AuditLog>> GetByEntityAsync(string entityName, Guid entityId, CancellationToken ct = default)
        {
            return await _context.AuditLogs
                .Where(a => a.EntityName == entityName && a.EntityId == entityId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(ct);
        }
    }
}
