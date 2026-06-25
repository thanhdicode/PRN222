using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class TaskStatus
{
    public int TaskStatusId { get; set; }

    public string StatusCode { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public virtual ICollection<ProductionTask> ProductionTasks { get; set; } = new List<ProductionTask>();
}
