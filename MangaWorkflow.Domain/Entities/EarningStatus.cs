using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class EarningStatus
{
    public int EarningStatusId { get; set; }

    public string StatusCode { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public virtual ICollection<AssistantEarning> AssistantEarnings { get; set; } = new List<AssistantEarning>();
}
