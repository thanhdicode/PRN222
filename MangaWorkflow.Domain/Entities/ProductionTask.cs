using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class ProductionTask
{
    public Guid TaskId { get; set; }

    public Guid PageId { get; set; }

    public Guid? RegionId { get; set; }

    public Guid AssignedToAssistantId { get; set; }

    public Guid CreatedByMangakaId { get; set; }

    public int TaskTypeId { get; set; }

    public int TaskStatusId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int Priority { get; set; }

    public DateTime? Deadline { get; set; }

    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User AssignedToAssistant { get; set; } = null!;

    public virtual AssistantEarning? AssistantEarning { get; set; }

    public virtual User CreatedByMangaka { get; set; } = null!;

    public virtual MangaPage Page { get; set; } = null!;

    public virtual PageRegion? Region { get; set; }

    public virtual TaskStatus TaskStatus { get; set; } = null!;

    public virtual ICollection<TaskSubmission> TaskSubmissions { get; set; } = new List<TaskSubmission>();

    public virtual TaskType TaskType { get; set; } = null!;
}
