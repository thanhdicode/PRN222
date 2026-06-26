using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.DTOs.Dashboard;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface IDashboardRepository
    {
        Task<int> CountTotalUsersAsync(CancellationToken ct = default);
        Task<int> CountTotalSeriesAsync(CancellationToken ct = default);
        Task<int> CountActiveTasksAsync(CancellationToken ct = default);
        Task<int> CountPendingSubmissionsAsync(CancellationToken ct = default);
        
        Task<int> CountActiveSeriesByMangakaAsync(Guid mangakaId, CancellationToken ct = default);
        Task<int> CountPendingReviewByMangakaAsync(Guid mangakaId, CancellationToken ct = default);
        Task<List<ChapterProgressItemDto>> GetChapterProgressByMangakaAsync(Guid mangakaId, CancellationToken ct = default);
        
        Task<int> CountActiveTasksByAssistantAsync(Guid assistantId, CancellationToken ct = default);
        Task<int> CountOverdueTasksByAssistantAsync(Guid assistantId, CancellationToken ct = default);
        Task<List<TaskSummaryItemDto>> GetRecentTasksByAssistantAsync(Guid assistantId, int limit, CancellationToken ct = default);
        
        Task<int> CountAssignedSeriesByEditorAsync(Guid editorId, CancellationToken ct = default);
        Task<int> CountUnresolvedCommentsByEditorAsync(Guid editorId, CancellationToken ct = default);
        
        Task<int> CountPendingVotesByBoardMemberAsync(Guid boardMemberId, CancellationToken ct = default);
        Task<int> CountTotalSeriesReviewedByBoardMemberAsync(Guid boardMemberId, CancellationToken ct = default);
    }
}
