using System;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface IBackgroundJobLogRepository
    {
        Task<BackgroundJobLog> StartJobAsync(string workerName, CancellationToken ct = default);
        Task CompleteJobAsync(Guid logId, string summary, CancellationToken ct = default);
        Task FailJobAsync(Guid logId, string errorMessage, CancellationToken ct = default);
        Task<System.Collections.Generic.List<BackgroundJobLog>> GetRecentLogsAsync(int count, CancellationToken ct = default);
    }
}
