using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class PageRegion
{
    public Guid RegionId { get; set; }

    public Guid PageId { get; set; }

    public int RegionTypeId { get; set; }

    public decimal X { get; set; }

    public decimal Y { get; set; }

    public decimal Width { get; set; }

    public decimal Height { get; set; }

    public string? Label { get; set; }

    public decimal? Confidence { get; set; }

    public string SourceType { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual MangaPage Page { get; set; } = null!;

    public virtual ICollection<ProductionTask> ProductionTasks { get; set; } = new List<ProductionTask>();

    public virtual RegionType RegionType { get; set; } = null!;
}
