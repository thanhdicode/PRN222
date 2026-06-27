using System;

namespace MangaWorkflow.Domain.Entities;

public partial class AiModelVersion
{
    public Guid ModelVersionId { get; set; } = Guid.NewGuid();
    public string ModelName { get; set; } = null!;
    public string ModelType { get; set; } = null!;
    public string VersionLabel { get; set; } = null!;
    public string Framework { get; set; } = null!;
    public string? ModelPath { get; set; }
    public bool IsActive { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}
