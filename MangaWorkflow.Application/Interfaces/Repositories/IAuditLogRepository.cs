using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface IAuditLogRepository
    {
        /// <summary>Persist a new audit log entry.</summary>
        Task AddAsync(AuditLog log, CancellationToken ct = default);

        /// <summary>Flush pending changes to the database.</summary>
        Task SaveChangesAsync(CancellationToken ct = default);

        /// <summary>Return all audit log entries for a given user, newest first.</summary>
        Task<List<AuditLog>> GetByUserAsync(Guid userId, int take = 50, CancellationToken ct = default);

        /// <summary>Return recent audit log entries for a given entity (e.g. "Series", seriesId).</summary>
        Task<List<AuditLog>> GetByEntityAsync(string entityName, Guid entityId, CancellationToken ct = default);
    }
}
