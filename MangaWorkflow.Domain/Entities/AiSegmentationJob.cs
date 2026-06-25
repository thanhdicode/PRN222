using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class AiSegmentationJob
{
    public Guid AiSegmentationJobId { get; set; }

    public Guid PageId { get; set; }

    public Guid RequestedByUserId { get; set; }

    public string ModelName { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? RawResultJson { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual MangaPage Page { get; set; } = null!;

    public virtual User RequestedByUser { get; set; } = null!;
}
