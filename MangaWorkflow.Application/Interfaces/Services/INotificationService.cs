namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<int> CountUnreadNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
