using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class VwAssistantMonthlyEarning
{
    public Guid AssistantId { get; set; }

    public string AssistantName { get; set; } = null!;

    public int? EarningYear { get; set; }

    public int? EarningMonth { get; set; }

    public int? TotalPaidTasks { get; set; }

    public decimal? TotalAmount { get; set; }
}
