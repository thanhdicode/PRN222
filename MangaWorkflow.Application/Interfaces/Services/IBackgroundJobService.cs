using System.Threading;
using System.Threading.Tasks;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface IBackgroundJobService
    {
        Task<int> ProcessDeadlineRemindersAsync(CancellationToken ct = default);
        Task<int> ProcessOverdueTasksAsync(CancellationToken ct = default);
        Task<int> ProcessRankingRisksAsync(CancellationToken ct = default);
        Task<int> ProcessNotificationCleanupAsync(CancellationToken ct = default);
        Task<int> ProcessMonthlyEarningsAsync(CancellationToken ct = default);
    }
}
