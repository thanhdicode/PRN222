using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class Manuscript
{
    public Guid ManuscriptId { get; set; }

    public Guid SeriesId { get; set; }

    public Guid? ChapterId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string FileUrl { get; set; } = null!;

    public string? FileName { get; set; }

    public string? ContentType { get; set; }

    public long? FileSizeBytes { get; set; }

    public int VersionNo { get; set; }

    public int ManuscriptStatusId { get; set; }

    public Guid UploadedByUserId { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual Chapter? Chapter { get; set; }

    public virtual ManuscriptStatus ManuscriptStatus { get; set; } = null!;

    public virtual Series Series { get; set; } = null!;

    public virtual User UploadedByUser { get; set; } = null!;
}
