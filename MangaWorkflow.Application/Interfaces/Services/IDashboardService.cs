using MangaWorkflow.Application.DTOs;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
        Task<MangaWorkflow.Application.DTOs.Dashboard.AdminDashboardDto> GetAdminDashboardAsync(CancellationToken ct = default);
        Task<MangaWorkflow.Application.DTOs.Dashboard.MangakaDashboardDto> GetMangakaDashboardAsync(Guid mangakaId, CancellationToken ct = default);
        Task<MangaWorkflow.Application.DTOs.Dashboard.AssistantDashboardDto> GetAssistantDashboardAsync(Guid assistantId, CancellationToken ct = default);
        Task<MangaWorkflow.Application.DTOs.Dashboard.EditorDashboardDto> GetEditorDashboardAsync(Guid editorId, CancellationToken ct = default);
        Task<MangaWorkflow.Application.DTOs.Dashboard.BoardDashboardDto> GetBoardDashboardAsync(Guid boardMemberId, CancellationToken ct = default);
    }
}
