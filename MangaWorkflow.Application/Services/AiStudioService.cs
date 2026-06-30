using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.DTOs.Ai;
using MangaWorkflow.Application.Interfaces;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Services;

public class AiStudioService : IAiStudioService
{
    private readonly IAiInferenceRepository _inferenceRepository;
    private readonly IAiDetectedRegionRepository _detectedRegionRepository;
    private readonly IAiTaskSuggestionRepository _taskSuggestionRepository;
    private readonly IPageRepository _pageRepository;
    private readonly IPageRegionRepository _pageRegionRepository;
    private readonly ITaskTypeRepository _taskTypeRepository;
    private readonly ITaskStatusRepository _taskStatusRepository;
    private readonly IProductionTaskRepository _productionTaskRepository;
    private readonly IAiVisionClient _aiClient;

    public AiStudioService(
        IAiInferenceRepository inferenceRepository,
        IAiDetectedRegionRepository detectedRegionRepository,
        IAiTaskSuggestionRepository taskSuggestionRepository,
        IPageRepository pageRepository,
        IPageRegionRepository pageRegionRepository,
        ITaskTypeRepository taskTypeRepository,
        ITaskStatusRepository taskStatusRepository,
        IProductionTaskRepository productionTaskRepository,
        IAiVisionClient aiClient)
    {
        _inferenceRepository = inferenceRepository;
        _detectedRegionRepository = detectedRegionRepository;
        _taskSuggestionRepository = taskSuggestionRepository;
        _pageRepository = pageRepository;
        _pageRegionRepository = pageRegionRepository;
        _taskTypeRepository = taskTypeRepository;
        _taskStatusRepository = taskStatusRepository;
        _productionTaskRepository = productionTaskRepository;
        _aiClient = aiClient;
    }

    public async Task<AiInferenceRequest> RunSegmentationAsync(
        Guid pageId, 
        Guid requestedByUserId, 
        CancellationToken cancellationToken = default)
    {
        // Load the MangaPage with its ImageUrl
        var page = await _pageRepository.GetByIdWithDetailsAsync(pageId, cancellationToken);
        if (page == null) 
            throw new ArgumentException("Page not found");

        // Create inference request
        var request = new AiInferenceRequest
        {
            PageId = pageId,
            RequestedByUserId = requestedByUserId,
            RequestType = "Segmentation",
            Status = "Processing",
            StartedAt = DateTime.UtcNow
        };
        await _inferenceRepository.AddAsync(request, cancellationToken);
        await _inferenceRepository.SaveChangesAsync(cancellationToken);

        try
        {
            // Create request DTO with image data
            var segmentationRequest = new AiSegmentationRequestDto
            {
                PageId = pageId.ToString(),
                ImageUrl = page.ImageUrl,
                ImagePath = page.FileName,
                Mode = "mock"
            };

            var response = await _aiClient.SegmentPageAsync(segmentationRequest, cancellationToken);

            // Save detected regions
            var detectedRegions = response.Detections.Select(det => new AiDetectedRegion
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
            }).ToList();

            await _detectedRegionRepository.AddRangeAsync(detectedRegions, cancellationToken);

            request.Status = "Completed";
            request.FinishedAt = DateTime.UtcNow;
            await _inferenceRepository.UpdateAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            request.Status = "Failed";
            request.FinishedAt = DateTime.UtcNow;
            request.ErrorMessage = ex.Message;
            await _inferenceRepository.UpdateAsync(request, cancellationToken);
        }

