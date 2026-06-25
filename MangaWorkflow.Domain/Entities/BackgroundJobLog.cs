using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class BackgroundJobLog
{
    public Guid JobLogId { get; set; }

    public string JobName { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? Message { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }
}
