using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class ProductionTaskRepository : IProductionTaskRepository
    {
        private readonly MangaWorkflowDbContext _context;
        public ProductionTaskRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountTasksAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ProductionTasks.CountAsync(cancellationToken);
        }

        public async Task<List<ProductionTask>> GetByAssistantAsync(Guid assistantId, string? statusCode, CancellationToken ct = default)
        {
            var query = _context.ProductionTasks
                .Include(t => t.TaskType)
                .Include(t => t.TaskStatus)
                .Include(t => t.Page)
                    .ThenInclude(p => p.Chapter)
                        .ThenInclude(c => c.Series)
                .Where(t => t.AssignedToAssistantId == assistantId);

            if (!string.IsNullOrEmpty(statusCode))
            {
                query = query.Where(t => t.TaskStatus.StatusCode == statusCode);
            }

            return await query.OrderBy(t => t.Deadline).ToListAsync(ct);
        }

        public async Task<ProductionTask?> GetWithDetailsAsync(Guid taskId, CancellationToken ct = default)
        {
            return await _context.ProductionTasks
                .Include(t => t.TaskType)
                .Include(t => t.TaskStatus)
                .Include(t => t.Page)
                .Include(t => t.AssignedToAssistant)
                .FirstOrDefaultAsync(t => t.TaskId == taskId, ct);
        }

        public async Task UpdateStatusAsync(Guid taskId, string statusCode, CancellationToken ct = default)
        {
            var task = await _context.ProductionTasks.FindAsync(new object[] { taskId }, ct);
            if (task != null)
            {
                var status = await _context.TaskStatuses
                    .FirstOrDefaultAsync(s => s.StatusCode == statusCode, ct);
                if (status == null)
                    throw new InvalidOperationException($"TaskStatus '{statusCode}' not found.");

                task.TaskStatusId = status.TaskStatusId;
                task.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<ProductionTask?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.ProductionTasks.FindAsync(new object[] { id }, ct);
        }

        public async Task AddAsync(ProductionTask task, CancellationToken ct = default)
        {
            _context.ProductionTasks.Add(task);
            await _context.SaveChangesAsync(ct);
        }
    }
}

