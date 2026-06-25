using MangaWorkflow.Application.DTOs;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
    }
}
