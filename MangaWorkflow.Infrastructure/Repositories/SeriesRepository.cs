using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Domain.Entities;
using MangaWorkflow.Application.Interfaces.Repositories;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class SeriesRepository : ISeriesRepository
    {
        private readonly MangaWorkflowDbContext _context;
        public SeriesRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountSeriesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Series.CountAsync(cancellationToken);
        }

        public async Task<List<Series>> GetRecentSeriesAsync(int take = 10, CancellationToken cancellationToken = default)
        {
            return await _context.Series
                .OrderByDescending(s => s.CreatedAt)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Series>> GetByMangakaAsync(Guid mangakaId, string? statusCode, string? keyword, CancellationToken ct = default)
        {
            var query = _context.Series
                .Include(s => s.SeriesStatus)
                .Include(s => s.Mangaka)
                .Include(s => s.Chapters)
                .Where(s => s.MangakaId == mangakaId && !s.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(statusCode))
                query = query.Where(s => s.SeriesStatus.StatusCode == statusCode);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(s => s.Title.Contains(keyword) || (s.Genre != null && s.Genre.Contains(keyword)));

            return await query.OrderByDescending(s => s.CreatedAt).ToListAsync(ct);
        }

        public async Task<List<Series>> GetAllFilteredAsync(string? statusCode, string? keyword, CancellationToken ct = default)
        {
            var query = _context.Series
                .Include(s => s.SeriesStatus)
                .Include(s => s.Mangaka)
                .Include(s => s.Chapters)
                .Where(s => !s.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(statusCode))
                query = query.Where(s => s.SeriesStatus.StatusCode == statusCode);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(s => s.Title.Contains(keyword) || (s.Genre != null && s.Genre.Contains(keyword)));

            return await query.OrderByDescending(s => s.CreatedAt).ToListAsync(ct);
        }

        public async Task<Series?> GetByIdWithDetailsAsync(Guid seriesId, CancellationToken ct = default)
        {
            return await _context.Series
                .Include(s => s.SeriesStatus)
                .Include(s => s.Mangaka)
                .Include(s => s.Chapters)
                .Include(s => s.BoardVotes)
                    .ThenInclude(v => v.VoteValue)
                .Include(s => s.BoardVotes)
                    .ThenInclude(v => v.BoardMember)
                .FirstOrDefaultAsync(s => s.SeriesId == seriesId, ct);
        }

        public async Task<List<Series>> GetSubmittedSeriesAsync(CancellationToken ct = default)
        {
            return await _context.Series
                .Include(s => s.SeriesStatus)
                .Include(s => s.Mangaka)
                .Include(s => s.Chapters)
                .Include(s => s.BoardVotes)
                .Where(s => !s.IsDeleted &&
                    (s.SeriesStatus.StatusCode == "Submitted" || s.SeriesStatus.StatusCode == "UnderReview"))
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Series series, CancellationToken ct = default)
        {
            await _context.Series.AddAsync(series, ct);
        }

        public async Task<List<SeriesStatus>> GetAllStatusesAsync(CancellationToken ct = default)
        {
            return await _context.SeriesStatuses.ToListAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}

