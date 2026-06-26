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
    public class OverdueTaskScannerWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OverdueTaskScannerWorker> _logger;
        private readonly int _intervalMinutes;

        public OverdueTaskScannerWorker(IServiceScopeFactory scopeFactory, ILogger<OverdueTaskScannerWorker> logger, IConfiguration config)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _intervalMinutes = config.GetValue<int>("WorkerSettings:OverdueTaskScannerIntervalMinutes", 5);
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
                _logger.LogInformation("OverdueTaskScannerWorker cancelled.");
            }
        }

        private async Task ProcessJobAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var jobService = scope.ServiceProvider.GetRequiredService<IBackgroundJobService>();
            var logRepo = scope.ServiceProvider.GetRequiredService<IBackgroundJobLogRepository>();

            var jobLog = await logRepo.StartJobAsync("OverdueTaskScannerWorker", ct);
            try
            {
                int processedCount = await jobService.ProcessOverdueTasksAsync(ct);
                await logRepo.CompleteJobAsync(jobLog.JobLogId, $"Marked {processedCount} tasks as overdue.", ct);
                _logger.LogInformation("OverdueTaskScannerWorker: {count} tasks marked overdue.", processedCount);
            }
            catch (Exception ex)
            {
                await logRepo.FailJobAsync(jobLog.JobLogId, ex.Message, ct);
                _logger.LogError(ex, "OverdueTaskScannerWorker failed.");
            }
        }
    }
}
