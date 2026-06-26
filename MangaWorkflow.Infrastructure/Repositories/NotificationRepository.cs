using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;

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

        public async Task<NotificationType?> GetTypeByCodeAsync(string typeCode, CancellationToken ct = default)
        {
            return await _context.NotificationTypes
                .FirstOrDefaultAsync(t => t.TypeCode == typeCode, ct);
        }

        public async Task AddTypeAsync(NotificationType type, CancellationToken ct = default)
        {
            await _context.NotificationTypes.AddAsync(type, ct);
        }

        public async Task AddAsync(Notification notification, CancellationToken ct = default)
        {
            await _context.Notifications.AddAsync(notification, ct);
        }

        public async Task<List<Notification>> GetUnreadAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Notifications
                .Include(n => n.NotificationType)
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<Notification?> GetByIdAsync(Guid notificationId, CancellationToken ct = default)
        {
            return await _context.Notifications.FindAsync(new object[] { notificationId }, ct);
        }

        public async Task<List<Notification>> GetAllUnreadAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}