        await _inferenceRepository.SaveChangesAsync(cancellationToken);
        return request;
    }

    public async Task<List<AiDetectedRegion>> GetDetectedRegionsAsync(
        Guid pageId, 
        CancellationToken cancellationToken = default)
    {
        return await _detectedRegionRepository.GetByPageIdAndAcceptedStatusAsync(pageId, null, cancellationToken);
    }

    public async Task AcceptRegionAsync(
        Guid detectedRegionId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var region = await _detectedRegionRepository.GetByIdAsync(detectedRegionId, cancellationToken);
        if (region == null)
            throw new ArgumentException("Detected region not found");

        region.IsAccepted = true;

        // Create PageRegion from accepted detection
        var newRegion = new PageRegion
        {
            PageId = region.PageId,
            RegionTypeId = 1, // TODO: Map from region.RegionTypeCode to actual RegionTypeId in Task 4
            X = region.X,
            Y = region.Y,
            Width = region.Width,
            Height = region.Height,
            Confidence = region.Confidence,
            SourceType = "AI",
            CreatedByUserId = currentUserId
        };

        await _pageRegionRepository.AddAsync(newRegion, cancellationToken);

        region.PageRegionId = newRegion.RegionId;
        await _detectedRegionRepository.UpdateAsync(region, cancellationToken);
        await _detectedRegionRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectRegionAsync(
        Guid detectedRegionId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var region = await _detectedRegionRepository.GetByIdAsync(detectedRegionId, cancellationToken);
        if (region == null)
            throw new ArgumentException("Detected region not found");

        region.IsAccepted = false;
        await _detectedRegionRepository.UpdateAsync(region, cancellationToken);
        await _detectedRegionRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<AiTaskSuggestion>> SuggestTasksForPageAsync(
        Guid pageId, 
        Guid requestedByUserId, 
        CancellationToken cancellationToken = default)
    {
        var page = await _pageRepository.GetByIdWithDetailsAsync(pageId, cancellationToken);
        if (page == null) 
            throw new ArgumentException("Page not found");

        // Get ONLY ACCEPTED detected regions (IsAccepted = true)
        var acceptedDetections = await _detectedRegionRepository.GetByPageIdAndAcceptedStatusAsync(pageId, true, cancellationToken);
        
        // Load all TaskTypes from database
        var allTaskTypes = await _taskTypeRepository.GetAllAsync(cancellationToken);
        var taskTypeDict = allTaskTypes.ToDictionary(t => t.TypeCode, t => t.TaskTypeId);
        
        // Fallback "Other" TaskType
        var otherTaskType = allTaskTypes.FirstOrDefault(t => t.TypeCode == "Other");
        var otherTaskTypeId = otherTaskType?.TaskTypeId ?? 0;

        var suggestions = new List<AiTaskSuggestion>();

        // Generate suggestions based on accepted PageRegions
        foreach (var detection in acceptedDetections)
        {
            // Must have PageRegionId (linked to real PageRegion)
            if (!detection.PageRegionId.HasValue)
                continue;

            string taskTypeCode;
            string title;
            string description;
            decimal complexityScore;

            // Map region type to task type with fallback logic
            switch (detection.RegionTypeCode)
            {
                case "Panel":
                    // Try LayoutCleanup, fallback to Other
                    taskTypeCode = taskTypeDict.ContainsKey("LayoutCleanup") ? "LayoutCleanup" : "Other";
                    title = "Layout Cleanup";
                    description = $"Clean up and organize panel layout at ({detection.X}, {detection.Y})";
                    complexityScore = 2;
                    break;

                case "SpeechBubble":
                    taskTypeCode = taskTypeDict.ContainsKey("SpeechBubbleCleanup") ? "SpeechBubbleCleanup" : "Other";
                    title = "Speech Bubble Cleanup";
                    description = $"Clean and prepare speech bubble at ({detection.X}, {detection.Y})";
                    complexityScore = 1;
                    break;

                case "Character":
                    // Try CleanLine first, then Tone, fallback to Other
                    if (taskTypeDict.ContainsKey("CleanLine"))
                        taskTypeCode = "CleanLine";
                    else if (taskTypeDict.ContainsKey("Tone"))
                        taskTypeCode = "Tone";
                    else
                        taskTypeCode = "Other";
                    title = "Character Line Work";
                    description = $"Clean character lines at ({detection.X}, {detection.Y})";
                    complexityScore = 3;
                    break;

                case "Background":
                    taskTypeCode = taskTypeDict.ContainsKey("BackgroundDrawing") ? "BackgroundDrawing" : "Other";
                    title = "Background Drawing";
                    description = $"Draw background at ({detection.X}, {detection.Y})";
                    complexityScore = 4;
                    break;

                case "Shading":
                    taskTypeCode = taskTypeDict.ContainsKey("Shading") ? "Shading" : "Other";
                    title = "Apply Shading";
                    description = $"Add shading at ({detection.X}, {detection.Y})";
                    complexityScore = 2;
                    break;

                case "Effect":
                    taskTypeCode = taskTypeDict.ContainsKey("Effect") ? "Effect" : "Other";
                    title = "Special Effect";
                    description = $"Add special effect at ({detection.X}, {detection.Y})";
                    complexityScore = 2;
                    break;

                default:
                    taskTypeCode = "Other";
                    title = $"{detection.RegionTypeCode} Processing";
                    description = $"Process {detection.RegionTypeCode} region at ({detection.X}, {detection.Y})";
                    complexityScore = 1;
                    break;
            }

            // Get TaskTypeId (fallback to Other if not found)
            var taskTypeId = taskTypeDict.ContainsKey(taskTypeCode) ? taskTypeDict[taskTypeCode] : otherTaskTypeId;

            suggestions.Add(new AiTaskSuggestion
            {
                PageId = pageId,
                PageRegionId = detection.PageRegionId.Value, // Use actual PageRegionId, NOT DetectedRegionId
                SuggestedAssistantId = null, // Will be selected during approval
                TaskTypeCode = taskTypeCode,
                Title = title,
                Description = description,
                ComplexityScore = complexityScore,
                Status = "Draft" // Always Draft initially
            });
        }

        // Save suggestions
        if (suggestions.Any())
        {
            await _taskSuggestionRepository.AddRangeAsync(suggestions, cancellationToken);
            await _taskSuggestionRepository.SaveChangesAsync(cancellationToken);
        }

        return suggestions;
    }

    public async Task ApproveTaskSuggestionAsync(
        Guid suggestionId,
        Guid mangakaId,
        Guid? assistantId,
        DateTime? deadline,
        CancellationToken cancellationToken = default)
    {
        // Load suggestion
        var suggestion = await _taskSuggestionRepository.GetByIdAsync(suggestionId, cancellationToken);
        if (suggestion == null)
            throw new ArgumentException("Task suggestion not found");

        // Validate status is Draft
        if (suggestion.Status != "Draft")
            throw new InvalidOperationException($"Cannot approve suggestion with status '{suggestion.Status}'. Only Draft suggestions can be approved.");

        // Validate assistant is provided
        if (!assistantId.HasValue || assistantId.Value == Guid.Empty)
            throw new ArgumentException("AssistantId is required to approve task suggestion");

        // Validate PageRegion exists
        if (!suggestion.PageRegionId.HasValue)
            throw new InvalidOperationException("Suggestion must have a PageRegionId");

        var pageRegion = await _pageRegionRepository.GetByIdAsync(suggestion.PageRegionId.Value, cancellationToken);
        if (pageRegion == null)
            throw new ArgumentException("PageRegion not found");

        // Resolve TaskType by TypeCode
        var taskType = await _taskTypeRepository.GetByTypeCodeAsync(suggestion.TaskTypeCode, cancellationToken);
        if (taskType == null)
            throw new ArgumentException($"TaskType with code '{suggestion.TaskTypeCode}' not found");

        // Resolve TaskStatus by StatusCode = "Assigned"
        var taskStatus = await _taskStatusRepository.GetByStatusCodeAsync("Assigned", cancellationToken);
        if (taskStatus == null)
            throw new ArgumentException("TaskStatus 'Assigned' not found in database");

        // Create ProductionTask
        var productionTask = new ProductionTask
        {
            TaskTypeId = taskType.TaskTypeId,
            TaskStatusId = taskStatus.TaskStatusId,
            PageId = suggestion.PageId,
            RegionId = suggestion.PageRegionId,
            Title = suggestion.Title,
            Description = suggestion.Description,
            CreatedByMangakaId = mangakaId,
            AssignedToAssistantId = assistantId.Value,
            Deadline = deadline,
            Priority = 3, // Medium priority
            Price = 0m // Will be calculated based on task type and complexity
        };

        await _productionTaskRepository.AddAsync(productionTask, cancellationToken);

        // Mark suggestion as Approved
        suggestion.Status = "Approved";
        await _taskSuggestionRepository.UpdateAsync(suggestion, cancellationToken);
        await _taskSuggestionRepository.SaveChangesAsync(cancellationToken);

        // Notify Assistant if assigned
        // Note: INotificationService should be injected if needed
        // For now, skipping notification - can be added in Task 8 or 10
    }

    public async Task RejectTaskSuggestionAsync(
        Guid suggestionId,
        Guid mangakaId,
        CancellationToken cancellationToken = default)
    {
        // Load suggestion
        var suggestion = await _taskSuggestionRepository.GetByIdAsync(suggestionId, cancellationToken);
        if (suggestion == null)
            throw new ArgumentException("Task suggestion not found");

        // Validate status is Draft
        if (suggestion.Status != "Draft")
            throw new InvalidOperationException($"Cannot reject suggestion with status '{suggestion.Status}'. Only Draft suggestions can be rejected.");

        // Mark suggestion as Rejected
        suggestion.Status = "Rejected";
        await _taskSuggestionRepository.UpdateAsync(suggestion, cancellationToken);
        await _taskSuggestionRepository.SaveChangesAsync(cancellationToken);
    }
}
