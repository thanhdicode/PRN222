using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class MangaPage
{
    public Guid PageId { get; set; }

    public Guid ChapterId { get; set; }

    public int PageNumber { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? ThumbnailUrl { get; set; }

    public string? FileName { get; set; }

    public string? ContentType { get; set; }

    public long? FileSizeBytes { get; set; }

    public int VersionNo { get; set; }

    public int PageStatusId { get; set; }

    public Guid UploadedByUserId { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual ICollection<AiSegmentationJob> AiSegmentationJobs { get; set; } = new List<AiSegmentationJob>();

    public virtual Chapter Chapter { get; set; } = null!;

    public virtual ICollection<EditorComment> EditorComments { get; set; } = new List<EditorComment>();

    public virtual ICollection<PageRegion> PageRegions { get; set; } = new List<PageRegion>();

    public virtual PageStatus PageStatus { get; set; } = null!;

    public virtual ICollection<ProductionTask> ProductionTasks { get; set; } = new List<ProductionTask>();

    public virtual User UploadedByUser { get; set; } = null!;
}
