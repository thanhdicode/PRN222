using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;
using MangaWorkflow.Infrastructure.Persistence;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class BackgroundJobQueriesRepository : IBackgroundJobQueriesRepository
    {
        private readonly MangaWorkflowDbContext _context;

        public BackgroundJobQueriesRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasRecentNotificationAsync(Guid userId, string notificationType, string title, TimeSpan withinTimeSpan, CancellationToken ct, string? referenceType = null, Guid? referenceId = null)
        {
            var cutoff = DateTime.UtcNow.Subtract(withinTimeSpan);
            var query = _context.Notifications
                .Where(n => n.UserId == userId 
                            && n.NotificationType.TypeCode == notificationType 
                            && n.CreatedAt >= cutoff);

            if (!string.IsNullOrEmpty(referenceType))
            {
                query = query.Where(n => n.ReferenceType == referenceType);
            }

            if (referenceId.HasValue)
            {
                query = query.Where(n => n.ReferenceId == referenceId.Value);
            }
            else
            {
                query = query.Where(n => n.Title == title);
            }

            return await query.AnyAsync(ct);
        }

        public async Task<List<Series>> GetSeriesForRankingRiskAsync(CancellationToken ct)
        {
            // Get series that have ranking records, include the latest 2 records
            return await _context.Series
                .Include(s => s.RankingRecords)
                .Where(s => s.RankingRecords.Any())
                .ToListAsync(ct);
        }

        public async Task UpdateSeriesCancellationRiskAsync(Guid seriesId, decimal riskScore, CancellationToken ct)
        {
            var series = await _context.Series.FindAsync(new object[] { seriesId }, ct);
            if (series != null)
            {
                series.CancellationRiskScore = riskScore;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<int> CleanupOldReadNotificationsAsync(int daysOld, CancellationToken ct)
        {
            var cutoff = DateTime.UtcNow.AddDays(-daysOld);
            var oldNotifications = await _context.Notifications
                .Where(n => n.IsRead && n.CreatedAt < cutoff)
                .ToListAsync(ct);

            if (oldNotifications.Any())
            {
                _context.Notifications.RemoveRange(oldNotifications);
                await _context.SaveChangesAsync(ct);
            }
            return oldNotifications.Count;
        }

        public async Task<int> CalculateMonthlyEarningsAsync(decimal fixedRate, CancellationToken ct)
        {
            // Find tasks that are Approved but don't have an AssistantEarning record
            var approvedTasksWithoutEarnings = await _context.ProductionTasks
                .Include(t => t.TaskStatus)
                .Where(t => t.TaskStatus.StatusCode == "Approved" 
                         && !_context.AssistantEarnings.Any(e => e.TaskId == t.TaskId))
                .ToListAsync(ct);

            if (!approvedTasksWithoutEarnings.Any())
                return 0;

            var pendingEarningStatus = await _context.EarningStatuses.FirstOrDefaultAsync(s => s.StatusCode == "Pending", ct);
            if (pendingEarningStatus == null) return 0;

            var newEarnings = approvedTasksWithoutEarnings.Select(t => new AssistantEarning
            {
                EarningId = Guid.NewGuid(),
                AssistantId = t.AssignedToAssistantId,
                TaskId = t.TaskId,
                Amount = fixedRate,
                EarningStatusId = pendingEarningStatus.EarningStatusId,
                CalculatedAt = DateTime.UtcNow
            }).ToList();

            _context.AssistantEarnings.AddRange(newEarnings);
            await _context.SaveChangesAsync(ct);

            return newEarnings.Count;
        }
    }
}
