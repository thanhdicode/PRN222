using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class WorkflowStatusHistory
{
    public Guid WorkflowHistoryId { get; set; }

    public string EntityName { get; set; } = null!;

    public Guid EntityId { get; set; }

    public string? FromStatusCode { get; set; }

    public string ToStatusCode { get; set; } = null!;

    public Guid? ChangedByUserId { get; set; }

    public string? Note { get; set; }

    public DateTime ChangedAt { get; set; }

    public virtual User? ChangedByUser { get; set; }
}
