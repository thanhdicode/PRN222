using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly MangaWorkflowDbContext _context;
        public NotificationRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountUnreadNotificationsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications.CountAsync(n => n.UserId == userId && n.IsRead == false, cancellationToken);
        }
    }
}
