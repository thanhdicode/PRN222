using MangaWorkflow.Application.DTOs.Rankings;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface IRankingService
    {
        Task<List<RankingListItemDto>> GetRankingsByIssueAsync(string issueNumber, CancellationToken ct = default);
        Task<List<string>> GetAvailableIssueNumbersAsync(CancellationToken ct = default);
        Task CreateOrUpdateRankingAsync(CreateRankingDto dto, CancellationToken ct = default);
        Task<List<RankingListItemDto>> GetLatestRankingsAsync(CancellationToken ct = default);
    }
}
