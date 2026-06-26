using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.Interfaces.Repositories;

namespace MangaWorkflow.Worker
{
    public class NotificationCleanupWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NotificationCleanupWorker> _logger;
        private readonly int _intervalHours;

        public NotificationCleanupWorker(IServiceScopeFactory scopeFactory, ILogger<NotificationCleanupWorker> logger, IConfiguration config)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _intervalHours = config.GetValue<int>("WorkerSettings:NotificationCleanupIntervalHours", 24);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromHours(_intervalHours));
            
            try
            {
                do
                {
                    await ProcessJobAsync(stoppingToken);
                }
                while (await timer.WaitForNextTickAsync(stoppingToken));
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("NotificationCleanupWorker cancelled.");
            }
        }

        private async Task ProcessJobAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var jobService = scope.ServiceProvider.GetRequiredService<IBackgroundJobService>();
            var logRepo = scope.ServiceProvider.GetRequiredService<IBackgroundJobLogRepository>();

            var jobLog = await logRepo.StartJobAsync("NotificationCleanupWorker", ct);
            try
            {
                int processedCount = await jobService.ProcessNotificationCleanupAsync(ct);
                string summary = processedCount == 0 ? "Skipped — no old notifications." : $"Cleaned up {processedCount} old notifications.";
                await logRepo.CompleteJobAsync(jobLog.JobLogId, summary, ct);
                _logger.LogInformation("NotificationCleanupWorker: {summary}", summary);
            }
            catch (Exception ex)
            {
                await logRepo.FailJobAsync(jobLog.JobLogId, ex.Message, ct);
                _logger.LogError(ex, "NotificationCleanupWorker failed.");
            }
        }
    }
}
