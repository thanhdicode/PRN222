using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces;

public interface IAiStudioService
{
    Task<AiInferenceRequest> RunSegmentationAsync(Guid pageId, Guid requestedByUserId, CancellationToken cancellationToken = default);
    Task<List<AiDetectedRegion>> GetDetectedRegionsAsync(Guid pageId, CancellationToken cancellationToken = default);
    Task AcceptRegionAsync(Guid detectedRegionId, CancellationToken cancellationToken = default);
    Task RejectRegionAsync(Guid detectedRegionId, CancellationToken cancellationToken = default);
    Task<List<AiTaskSuggestion>> SuggestTasksForPageAsync(Guid pageId, Guid requestedByUserId, CancellationToken cancellationToken = default);
}
