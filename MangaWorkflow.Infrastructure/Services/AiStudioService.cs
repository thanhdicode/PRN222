using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.Interfaces;
using MangaWorkflow.Domain.Entities;
using MangaWorkflow.Infrastructure.Persistence; // Or however the context is injected
using Microsoft.EntityFrameworkCore;

namespace MangaWorkflow.Infrastructure.Services;

public class AiStudioService : IAiStudioService
{
    private readonly MangaWorkflowDbContext _context;
    private readonly IAiVisionClient _aiClient;

    public AiStudioService(MangaWorkflowDbContext context, IAiVisionClient aiClient)
    {
        _context = context;
        _aiClient = aiClient;
    }

    public async Task<AiInferenceRequest> RunSegmentationAsync(Guid pageId, Guid requestedByUserId, CancellationToken cancellationToken = default)
    {
        var page = await _context.MangaPages.FindAsync(new object[] { pageId }, cancellationToken);
        if (page == null) throw new ArgumentException("Page not found");

        var request = new AiInferenceRequest
        {
            PageId = pageId,
            RequestedByUserId = requestedByUserId,
            RequestType = "Segmentation",
            Status = "Processing",
            StartedAt = DateTime.UtcNow
        };
        _context.AiInferenceRequests.Add(request);
        await _context.SaveChangesAsync(cancellationToken);

        try
        {
            var response = await _aiClient.SegmentPageAsync(pageId.ToString(), cancellationToken);

            foreach (var det in response.Detections)
            {
                var region = new AiDetectedRegion
                {
                    InferenceRequestId = request.InferenceRequestId,
                    PageId = pageId,
                    RegionTypeCode = det.Label,
                    X = det.X,
                    Y = det.Y,
                    Width = det.Width,
                    Height = det.Height,
                    Confidence = det.Confidence,
                    PolygonJson = det.Polygon
                };
                _context.AiDetectedRegions.Add(region);
            }

            request.Status = "Completed";
            request.FinishedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            request.Status = "Failed";
            request.FinishedAt = DateTime.UtcNow;
            request.ErrorMessage = ex.Message;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return request;
    }

    public async Task<List<AiDetectedRegion>> GetDetectedRegionsAsync(Guid pageId, CancellationToken cancellationToken = default)
    {
        return await _context.AiDetectedRegions
            .Where(r => r.PageId == pageId && r.IsAccepted == null)
            .OrderByDescending(r => r.Confidence)
            .ToListAsync(cancellationToken);
    }

    public async Task AcceptRegionAsync(Guid detectedRegionId, CancellationToken cancellationToken = default)
    {
        var region = await _context.AiDetectedRegions.FindAsync(new object[] { detectedRegionId }, cancellationToken);
        if (region != null)
        {
            region.IsAccepted = true;
            
            // Map the detected region to a real PageRegion
            var regionType = await _context.RegionTypes.FirstOrDefaultAsync(rt => rt.TypeCode == region.RegionTypeCode, cancellationToken);
            var actualRegionType = regionType ?? await _context.RegionTypes.FirstOrDefaultAsync(rt => rt.TypeCode == "Panel", cancellationToken);

            if (actualRegionType != null)
            {
                var newRegion = new PageRegion
                {
                    PageId = region.PageId,
                    RegionTypeId = actualRegionType.RegionTypeId,
                    X = region.X,
                    Y = region.Y,
                    Width = region.Width,
                    Height = region.Height,
                    Confidence = region.Confidence,
                    SourceType = "AI_Model",
                    // Since it's accepted by someone, we should map it, but we need a user ID. For now we use a system user or the first editor.
                    CreatedByUserId = (await _context.Users.FirstAsync(cancellationToken)).UserId
                };
                _context.PageRegions.Add(newRegion);
                await _context.SaveChangesAsync(cancellationToken);
                
                region.PageRegionId = newRegion.RegionId;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RejectRegionAsync(Guid detectedRegionId, CancellationToken cancellationToken = default)
    {
        var region = await _context.AiDetectedRegions.FindAsync(new object[] { detectedRegionId }, cancellationToken);
        if (region != null)
        {
            region.IsAccepted = false;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<AiTaskSuggestion>> SuggestTasksForPageAsync(Guid pageId, Guid requestedByUserId, CancellationToken cancellationToken = default)
    {
        var page = await _context.MangaPages.FindAsync(new object[] { pageId }, cancellationToken);
        if (page == null) throw new ArgumentException("Page not found");

        var regions = await _context.AiDetectedRegions
            .Where(r => r.PageId == pageId)
            .ToListAsync(cancellationToken);

        var suggestions = new List<AiTaskSuggestion>();

        // Mock task suggestion logic based on detected regions
        foreach (var region in regions)
        {
            if (region.RegionTypeCode == "Text")
            {
                suggestions.Add(new AiTaskSuggestion
                {
                    PageId = pageId,
                    PageRegionId = region.DetectedRegionId, // Should be mapped to actual PageRegionId if accepted, but using this for demo
                    TaskTypeCode = "Typesetting",
                    Title = "Typeset Dialogue",
                    Description = $"Typeset text in region ({region.X}, {region.Y})",
                    ComplexityScore = 1,
                    Status = "Pending Review"
                });
            }
            else if (region.RegionTypeCode == "Background")
            {
                suggestions.Add(new AiTaskSuggestion
                {
                    PageId = pageId,
                    PageRegionId = region.DetectedRegionId,
                    TaskTypeCode = "Background_Art",
                    Title = "Draw Background",
                    Description = $"Draw background in region ({region.X}, {region.Y})",
                    ComplexityScore = 3,
                    Status = "Pending Review"
                });
            }
        }

        // Add to DB
        _context.AiTaskSuggestions.AddRange(suggestions);
        await _context.SaveChangesAsync(cancellationToken);

        return suggestions;
    }
}
