using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class ChapterPublication
{
    public Guid ChapterPublicationId { get; set; }

    public Guid ChapterId { get; set; }

    public Guid PublicationIssueId { get; set; }

    public int PublicationOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Chapter Chapter { get; set; } = null!;

    public virtual PublicationIssue PublicationIssue { get; set; } = null!;
}
