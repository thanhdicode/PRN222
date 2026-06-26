using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface IBoardVoteRepository
    {
        Task<List<BoardVote>> GetBySeriesAsync(Guid seriesId, CancellationToken ct = default);
        Task<BoardVote?> GetExistingVoteAsync(Guid seriesId, Guid boardMemberId, CancellationToken ct = default);
        Task<List<VoteValue>> GetVoteValuesAsync(CancellationToken ct = default);
        Task AddAsync(BoardVote vote, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
