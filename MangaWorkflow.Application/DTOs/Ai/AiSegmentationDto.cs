using System;
using System.Collections.Generic;

namespace MangaWorkflow.Application.DTOs.Ai;

public class AiDetectedRegionDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Confidence { get; set; }
    public decimal X { get; set; }
    public decimal Y { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public string? Polygon { get; set; }
}

public class AiSegmentationResponseDto
{
    public string PageId { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public string ModelVersion { get; set; } = string.Empty;
    public List<AiDetectedRegionDto> Detections { get; set; } = new();
}

public class AiSegmentationRequestDto
{
    public string PageId { get; set; } = string.Empty;
}
