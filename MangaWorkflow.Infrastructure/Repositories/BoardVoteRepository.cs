using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class BoardVoteRepository : IBoardVoteRepository
    {
        private readonly MangaWorkflowDbContext _context;

        public BoardVoteRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<List<BoardVote>> GetBySeriesAsync(Guid seriesId, CancellationToken ct = default)
        {
            return await _context.BoardVotes
                .Include(v => v.VoteValue)
                .Include(v => v.BoardMember)
                .Where(v => v.SeriesId == seriesId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<BoardVote?> GetExistingVoteAsync(Guid seriesId, Guid boardMemberId, CancellationToken ct = default)
        {
            return await _context.BoardVotes
                .FirstOrDefaultAsync(v => v.SeriesId == seriesId && v.BoardMemberId == boardMemberId, ct);
        }

        public async Task<List<VoteValue>> GetVoteValuesAsync(CancellationToken ct = default)
        {
            return await _context.VoteValues.ToListAsync(ct);
        }

        public async Task AddAsync(BoardVote vote, CancellationToken ct = default)
        {
            await _context.BoardVotes.AddAsync(vote, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
