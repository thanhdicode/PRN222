using System;

namespace MangaWorkflow.Domain.Entities;

public partial class AiTaskSuggestion
{
    public Guid TaskSuggestionId { get; set; } = Guid.NewGuid();
    public Guid PageId { get; set; }
    public Guid? PageRegionId { get; set; }
    public Guid? SuggestedAssistantId { get; set; }
    public string TaskTypeCode { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal ComplexityScore { get; set; } = 1;
    public decimal? EstimatedHours { get; set; }
    public decimal? EstimatedAmount { get; set; }
    public string Status { get; set; } = "Draft";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual MangaPage Page { get; set; } = null!;
    public virtual PageRegion? PageRegion { get; set; }
    public virtual User? SuggestedAssistant { get; set; }
}
