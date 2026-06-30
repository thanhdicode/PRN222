using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;
using MangaWorkflow.Infrastructure.Persistence;

namespace MangaWorkflow.Infrastructure.Repositories;

public class AiInferenceRepository : IAiInferenceRepository
{
    private readonly MangaWorkflowDbContext _context;

    public AiInferenceRepository(MangaWorkflowDbContext context)
    {
        _context = context;
    }

    public async Task<AiInferenceRequest?> GetByIdAsync(Guid inferenceRequestId, CancellationToken ct = default)
    {
        return await _context.AiInferenceRequests
            .FirstOrDefaultAsync(r => r.InferenceRequestId == inferenceRequestId, ct);
    }

    public async Task AddAsync(AiInferenceRequest request, CancellationToken ct = default)
    {
        await _context.AiInferenceRequests.AddAsync(request, ct);
    }

    public async Task UpdateAsync(AiInferenceRequest request, CancellationToken ct = default)
    {
        _context.AiInferenceRequests.Update(request);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}
