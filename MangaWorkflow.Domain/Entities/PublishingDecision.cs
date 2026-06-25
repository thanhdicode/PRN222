using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class PublishingDecision
{
    public Guid DecisionId { get; set; }

    public Guid SeriesId { get; set; }

    public int DecisionTypeId { get; set; }

    public Guid DecidedByUserId { get; set; }

    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User DecidedByUser { get; set; } = null!;

    public virtual DecisionType DecisionType { get; set; } = null!;

    public virtual Series Series { get; set; } = null!;
}
