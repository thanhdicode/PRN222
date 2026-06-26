using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<int> CountUnreadNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<NotificationType?> GetTypeByCodeAsync(string typeCode, CancellationToken ct = default);
        Task AddAsync(Notification notification, CancellationToken ct = default);
        Task<List<Notification>> GetUnreadAsync(Guid userId, CancellationToken ct = default);
        Task<Notification?> GetByIdAsync(Guid notificationId, CancellationToken ct = default);
        Task<List<Notification>> GetAllUnreadAsync(Guid userId, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

