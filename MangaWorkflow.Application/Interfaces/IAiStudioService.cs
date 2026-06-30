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
    Task AcceptRegionAsync(Guid detectedRegionId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task RejectRegionAsync(Guid detectedRegionId, Guid currentUserId, CancellationToken cancellationToken = default);
    Task<List<AiTaskSuggestion>> SuggestTasksForPageAsync(Guid pageId, Guid requestedByUserId, CancellationToken cancellationToken = default);
    Task ApproveTaskSuggestionAsync(Guid suggestionId, Guid mangakaId, Guid? assistantId, DateTime? deadline, CancellationToken cancellationToken = default);
    Task RejectTaskSuggestionAsync(Guid suggestionId, Guid mangakaId, CancellationToken cancellationToken = default);
}
