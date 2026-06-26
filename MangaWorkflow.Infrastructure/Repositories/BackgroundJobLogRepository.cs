using System;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;
using MangaWorkflow.Infrastructure.Persistence;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class BackgroundJobLogRepository : IBackgroundJobLogRepository
    {
        private readonly MangaWorkflowDbContext _context;

        public BackgroundJobLogRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<BackgroundJobLog> StartJobAsync(string workerName, CancellationToken ct = default)
        {
            var log = new BackgroundJobLog
            {
                JobLogId = Guid.NewGuid(),
                JobName = workerName,
                Status = "Started",
                StartedAt = DateTime.UtcNow
            };

            _context.BackgroundJobLogs.Add(log);
            await _context.SaveChangesAsync(ct);
            return log;
        }

        public async Task CompleteJobAsync(Guid logId, string summary, CancellationToken ct = default)
        {
            var log = await _context.BackgroundJobLogs.FindAsync(new object[] { logId }, ct);
            if (log != null)
            {
                log.Status = "Succeeded";
                log.Message = summary;
                log.FinishedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task FailJobAsync(Guid logId, string errorMessage, CancellationToken ct = default)
        {
            var log = await _context.BackgroundJobLogs.FindAsync(new object[] { logId }, ct);
            if (log != null)
            {
                log.Status = "Failed";
                log.Message = errorMessage;
                log.FinishedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<System.Collections.Generic.List<BackgroundJobLog>> GetRecentLogsAsync(int count, CancellationToken ct = default)
        {
            return await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
                System.Linq.Queryable.Take(
                    System.Linq.Queryable.OrderByDescending(_context.BackgroundJobLogs, l => l.StartedAt), 
                    count), 
                ct);
        }
    }
}
