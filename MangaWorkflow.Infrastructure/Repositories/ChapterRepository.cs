using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class ChapterRepository : IChapterRepository
    {
        private readonly MangaWorkflowDbContext _context;
        public ChapterRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountChaptersAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Chapters.CountAsync(cancellationToken);
        }

        public async Task<List<Chapter>> GetBySeriesAsync(Guid seriesId, CancellationToken ct = default)
        {
            return await _context.Chapters
                .Include(c => c.ChapterStatus)
                .Include(c => c.Series)
                .Include(c => c.MangaPages)
                .Where(c => c.SeriesId == seriesId)
                .OrderBy(c => c.ChapterNumber)
                .ToListAsync(ct);
        }

        public async Task<Chapter?> GetByIdWithDetailsAsync(Guid chapterId, CancellationToken ct = default)
        {
            return await _context.Chapters
                .Include(c => c.ChapterStatus)
                .Include(c => c.Series)
                    .ThenInclude(s => s.SeriesStatus)
                .Include(c => c.MangaPages)
                    .ThenInclude(p => p.PageRegions)
                .FirstOrDefaultAsync(c => c.ChapterId == chapterId, ct);
        }

        public async Task AddAsync(Chapter chapter, CancellationToken ct = default)
        {
            await _context.Chapters.AddAsync(chapter, ct);
        }

        public async Task<List<ChapterStatus>> GetAllStatusesAsync(CancellationToken ct = default)
        {
            return await _context.ChapterStatuses.ToListAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}

