using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class TaskType
{
    public int TaskTypeId { get; set; }

    public string TypeCode { get; set; } = null!;

    public string TypeName { get; set; } = null!;

    public virtual ICollection<ProductionTask> ProductionTasks { get; set; } = new List<ProductionTask>();
}
