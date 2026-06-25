namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task<int> CountUnreadNotificationsAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
