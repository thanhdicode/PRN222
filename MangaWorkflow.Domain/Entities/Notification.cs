using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class Notification
{
    public Guid NotificationId { get; set; }

    public Guid UserId { get; set; }

    public int NotificationTypeId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? ReferenceType { get; set; }

    public Guid? ReferenceId { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual NotificationType NotificationType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
