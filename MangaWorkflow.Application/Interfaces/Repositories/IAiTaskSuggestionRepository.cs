using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories;

public interface IAiTaskSuggestionRepository
{
    Task<AiTaskSuggestion?> GetByIdAsync(Guid taskSuggestionId, CancellationToken ct = default);
    Task<List<AiTaskSuggestion>> GetByPageIdAsync(Guid pageId, CancellationToken ct = default);
    Task AddAsync(AiTaskSuggestion suggestion, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<AiTaskSuggestion> suggestions, CancellationToken ct = default);
    Task UpdateAsync(AiTaskSuggestion suggestion, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
