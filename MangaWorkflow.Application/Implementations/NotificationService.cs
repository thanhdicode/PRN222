using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<int> CountUnreadNotificationsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _notificationRepository.CountUnreadNotificationsAsync(userId, cancellationToken);
        }
    }
}
