using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IWorkflowHubNotifier _workflowHubNotifier;

        public NotificationService(INotificationRepository notificationRepository, IWorkflowHubNotifier workflowHubNotifier)
        {
            _notificationRepository = notificationRepository;
            _workflowHubNotifier = workflowHubNotifier;
        }

        public async Task<int> CountUnreadNotificationsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _notificationRepository.CountUnreadNotificationsAsync(userId, cancellationToken);
        }

        public async Task<List<MangaWorkflow.Application.DTOs.Notifications.NotificationDto>> GetUnreadAsync(Guid userId, CancellationToken ct = default)
        {
            var notifications = await _notificationRepository.GetUnreadAsync(userId, ct);
            return notifications.Select(n => new MangaWorkflow.Application.DTOs.Notifications.NotificationDto
            {
                NotificationId = n.NotificationId,
                UserId = n.UserId,
                TypeCode = n.NotificationType.TypeCode,
                Title = n.Title,
                Message = n.Message,
                ReferenceType = n.ReferenceType,
                ReferenceId = n.ReferenceId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList();
        }

        private static readonly HashSet<string> ValidNotificationTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "TaskAssigned", "SubmissionUploaded", "SubmissionReviewed", "EditorCommentCreated",
            "BoardVoteSubmitted", "RankingUpdated", "DeadlineWarning", "TaskOverdue", "RankingRisk", "System"
        };

        private async Task<MangaWorkflow.Domain.Entities.NotificationType> GetOrCreateTypeByCodeAsync(string typeCode, CancellationToken ct)
        {
            if (!ValidNotificationTypes.Contains(typeCode))
            {
                typeCode = "System";
            }

            var type = await _notificationRepository.GetTypeByCodeAsync(typeCode, ct);
            if (type == null)
            {
                type = new MangaWorkflow.Domain.Entities.NotificationType
                {
                    TypeCode = typeCode,
                    TypeName = typeCode
                };
                await _notificationRepository.AddTypeAsync(type, ct);
                await _notificationRepository.SaveChangesAsync(ct);
            }
            return type;
        }

        public async Task MarkReadAsync(Guid notificationId, Guid currentUserId, CancellationToken ct = default)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId, ct);
            if (notification != null && notification.UserId == currentUserId && !notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _notificationRepository.SaveChangesAsync(ct);
            }
        }

        public async Task MarkAllReadAsync(Guid currentUserId, CancellationToken ct = default)
        {
            var unread = await _notificationRepository.GetAllUnreadAsync(currentUserId, ct);
            foreach (var n in unread)
            {
                n.IsRead = true;
                n.ReadAt = DateTime.UtcNow;
            }
            if (unread.Any())
            {
                await _notificationRepository.SaveChangesAsync(ct);
            }
        }

        public async Task<MangaWorkflow.Application.DTOs.Notifications.NotificationDto?> CreateAndSendAsync(Guid userId, string typeCode, string title, string message, string? referenceType = null, Guid? referenceId = null, CancellationToken ct = default)
        {
            var type = await GetOrCreateTypeByCodeAsync(typeCode, ct);

            var notification = new MangaWorkflow.Domain.Entities.Notification
            {
                UserId = userId,
                NotificationTypeId = type.NotificationTypeId,
                Title = title,
                Message = message,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification, ct);
            await _notificationRepository.SaveChangesAsync(ct);

            var dto = new MangaWorkflow.Application.DTOs.Notifications.NotificationDto
            {
                NotificationId = notification.NotificationId,
                UserId = notification.UserId,
                TypeCode = type.TypeCode,
                Title = notification.Title,
                Message = notification.Message,
                ReferenceType = notification.ReferenceType,
                ReferenceId = notification.ReferenceId,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };

            await _workflowHubNotifier.SendToUserAsync(userId, "ReceiveNotification", dto, ct);
            return dto;
        }
    }
}
