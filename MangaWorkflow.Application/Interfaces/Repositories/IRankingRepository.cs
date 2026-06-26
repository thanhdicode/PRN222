using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface IRankingRepository
    {
        Task<List<RankingRecord>> GetByIssueNumberAsync(string issueNumber, CancellationToken ct = default);
        Task<List<string>> GetDistinctIssueNumbersAsync(CancellationToken ct = default);
        Task<RankingRecord?> GetExistingAsync(Guid seriesId, string issueNumber, CancellationToken ct = default);
        Task AddAsync(RankingRecord record, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
