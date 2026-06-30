using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;
using MangaWorkflow.Infrastructure.Persistence;

namespace MangaWorkflow.Infrastructure.Repositories;

public class AiTaskSuggestionRepository : IAiTaskSuggestionRepository
{
    private readonly MangaWorkflowDbContext _context;

    public AiTaskSuggestionRepository(MangaWorkflowDbContext context)
    {
        _context = context;
    }

    public async Task<AiTaskSuggestion?> GetByIdAsync(Guid taskSuggestionId, CancellationToken ct = default)
    {
        return await _context.AiTaskSuggestions
            .FirstOrDefaultAsync(s => s.TaskSuggestionId == taskSuggestionId, ct);
    }

    public async Task<List<AiTaskSuggestion>> GetByPageIdAsync(Guid pageId, CancellationToken ct = default)
    {
        return await _context.AiTaskSuggestions
            .Where(s => s.PageId == pageId)
            .ToListAsync(ct);
    }

    public async Task AddAsync(AiTaskSuggestion suggestion, CancellationToken ct = default)
    {
        await _context.AiTaskSuggestions.AddAsync(suggestion, ct);
    }

    public async Task AddRangeAsync(IEnumerable<AiTaskSuggestion> suggestions, CancellationToken ct = default)
    {
        await _context.AiTaskSuggestions.AddRangeAsync(suggestions, ct);
    }

    public async Task UpdateAsync(AiTaskSuggestion suggestion, CancellationToken ct = default)
    {
        _context.AiTaskSuggestions.Update(suggestion);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}
