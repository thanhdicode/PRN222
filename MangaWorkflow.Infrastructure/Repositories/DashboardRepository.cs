using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Application.DTOs.Dashboard;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Infrastructure.Persistence;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly MangaWorkflowDbContext _context;

        public DashboardRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountTotalUsersAsync(CancellationToken ct = default)
        {
            return await _context.Users.CountAsync(ct);
        }

        public async Task<int> CountTotalSeriesAsync(CancellationToken ct = default)
        {
            return await _context.Series.CountAsync(ct);
        }

        public async Task<int> CountActiveTasksAsync(CancellationToken ct = default)
        {
            return await _context.ProductionTasks
                .CountAsync(t => t.TaskStatus.StatusCode != "Approved", ct);
        }

        public async Task<int> CountPendingSubmissionsAsync(CancellationToken ct = default)
        {
            return await _context.TaskSubmissions
                .CountAsync(s => s.SubmissionStatus.StatusCode == "Submitted", ct);
        }

        public async Task<int> CountActiveSeriesByMangakaAsync(Guid mangakaId, CancellationToken ct = default)
        {
            return await _context.SeriesTeamMembers
                .Where(m => m.UserId == mangakaId)
                .Select(m => m.Series)
                .Where(s => s.SeriesStatus.StatusCode != "Completed" && s.SeriesStatus.StatusCode != "Cancelled")
                .Distinct()
                .CountAsync(ct);
        }

        public async Task<int> CountPendingReviewByMangakaAsync(Guid mangakaId, CancellationToken ct = default)
        {
            // Submissions pending review for tasks that belong to chapters in series where mangaka is a member
            return await _context.TaskSubmissions
                .Where(s => s.SubmissionStatus.StatusCode == "Submitted")
                .Where(s => s.Task.Page.Chapter.Series.SeriesTeamMembers.Any(m => m.UserId == mangakaId))
                .CountAsync(ct);
        }

        public async Task<List<ChapterProgressItemDto>> GetChapterProgressByMangakaAsync(Guid mangakaId, CancellationToken ct = default)
        {
            var seriesIds = await _context.SeriesTeamMembers
                .Where(m => m.UserId == mangakaId)
                .Select(m => m.SeriesId)
                .Distinct()
                .ToListAsync(ct);

            var query = _context.VwChapterProgresses
                .Where(p => seriesIds.Contains(p.SeriesId))
                .OrderBy(p => p.Deadline)
                .Take(10);

            var list = await query.ToListAsync(ct);
            return list.Select(p => new ChapterProgressItemDto
            {
                SeriesTitle = p.SeriesTitle,
                ChapterTitle = p.ChapterTitle,
                TotalTasks = p.TotalTasks ?? 0,
                CompletedTasks = p.ApprovedTasks ?? 0,
                PublishedDate = null // Assuming we don't have published date in the view
            }).ToList();
        }

        public async Task<int> CountActiveTasksByAssistantAsync(Guid assistantId, CancellationToken ct = default)
        {
            return await _context.ProductionTasks
                .CountAsync(t => t.AssignedToAssistantId == assistantId && t.TaskStatus.StatusCode != "Approved", ct);
        }

        public async Task<int> CountOverdueTasksByAssistantAsync(Guid assistantId, CancellationToken ct = default)
        {
            return await _context.ProductionTasks
                .CountAsync(t => t.AssignedToAssistantId == assistantId 
                              && t.TaskStatus.StatusCode != "Approved" 
                              && t.Deadline < DateTime.UtcNow, ct);
        }

        public async Task<List<TaskSummaryItemDto>> GetRecentTasksByAssistantAsync(Guid assistantId, int limit, CancellationToken ct = default)
        {
            var tasks = await _context.ProductionTasks
                .Include(t => t.TaskStatus)
                .Where(t => t.AssignedToAssistantId == assistantId)
                .OrderBy(t => t.Deadline)
                .Take(limit)
                .ToListAsync(ct);

            return tasks.Select(t => new TaskSummaryItemDto
            {
                TaskId = t.TaskId,
                Description = t.Description ?? string.Empty,
                StatusCode = t.TaskStatus.StatusCode,
                Deadline = t.Deadline ?? DateTime.MaxValue
            }).ToList();
        }

        public async Task<int> CountAssignedSeriesByEditorAsync(Guid editorId, CancellationToken ct = default)
        {
            return await _context.SeriesTeamMembers
                .Where(m => m.UserId == editorId)
                .Select(m => m.SeriesId)
                .Distinct()
                .CountAsync(ct);
        }

        public async Task<int> CountUnresolvedCommentsByEditorAsync(Guid editorId, CancellationToken ct = default)
        {
            return await _context.EditorComments
                .CountAsync(c => c.EditorId == editorId && c.CommentStatus.StatusCode != "Resolved", ct);
        }

        public async Task<int> CountPendingVotesByBoardMemberAsync(Guid boardMemberId, CancellationToken ct = default)
        {
            return await _context.BoardVotes
                .CountAsync(v => v.BoardMemberId == boardMemberId && v.VoteValue.VoteCode == "Pending", ct);
        }

        public async Task<int> CountTotalSeriesReviewedByBoardMemberAsync(Guid boardMemberId, CancellationToken ct = default)
        {
            return await _context.BoardVotes
                .Where(v => v.BoardMemberId == boardMemberId && v.VoteValue.VoteCode != "Pending")
                .Select(v => v.SeriesId)
                .Distinct()
                .CountAsync(ct);
        }
    }
}
