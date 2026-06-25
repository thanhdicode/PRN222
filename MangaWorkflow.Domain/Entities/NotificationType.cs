using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class NotificationType
{
    public int NotificationTypeId { get; set; }

    public string TypeCode { get; set; } = null!;

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
