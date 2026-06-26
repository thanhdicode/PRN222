using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<int> CountUnreadNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<NotificationType?> GetTypeByCodeAsync(string typeCode, CancellationToken ct = default);
        Task AddAsync(Notification notification, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

