using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class PageRepository : IPageRepository
    {
        private readonly MangaWorkflowDbContext _context;

        public PageRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<List<MangaPage>> GetByChapterAsync(Guid chapterId, CancellationToken ct = default)
        {
            return await _context.MangaPages
                .Include(p => p.PageStatus)
                .Include(p => p.PageRegions)
                .Where(p => p.ChapterId == chapterId)
                .OrderBy(p => p.PageNumber)
                .ToListAsync(ct);
        }

        public async Task<MangaPage?> GetByIdWithDetailsAsync(Guid pageId, CancellationToken ct = default)
        {
            return await _context.MangaPages
                .Include(p => p.PageStatus)
                .Include(p => p.Chapter)
                .Include(p => p.UploadedByUser)
                .Include(p => p.PageRegions)
                .FirstOrDefaultAsync(p => p.PageId == pageId, ct);
        }

        public async Task AddAsync(MangaPage page, CancellationToken ct = default)
        {
            await _context.MangaPages.AddAsync(page, ct);
        }

        public async Task<PageStatus?> GetStatusByCodeAsync(string statusCode, CancellationToken ct = default)
        {
            return await _context.PageStatuses
                .FirstOrDefaultAsync(s => s.StatusCode == statusCode, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
