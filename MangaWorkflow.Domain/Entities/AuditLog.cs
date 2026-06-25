using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class AuditLog
{
    public Guid AuditLogId { get; set; }

    public Guid? ActorUserId { get; set; }

    public string EntityName { get; set; } = null!;

    public Guid? EntityId { get; set; }

    public string ActionName { get; set; } = null!;

    public string? Details { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? ActorUser { get; set; }
}
