using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories;

public interface IAiDetectedRegionRepository
{
    Task<AiDetectedRegion?> GetByIdAsync(Guid detectedRegionId, CancellationToken ct = default);
    Task<List<AiDetectedRegion>> GetByPageIdAsync(Guid pageId, CancellationToken ct = default);
    Task<List<AiDetectedRegion>> GetByPageIdAndAcceptedStatusAsync(Guid pageId, bool? isAccepted, CancellationToken ct = default);
    Task AddAsync(AiDetectedRegion region, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<AiDetectedRegion> regions, CancellationToken ct = default);
    Task UpdateAsync(AiDetectedRegion region, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
