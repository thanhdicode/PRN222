using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class VwChapterProgress
{
    public Guid ChapterId { get; set; }

    public Guid SeriesId { get; set; }

    public string SeriesTitle { get; set; } = null!;

    public int ChapterNumber { get; set; }

    public string ChapterTitle { get; set; } = null!;

    public string ChapterStatus { get; set; } = null!;

    public int? TotalTasks { get; set; }

    public int? ApprovedTasks { get; set; }

    public int? OverdueTasks { get; set; }

    public decimal? ProgressPercent { get; set; }

    public DateTime? Deadline { get; set; }
}
