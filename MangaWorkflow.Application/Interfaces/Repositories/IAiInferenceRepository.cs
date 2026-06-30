using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories;

public interface IAiInferenceRepository
{
    Task<AiInferenceRequest?> GetByIdAsync(Guid inferenceRequestId, CancellationToken ct = default);
    Task AddAsync(AiInferenceRequest request, CancellationToken ct = default);
    Task UpdateAsync(AiInferenceRequest request, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
