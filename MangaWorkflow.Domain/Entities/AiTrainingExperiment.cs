using System;

namespace MangaWorkflow.Domain.Entities;

public partial class AiTrainingExperiment
{
    public Guid ExperimentId { get; set; } = Guid.NewGuid();
    public string ExperimentName { get; set; } = null!;
    public string ModelType { get; set; } = null!;
    public string DatasetName { get; set; } = null!;
    public string? ConfigJson { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
