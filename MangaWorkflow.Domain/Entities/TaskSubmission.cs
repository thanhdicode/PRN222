using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class TaskSubmission
{
    public Guid SubmissionId { get; set; }

    public Guid TaskId { get; set; }

    public Guid SubmittedByAssistantId { get; set; }

    public string FileUrl { get; set; } = null!;

    public string? FileName { get; set; }

    public string? ContentType { get; set; }

    public long? FileSizeBytes { get; set; }

    public string? Comment { get; set; }

    public int SubmissionStatusId { get; set; }

    public DateTime SubmittedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public Guid? ReviewedByMangakaId { get; set; }

    public string? ReviewNote { get; set; }

    public virtual User? ReviewedByMangaka { get; set; }

    public virtual SubmissionStatus SubmissionStatus { get; set; } = null!;

    public virtual User SubmittedByAssistant { get; set; } = null!;

    public virtual ProductionTask Task { get; set; } = null!;
}
