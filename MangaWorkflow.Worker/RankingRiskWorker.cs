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
    public class RankingRiskWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RankingRiskWorker> _logger;
        private readonly int _intervalMinutes;

        public RankingRiskWorker(IServiceScopeFactory scopeFactory, ILogger<RankingRiskWorker> logger, IConfiguration config)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _intervalMinutes = config.GetValue<int>("WorkerSettings:RankingRiskIntervalMinutes", 60);
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
                _logger.LogInformation("RankingRiskWorker cancelled.");
            }
        }

        private async Task ProcessJobAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var jobService = scope.ServiceProvider.GetRequiredService<IBackgroundJobService>();
            var logRepo = scope.ServiceProvider.GetRequiredService<IBackgroundJobLogRepository>();

            var jobLog = await logRepo.StartJobAsync("RankingRiskWorker", ct);
            try
            {
                int processedCount = await jobService.ProcessRankingRisksAsync(ct);
                await logRepo.CompleteJobAsync(jobLog.JobLogId, $"Updated risk score for {processedCount} series.", ct);
                _logger.LogInformation("RankingRiskWorker: {count} series updated.", processedCount);
            }
            catch (Exception ex)
            {
                await logRepo.FailJobAsync(jobLog.JobLogId, ex.Message, ct);
                _logger.LogError(ex, "RankingRiskWorker failed.");
            }
        }
    }
}
