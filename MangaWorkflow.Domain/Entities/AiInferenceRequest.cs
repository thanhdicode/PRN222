using System;

namespace MangaWorkflow.Domain.Entities;

public partial class AiInferenceRequest
{
    public Guid InferenceRequestId { get; set; } = Guid.NewGuid();
    public Guid PageId { get; set; }
    public Guid RequestedByUserId { get; set; }
    public Guid? ModelVersionId { get; set; }
    public string RequestType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual MangaPage Page { get; set; } = null!;
    public virtual User RequestedByUser { get; set; } = null!;
    public virtual AiModelVersion? ModelVersion { get; set; }
}
