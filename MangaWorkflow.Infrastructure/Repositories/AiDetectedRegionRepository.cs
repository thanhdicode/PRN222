using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;
using MangaWorkflow.Infrastructure.Persistence;

namespace MangaWorkflow.Infrastructure.Repositories;

public class AiDetectedRegionRepository : IAiDetectedRegionRepository
{
    private readonly MangaWorkflowDbContext _context;

    public AiDetectedRegionRepository(MangaWorkflowDbContext context)
    {
        _context = context;
    }

    public async Task<AiDetectedRegion?> GetByIdAsync(Guid detectedRegionId, CancellationToken ct = default)
    {
        return await _context.AiDetectedRegions
            .FirstOrDefaultAsync(r => r.DetectedRegionId == detectedRegionId, ct);
    }

    public async Task<List<AiDetectedRegion>> GetByPageIdAsync(Guid pageId, CancellationToken ct = default)
    {
        return await _context.AiDetectedRegions
            .Where(r => r.PageId == pageId)
            .OrderByDescending(r => r.Confidence)
            .ToListAsync(ct);
    }

    public async Task<List<AiDetectedRegion>> GetByPageIdAndAcceptedStatusAsync(Guid pageId, bool? isAccepted, CancellationToken ct = default)
    {
        return await _context.AiDetectedRegions
            .Where(r => r.PageId == pageId && r.IsAccepted == isAccepted)
            .OrderByDescending(r => r.Confidence)
            .ToListAsync(ct);
    }

    public async Task AddAsync(AiDetectedRegion region, CancellationToken ct = default)
    {
        await _context.AiDetectedRegions.AddAsync(region, ct);
    }

    public async Task AddRangeAsync(IEnumerable<AiDetectedRegion> regions, CancellationToken ct = default)
    {
        await _context.AiDetectedRegions.AddRangeAsync(regions, ct);
    }

    public async Task UpdateAsync(AiDetectedRegion region, CancellationToken ct = default)
    {
        _context.AiDetectedRegions.Update(region);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
}
