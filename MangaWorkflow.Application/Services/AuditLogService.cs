using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repo;

        public AuditLogService(IAuditLogRepository repo)
        {
            _repo = repo;
        }

        public async Task LogAsync(
            string actionName,
            string entityName,
            Guid? entityId,
            Guid? actorUserId,
            string? details = null,
            CancellationToken ct = default)
        {
            var entry = new AuditLog
            {
                AuditLogId  = Guid.NewGuid(),
                ActionName  = actionName,
                EntityName  = entityName,
                EntityId    = entityId,
                ActorUserId = actorUserId,
                Details     = details,
                CreatedAt   = DateTime.UtcNow
            };

            await _repo.AddAsync(entry, ct);
            await _repo.SaveChangesAsync(ct);
        }
    }
}
