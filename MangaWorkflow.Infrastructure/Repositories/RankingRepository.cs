using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class RankingRepository : IRankingRepository
    {
        private readonly MangaWorkflowDbContext _context;

        public RankingRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<List<RankingRecord>> GetByIssueNumberAsync(string issueNumber, CancellationToken ct = default)
        {
            return await _context.RankingRecords
                .Include(r => r.Series)
                .Where(r => r.IssueNumber == issueNumber)
                .OrderBy(r => r.RankPosition)
                .ToListAsync(ct);
        }

        public async Task<List<string>> GetDistinctIssueNumbersAsync(CancellationToken ct = default)
        {
            return await _context.RankingRecords
                .Select(r => r.IssueNumber)
                .Distinct()
                .OrderByDescending(i => i)
                .ToListAsync(ct);
        }

        public async Task<RankingRecord?> GetExistingAsync(Guid seriesId, string issueNumber, CancellationToken ct = default)
        {
            return await _context.RankingRecords
                .FirstOrDefaultAsync(r => r.SeriesId == seriesId && r.IssueNumber == issueNumber, ct);
        }

        public async Task AddAsync(RankingRecord record, CancellationToken ct = default)
        {
            await _context.RankingRecords.AddAsync(record, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
