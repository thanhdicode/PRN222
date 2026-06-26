using MangaWorkflow.Application.DTOs.Rankings;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Domain.Constants;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Services
{
    public class RankingService : IRankingService
    {
        private readonly IRankingRepository _rankingRepository;
        private readonly ISeriesRepository _seriesRepository;
        private readonly INotificationService _notificationService;

        public RankingService(
            IRankingRepository rankingRepository,
            ISeriesRepository seriesRepository,
            INotificationService notificationService)
        {
            _rankingRepository = rankingRepository;
            _seriesRepository = seriesRepository;
            _notificationService = notificationService;
        }

        public async Task<List<RankingListItemDto>> GetRankingsByIssueAsync(string issueNumber, CancellationToken ct = default)
        {
            var records = await _rankingRepository.GetByIssueNumberAsync(issueNumber, ct);
            return records.OrderBy(r => r.RankPosition).Select(r => new RankingListItemDto
            {
                RankingRecordId = r.RankingRecordId,
                SeriesId = r.SeriesId,
                SeriesTitle = r.Series?.Title ?? "",
                IssueNumber = r.IssueNumber,
                RankPosition = r.RankPosition,
                PreviousRankPosition = r.PreviousRankPosition,
                VoteCount = r.VoteCount,
                Trend = r.Trend,
                TrendDisplay = GetTrendDisplay(r.Trend),
                CalculatedAt = r.CalculatedAt
            }).ToList();
        }

        public async Task<List<string>> GetAvailableIssueNumbersAsync(CancellationToken ct = default)
        {
            return await _rankingRepository.GetDistinctIssueNumbersAsync(ct);
        }

        public async Task<List<RankingListItemDto>> GetLatestRankingsAsync(CancellationToken ct = default)
        {
            var issueNumbers = await _rankingRepository.GetDistinctIssueNumbersAsync(ct);
            if (!issueNumbers.Any()) return new List<RankingListItemDto>();

            var latestIssue = issueNumbers.First();
            return await GetRankingsByIssueAsync(latestIssue, ct);
        }

        public async Task CreateOrUpdateRankingAsync(CreateRankingDto dto, CancellationToken ct = default)
        {
            var existing = await _rankingRepository.GetExistingAsync(dto.SeriesId, dto.IssueNumber, ct);

            if (existing != null)
            {
                // Update existing record
                existing.PreviousRankPosition = existing.RankPosition;
                existing.RankPosition = dto.RankPosition;
                existing.VoteCount = dto.VoteCount;
                existing.Trend = dto.TrendCode ?? "Stable";
                existing.CalculatedAt = DateTime.UtcNow;
                await _rankingRepository.SaveChangesAsync(ct);
            }
            else
            {
                // Create new record
                var record = new RankingRecord
                {
                    RankingRecordId = Guid.NewGuid(),
                    SeriesId = dto.SeriesId,
                    IssueNumber = dto.IssueNumber,
                    RankPosition = dto.RankPosition,
                    VoteCount = dto.VoteCount,
                    Trend = dto.TrendCode ?? "New",
                    CalculatedAt = DateTime.UtcNow
                };

                await _rankingRepository.AddAsync(record, ct);
                await _rankingRepository.SaveChangesAsync(ct);
            }

            // Insert RankingUpdated notification for the series Mangaka
            await InsertRankingNotificationAsync(dto.SeriesId, dto.IssueNumber, dto.RankPosition, ct);
        }

        private async Task InsertRankingNotificationAsync(Guid seriesId, string issueNumber, int rankPosition, CancellationToken ct)
        {
            var series = await _seriesRepository.GetByIdWithDetailsAsync(seriesId, ct);
            if (series == null) return;

            await _notificationService.CreateAndSendAsync(
                series.MangakaId,
                NotificationTypeCodes.RankingUpdated,
                "Ranking Updated",
                $"Your series '{series.Title}' is ranked #{rankPosition} in Issue {issueNumber}.",
                "Series",
                seriesId,
                ct
            );
        }

        private static string GetTrendDisplay(string trend) => trend switch
        {
            "Up" => "↑ Up",
            "Down" => "↓ Down",
            "Stable" => "→ Stable",
            "New" => "★ New",
            _ => trend
        };
    }
}
