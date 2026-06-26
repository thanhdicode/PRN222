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
    public class DeadlineReminderWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DeadlineReminderWorker> _logger;
        private readonly int _intervalMinutes;

        public DeadlineReminderWorker(IServiceScopeFactory scopeFactory, ILogger<DeadlineReminderWorker> logger, IConfiguration config)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _intervalMinutes = config.GetValue<int>("WorkerSettings:DeadlineReminderIntervalMinutes", 30);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(_intervalMinutes));
            
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
                _logger.LogInformation("DeadlineReminderWorker cancelled.");
            }
        }

        private async Task ProcessJobAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var jobService = scope.ServiceProvider.GetRequiredService<IBackgroundJobService>();
            var logRepo = scope.ServiceProvider.GetRequiredService<IBackgroundJobLogRepository>();

            var jobLog = await logRepo.StartJobAsync("DeadlineReminderWorker", ct);
            try
            {
                int processedCount = await jobService.ProcessDeadlineRemindersAsync(ct);
                await logRepo.CompleteJobAsync(jobLog.JobLogId, $"Sent {processedCount} reminders.", ct);
                _logger.LogInformation("DeadlineReminderWorker: {count} reminders sent.", processedCount);
            }
            catch (Exception ex)
            {
                await logRepo.FailJobAsync(jobLog.JobLogId, ex.Message, ct);
                _logger.LogError(ex, "DeadlineReminderWorker failed.");
            }
        }
    }
}
