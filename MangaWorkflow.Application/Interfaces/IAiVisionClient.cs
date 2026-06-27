using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.DTOs.Ai;

namespace MangaWorkflow.Application.Interfaces;

public interface IAiVisionClient
{
    Task<AiSegmentationResponseDto> SegmentPageAsync(string pageId, CancellationToken cancellationToken = default);
}
