namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<int> CountUnreadNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<MangaWorkflow.Application.DTOs.Notifications.NotificationDto>> GetUnreadAsync(Guid userId, CancellationToken ct = default);
        Task MarkReadAsync(Guid notificationId, Guid currentUserId, CancellationToken ct = default);
        Task MarkAllReadAsync(Guid currentUserId, CancellationToken ct = default);
        Task<MangaWorkflow.Application.DTOs.Notifications.NotificationDto?> CreateAndSendAsync(Guid userId, string typeCode, string title, string message, string? referenceType = null, Guid? referenceId = null, CancellationToken ct = default);
    }
}
