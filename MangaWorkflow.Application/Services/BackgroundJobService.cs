using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.Interfaces.Repositories;

namespace MangaWorkflow.Application.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly ITaskWorkflowService _taskService;
        private readonly IBackgroundJobQueriesRepository _jobQueriesRepo;
        private readonly INotificationService _notificationService;
        private readonly ILogger<BackgroundJobService> _logger;

        public BackgroundJobService(
            ITaskWorkflowService taskService,
            IBackgroundJobQueriesRepository jobQueriesRepo,
            INotificationService notificationService,
            ILogger<BackgroundJobService> logger)
        {
            _taskService = taskService;
            _jobQueriesRepo = jobQueriesRepo;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<int> ProcessDeadlineRemindersAsync(CancellationToken ct = default)
        {
            var tasks = await _taskService.GetTasksDueWithinHoursAsync(48, ct);
            int count = 0;

            foreach (var task in tasks)
            {
                // Check if notification already sent in the last 48 hours to avoid duplicates
                bool alreadySent = await _jobQueriesRepo.HasRecentNotificationAsync(
                    task.AssignedToUserId, 
                    "DeadlineWarning", 
                    "Task Deadline Approaching", 
                    TimeSpan.FromHours(48), 
                    ct);

                if (!alreadySent)
                {
                    await _notificationService.CreateAndSendAsync(
                        task.AssignedToUserId,
                        "DeadlineWarning",
                        "Task Deadline Approaching",
                        $"Task '{task.Title}' is due within 48 hours.",
                        ct: ct);
                    count++;
                }
            }

            return count;
        }

        public async Task<int> ProcessOverdueTasksAsync(CancellationToken ct = default)
        {
            var tasks = await _taskService.GetOverdueTasksAsync(ct);
            if (!tasks.Any()) return 0;

            var taskIds = tasks.Select(t => t.TaskId).ToList();
            await _taskService.MarkTasksAsOverdueAsync(taskIds, ct);

            int count = 0;
            foreach (var task in tasks)
            {
                // Notify Assistant
                await _notificationService.CreateAndSendAsync(
                    task.AssignedToUserId,
                    "TaskOverdue",
                    "Task Overdue",
                    $"Your task '{task.Title}' is now overdue.",
                    ct: ct);

                // Notify Mangaka
                if (task.MangakaId != Guid.Empty && task.MangakaId != task.AssignedToUserId)
                {
                    await _notificationService.CreateAndSendAsync(
                        task.MangakaId,
                        "TaskOverdue",
                        "Assistant Task Overdue",
                        $"Task '{task.Title}' assigned to your assistant is now overdue.",
                        ct: ct);
                }
                
                count++;
            }

            return count;
        }

        public async Task<int> ProcessRankingRisksAsync(CancellationToken ct = default)
        {
            var seriesList = await _jobQueriesRepo.GetSeriesForRankingRiskAsync(ct);
            int count = 0;

            foreach (var series in seriesList)
            {
                var latestRecords = series.RankingRecords.OrderByDescending(r => r.CalculatedAt).Take(2).ToList();
                if (latestRecords.Count == 2)
                {
                    var current = latestRecords[0];
                    var previous = latestRecords[1];

                    if (current.RankPosition - previous.RankPosition > 3)
                    {
                        var newRiskScore = series.CancellationRiskScore + 10m;
                        await _jobQueriesRepo.UpdateSeriesCancellationRiskAsync(series.SeriesId, newRiskScore, ct);

                        await _notificationService.CreateAndSendAsync(
                            series.MangakaId,
                            "RankingRisk",
                            "Series Ranking Warning",
                            $"Your series '{series.Title}' has dropped significantly in ranking. Risk score increased.",
                            ct: ct);
                        
                        count++;
                    }
                }
            }

            return count;
        }

        public async Task<int> ProcessNotificationCleanupAsync(CancellationToken ct = default)
        {
            return await _jobQueriesRepo.CleanupOldReadNotificationsAsync(30, ct);
        }

        public async Task<int> ProcessMonthlyEarningsAsync(CancellationToken ct = default)
        {
            // Fixed rate demo logic
            return await _jobQueriesRepo.CalculateMonthlyEarningsAsync(1000m, ct);
        }
    }
}
