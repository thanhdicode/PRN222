using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class PublicationIssue
{
    public Guid PublicationIssueId { get; set; }

    public string IssueNumber { get; set; } = null!;

    public string? IssueTitle { get; set; }

    public DateOnly? PublishedDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ChapterPublication> ChapterPublications { get; set; } = new List<ChapterPublication>();
}
