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
    public class MonthlyEarningCalculatorWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MonthlyEarningCalculatorWorker> _logger;
        private readonly int _intervalHours;

        public MonthlyEarningCalculatorWorker(IServiceScopeFactory scopeFactory, ILogger<MonthlyEarningCalculatorWorker> logger, IConfiguration config)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _intervalHours = config.GetValue<int>("WorkerSettings:MonthlyEarningCalculatorIntervalHours", 24);
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
                _logger.LogInformation("MonthlyEarningCalculatorWorker cancelled.");
            }
        }

        private async Task ProcessJobAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var jobService = scope.ServiceProvider.GetRequiredService<IBackgroundJobService>();
            var logRepo = scope.ServiceProvider.GetRequiredService<IBackgroundJobLogRepository>();

            var jobLog = await logRepo.StartJobAsync("MonthlyEarningCalculatorWorker", ct);
            try
            {
                int processedCount = await jobService.ProcessMonthlyEarningsAsync(ct);
                await logRepo.CompleteJobAsync(jobLog.JobLogId, $"Calculated earnings for {processedCount} tasks.", ct);
                _logger.LogInformation("MonthlyEarningCalculatorWorker: {count} earning records created.", processedCount);
            }
            catch (Exception ex)
            {
                await logRepo.FailJobAsync(jobLog.JobLogId, ex.Message, ct);
                _logger.LogError(ex, "MonthlyEarningCalculatorWorker failed.");
            }
        }
    }
}
