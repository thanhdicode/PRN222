using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class TaskSubmissionRepository : ITaskSubmissionRepository
    {
        private readonly MangaWorkflowDbContext _context;
        public TaskSubmissionRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<TaskSubmission?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.TaskSubmissions.FindAsync(new object[] { id }, ct);
        }

        public async Task<List<TaskSubmission>> GetPendingSubmissionsForMangakaAsync(Guid mangakaId, CancellationToken ct = default)
        {
            return await _context.TaskSubmissions
                .Include(s => s.Task)
                    .ThenInclude(t => t.AssignedToAssistant)
                .Include(s => s.Task)
                    .ThenInclude(t => t.Page)
                        .ThenInclude(p => p.Chapter)
                            .ThenInclude(c => c.Series)
                                .ThenInclude(s => s.SeriesTeamMembers)
                .Where(s => s.SubmissionStatus.StatusCode == "PendingReview" && 
                            s.Task.Page.Chapter.Series.SeriesTeamMembers.Any(tm => tm.UserId == mangakaId && tm.RoleInSeries == "Mangaka"))
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync(ct);
        }

        public async Task<TaskSubmission?> GetWithTaskAsync(Guid submissionId, CancellationToken ct = default)
        {
            return await _context.TaskSubmissions
                .Include(s => s.Task)
                    .ThenInclude(t => t.AssignedToAssistant)
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId, ct);
        }

        public async Task AddAsync(TaskSubmission submission, CancellationToken ct = default)
        {
            _context.TaskSubmissions.Add(submission);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(TaskSubmission submission, CancellationToken ct = default)
        {
            _context.TaskSubmissions.Update(submission);
            await _context.SaveChangesAsync(ct);
        }
    }
}


