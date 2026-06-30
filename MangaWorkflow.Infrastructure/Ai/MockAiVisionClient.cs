using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.DTOs.Ai;
using MangaWorkflow.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace MangaWorkflow.Infrastructure.Ai;

public class MockAiVisionClient : IAiVisionClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MockAiVisionClient> _logger;

    public MockAiVisionClient(HttpClient httpClient, ILogger<MockAiVisionClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<AiSegmentationResponseDto> SegmentPageAsync(AiSegmentationRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/segment", request, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AiSegmentationResponseDto>(cancellationToken: cancellationToken);
                if (result != null) return result;
            }
            
            _logger.LogWarning("Failed to call AI python API, falling back to local mock data.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling AI python API, falling back to local mock data.");
        }

        // Fallback mock logic if API is unreachable
        return new AiSegmentationResponseDto
        {
            PageId = request.PageId,
            ModelName = "mock-yolo-seg-fallback",
            ModelVersion = "v1-fallback",
            ImageUrl = request.ImageUrl,
            Detections = new List<AiDetectedRegionDto>
            {
                new AiDetectedRegionDto { Label = "Panel", Confidence = 0.95m, X = 10, Y = 10, Width = 400, Height = 300 },
                new AiDetectedRegionDto { Label = "SpeechBubble", Confidence = 0.88m, X = 50, Y = 50, Width = 100, Height = 80 },
                new AiDetectedRegionDto { Label = "Character", Confidence = 0.92m, X = 200, Y = 150, Width = 150, Height = 200 }
            }
        };
    }
}
