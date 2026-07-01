using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.DTOs.Tasks;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Services
{
    public class TaskWorkflowService : ITaskWorkflowService
    {
        private readonly IProductionTaskRepository _taskRepo;
        private readonly ITaskStatusRepository _taskStatusRepo;

        public TaskWorkflowService(
            IProductionTaskRepository taskRepo,
            ITaskStatusRepository taskStatusRepo)
        {
            _taskRepo = taskRepo;
            _taskStatusRepo = taskStatusRepo;
        }

        public async Task<List<TaskListItemDto>> GetAssistantTasksAsync(Guid assistantId, string? statusFilter, CancellationToken ct = default)
        {
            var tasks = await _taskRepo.GetByAssistantAsync(assistantId, statusFilter, ct);
            return tasks.Select(t => new TaskListItemDto
            {
                TaskId = t.TaskId,
                Title = t.Title,
                TypeCode = t.TaskType.TypeCode,
                TypeName = t.TaskType?.TypeName ?? "",
                StatusCode = t.TaskStatus.StatusCode,
                StatusName = t.TaskStatus?.StatusName ?? "",
                Deadline = t.Deadline,
                SeriesTitle = t.Page?.Chapter?.Series?.Title,
                ChapterTitle = t.Page?.Chapter?.Title,
                PageNumber = t.Page?.PageNumber
            }).ToList();
        }

        public async Task<TaskDetailDto?> GetTaskDetailAsync(Guid taskId, CancellationToken ct = default)
        {
            var task = await _taskRepo.GetWithDetailsAsync(taskId, ct);
            if (task == null) return null;

            return new TaskDetailDto
            {
                TaskId = task.TaskId,
                Title = task.Title,
                Description = task.Description ?? "",
                TypeCode = task.TaskType.TypeCode,
                StatusCode = task.TaskStatus.StatusCode,
                StatusName = task.TaskStatus?.StatusName ?? "",
                Deadline = task.Deadline,
                PageId = task.PageId,
                // B6 FIX: Map PageNumber so the view shows "Page 3" instead of raw UUID
                PageNumber = task.Page?.PageNumber,
                PageImageUrl = task.Page?.ImageUrl,
                Instructions = task.Description,
                AssignedToUserId = task.AssignedToAssistantId,
                AssignedToUserName = task.AssignedToAssistant?.FullName
            };
        }

        public async Task StartTaskAsync(Guid taskId, Guid assistantId, CancellationToken ct = default)
        {
            // B7 FIX: Use GetWithDetailsAsync (which includes TaskStatus navigation property)
            // instead of GetByIdAsync (which uses FindAsync with no Include).
            // Previously, TaskStatus navigation property was null because FindAsync does NOT
            // load related entities — causing the guard check to always fail silently,
            // meaning status was never updated to InProgress.
            var task = await _taskRepo.GetWithDetailsAsync(taskId, ct);

            if (task == null ||
                task.AssignedToAssistantId != assistantId ||
                task.TaskStatus?.StatusCode != "Assigned")
                return;

            await _taskRepo.UpdateStatusAsync(taskId, "InProgress", ct);
        }

        /// <summary>
        /// Returns all task status options directly from the database — no hardcoded lists.
        /// </summary>
        public async Task<List<TaskStatusOption>> GetTaskStatusOptionsAsync(CancellationToken ct = default)
        {
            var statuses = await _taskStatusRepo.GetAllAsync(ct);
            return statuses.Select(s => new TaskStatusOption
            {
                StatusCode = s.StatusCode,
                StatusName = s.StatusName
            }).ToList();
        }

        public async Task<List<TaskDeadlineReminderDto>> GetTasksDueWithinHoursAsync(int hours, CancellationToken ct = default)
        {
            var tasks = await _taskRepo.GetTasksDueWithinHoursAsync(hours, ct);
            return tasks.Select(t => new TaskDeadlineReminderDto
            {
                TaskId = t.TaskId,
                Title = t.Title,
                AssignedToUserId = t.AssignedToAssistantId,
                Deadline = t.Deadline!.Value
            }).ToList();
        }

        public async Task<List<TaskOverdueDto>> GetOverdueTasksAsync(CancellationToken ct = default)
        {
            var tasks = await _taskRepo.GetOverdueTasksAsync(ct);
            return tasks.Select(t => new TaskOverdueDto
            {
                TaskId = t.TaskId,
                Title = t.Title,
                AssignedToUserId = t.AssignedToAssistantId,
                MangakaId = t.Page?.Chapter?.Series?.MangakaId ?? Guid.Empty
            }).ToList();
        }

        public async Task MarkTasksAsOverdueAsync(List<Guid> taskIds, CancellationToken ct = default)
        {
            if (taskIds.Any())
            {
                await _taskRepo.MarkTasksAsOverdueAsync(taskIds, ct);
            }
        }
    }
}
