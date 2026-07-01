namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface IAuditLogService
    {
        /// <summary>
        /// Record an audit event. All parameters except actorUserId are required.
        /// actorUserId can be null for anonymous actions (e.g. failed logins).
        /// </summary>
        Task LogAsync(
            string actionName,
            string entityName,
            Guid? entityId,
            Guid? actorUserId,
            string? details = null,
            CancellationToken ct = default);
    }
}
