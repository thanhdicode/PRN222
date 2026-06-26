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

        public async Task<List<ProductionTask>> GetTasksDueWithinHoursAsync(int hours, CancellationToken ct = default)
        {
            var cutoff = DateTime.UtcNow.AddHours(hours);
            return await _context.ProductionTasks
                .Include(t => t.TaskStatus)
                .Where(t => t.Deadline <= cutoff && 
                            t.TaskStatus.StatusCode != "Approved" && 
                            t.TaskStatus.StatusCode != "Cancelled")
                .ToListAsync(ct);
        }

        public async Task<List<ProductionTask>> GetOverdueTasksAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var excludedStatuses = new[] { "Approved", "Rejected", "Cancelled", "Overdue" };
            
            return await _context.ProductionTasks
                .Include(t => t.TaskStatus)
                .Include(t => t.Page).ThenInclude(p => p.Chapter).ThenInclude(c => c.Series)
                .Where(t => t.Deadline < now && !excludedStatuses.Contains(t.TaskStatus.StatusCode))
                .ToListAsync(ct);
        }

        public async Task MarkTasksAsOverdueAsync(List<Guid> taskIds, CancellationToken ct = default)
        {
            var overdueStatus = await _context.TaskStatuses.FirstOrDefaultAsync(s => s.StatusCode == "Overdue", ct);
            if (overdueStatus == null) return;

            var tasks = await _context.ProductionTasks
                .Where(t => taskIds.Contains(t.TaskId))
                .ToListAsync(ct);

            foreach (var task in tasks)
            {
                task.TaskStatusId = overdueStatus.TaskStatusId;
                task.UpdatedAt = DateTime.UtcNow;
                
                // Add to WorkflowStatusHistory if exists in DB Context
                _context.WorkflowStatusHistories.Add(new WorkflowStatusHistory
                {
                    WorkflowHistoryId = Guid.NewGuid(),
                    EntityName = "ProductionTask",
                    EntityId = task.TaskId,
                    FromStatusCode = null, // Or try to get current if tracked
                    ToStatusCode = overdueStatus.StatusCode,
                    ChangedByUserId = null,
                    ChangedAt = DateTime.UtcNow,
                    Note = "Automatically marked as Overdue by system worker."
                });
            }

            await _context.SaveChangesAsync(ct);
        }
    }
}

