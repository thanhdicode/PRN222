using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class PageRegionRepository : IPageRegionRepository
    {
        private readonly MangaWorkflowDbContext _context;
        public PageRegionRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<PageRegion?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.PageRegions.FindAsync(new object[] { id }, ct);
        }

        public async Task<List<PageRegion>> GetByPageAsync(Guid pageId, CancellationToken ct = default)
        {
            return await _context.PageRegions
                .Include(r => r.RegionType)
                .Where(r => r.PageId == pageId)
                .ToListAsync(ct);
        }

        public async Task AddAsync(PageRegion region, CancellationToken ct = default)
        {
            _context.PageRegions.Add(region);
            await _context.SaveChangesAsync(ct);
        }
    }
}
