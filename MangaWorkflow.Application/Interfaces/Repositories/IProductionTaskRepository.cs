using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface IProductionTaskRepository
    {
        Task<int> CountTasksAsync(CancellationToken cancellationToken = default);
        Task<List<ProductionTask>> GetByAssistantAsync(Guid assistantId, string? statusCode, CancellationToken ct = default);
        Task<ProductionTask?> GetWithDetailsAsync(Guid taskId, CancellationToken ct = default);
        Task UpdateStatusAsync(Guid taskId, string statusCode, CancellationToken ct = default);
        Task<ProductionTask?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(ProductionTask task, CancellationToken ct = default);
    }
}
