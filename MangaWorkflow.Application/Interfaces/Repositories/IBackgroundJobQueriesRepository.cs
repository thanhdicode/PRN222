using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface IBackgroundJobQueriesRepository
    {
        Task<bool> HasRecentNotificationAsync(Guid userId, string notificationType, string title, TimeSpan withinTimeSpan, CancellationToken ct, string? referenceType = null, Guid? referenceId = null);
        Task<List<Series>> GetSeriesForRankingRiskAsync(CancellationToken ct);
        Task UpdateSeriesCancellationRiskAsync(Guid seriesId, decimal riskScore, CancellationToken ct);
        Task<int> CleanupOldReadNotificationsAsync(int daysOld, CancellationToken ct);
        Task<int> CalculateMonthlyEarningsAsync(decimal fixedRate, CancellationToken ct);
    }
}
