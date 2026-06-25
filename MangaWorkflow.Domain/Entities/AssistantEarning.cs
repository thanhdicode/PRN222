using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class AssistantEarning
{
    public Guid EarningId { get; set; }

    public Guid AssistantId { get; set; }

    public Guid TaskId { get; set; }

    public decimal Amount { get; set; }

    public int EarningStatusId { get; set; }

    public DateTime CalculatedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual User Assistant { get; set; } = null!;

    public virtual EarningStatus EarningStatus { get; set; } = null!;

    public virtual ProductionTask Task { get; set; } = null!;
}
