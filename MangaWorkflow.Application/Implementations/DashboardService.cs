using System;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.DTOs;
using MangaWorkflow.Application.DTOs.Dashboard;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
        {
            return new DashboardSummaryDto
            {
                TotalUsers = await _dashboardRepository.CountTotalUsersAsync(cancellationToken),
                TotalSeries = await _dashboardRepository.CountTotalSeriesAsync(cancellationToken),
                TotalChapters = 0, // No longer tracked in summary, but leaving for compat
                TotalTasks = await _dashboardRepository.CountActiveTasksAsync(cancellationToken),
                UnreadNotifications = 0 
            };
        }

        public async Task<AdminDashboardDto> GetAdminDashboardAsync(CancellationToken ct = default)
        {
            return new AdminDashboardDto
            {
                TotalUsers = await _dashboardRepository.CountTotalUsersAsync(ct),
                TotalSeries = await _dashboardRepository.CountTotalSeriesAsync(ct),
                ActiveTasks = await _dashboardRepository.CountActiveTasksAsync(ct),
                PendingSubmissions = await _dashboardRepository.CountPendingSubmissionsAsync(ct)
            };
        }

        public async Task<MangakaDashboardDto> GetMangakaDashboardAsync(Guid mangakaId, CancellationToken ct = default)
        {
            return new MangakaDashboardDto
            {
                ActiveSeriesCount = await _dashboardRepository.CountActiveSeriesByMangakaAsync(mangakaId, ct),
                PendingReviewCount = await _dashboardRepository.CountPendingReviewByMangakaAsync(mangakaId, ct),
                ChapterProgress = await _dashboardRepository.GetChapterProgressByMangakaAsync(mangakaId, ct)
            };
        }

        public async Task<AssistantDashboardDto> GetAssistantDashboardAsync(Guid assistantId, CancellationToken ct = default)
        {
            return new AssistantDashboardDto
            {
                ActiveTasksCount = await _dashboardRepository.CountActiveTasksByAssistantAsync(assistantId, ct),
                OverdueTasksCount = await _dashboardRepository.CountOverdueTasksByAssistantAsync(assistantId, ct),
                RecentTasks = await _dashboardRepository.GetRecentTasksByAssistantAsync(assistantId, 5, ct)
            };
        }

        public async Task<EditorDashboardDto> GetEditorDashboardAsync(Guid editorId, CancellationToken ct = default)
        {
            return new EditorDashboardDto
            {
                AssignedSeriesCount = await _dashboardRepository.CountAssignedSeriesByEditorAsync(editorId, ct),
                UnresolvedCommentsCount = await _dashboardRepository.CountUnresolvedCommentsByEditorAsync(editorId, ct)
            };
        }

        public async Task<BoardDashboardDto> GetBoardDashboardAsync(Guid boardMemberId, CancellationToken ct = default)
        {
            return new BoardDashboardDto
            {
                PendingVotesCount = await _dashboardRepository.CountPendingVotesByBoardMemberAsync(boardMemberId, ct),
                TotalSeriesReviewed = await _dashboardRepository.CountTotalSeriesReviewedByBoardMemberAsync(boardMemberId, ct)
            };
        }
    }
}
