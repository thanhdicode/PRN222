using System;

namespace MangaWorkflow.Domain.Entities;

public partial class AiEvaluationMetric
{
    public Guid MetricId { get; set; } = Guid.NewGuid();
    public Guid? ExperimentId { get; set; }
    public Guid? ModelVersionId { get; set; }
    public string ClassName { get; set; } = null!;
    public decimal? IoU { get; set; }
    public decimal? DiceF1 { get; set; }
    public decimal? PrecisionValue { get; set; }
    public decimal? RecallValue { get; set; }
    public decimal? Map50 { get; set; }
    public decimal? Map5095 { get; set; }
    public decimal? LatencyMs { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual AiTrainingExperiment? Experiment { get; set; }
    public virtual AiModelVersion? ModelVersion { get; set; }
}
