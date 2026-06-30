using System;

namespace MangaWorkflow.Domain.Entities;

public partial class AiDetectedRegion
{
    public Guid DetectedRegionId { get; set; } = Guid.NewGuid();
    public Guid InferenceRequestId { get; set; }
    public Guid PageId { get; set; }
    public Guid? PageRegionId { get; set; }
    public string RegionTypeCode { get; set; } = null!;
    public decimal X { get; set; }
    public decimal Y { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal Confidence { get; set; }
    public string? PolygonJson { get; set; }
    public bool? IsAccepted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual AiInferenceRequest InferenceRequest { get; set; } = null!;
    public virtual MangaPage Page { get; set; } = null!;
    public virtual PageRegion? PageRegion { get; set; }
}
