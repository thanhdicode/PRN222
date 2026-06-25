using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class Chapter
{
    public Guid ChapterId { get; set; }

    public Guid SeriesId { get; set; }

    public int ChapterNumber { get; set; }

    public string Title { get; set; } = null!;

    public string? Summary { get; set; }

    public int ChapterStatusId { get; set; }

    public DateTime? Deadline { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual ChapterPublication? ChapterPublication { get; set; }

    public virtual ChapterStatus ChapterStatus { get; set; } = null!;

    public virtual ICollection<MangaPage> MangaPages { get; set; } = new List<MangaPage>();

    public virtual ICollection<Manuscript> Manuscripts { get; set; } = new List<Manuscript>();

    public virtual Series Series { get; set; } = null!;
}
