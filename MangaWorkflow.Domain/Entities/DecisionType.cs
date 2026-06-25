using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class DecisionType
{
    public int DecisionTypeId { get; set; }

    public string DecisionCode { get; set; } = null!;

    public string DecisionName { get; set; } = null!;

    public virtual ICollection<PublishingDecision> PublishingDecisions { get; set; } = new List<PublishingDecision>();
}
