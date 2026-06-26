using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.DTOs.Tasks;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Services
{
    public class TaskWorkflowService : ITaskWorkflowService
    {
        private readonly IProductionTaskRepository _taskRepo;

        public TaskWorkflowService(IProductionTaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
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
                PageImageUrl = task.Page?.ImageUrl,
                Instructions = task.Description,
                AssignedToUserId = task.AssignedToAssistantId,
                AssignedToUserName = task.AssignedToAssistant?.FullName
            };
        }

        public async Task StartTaskAsync(Guid taskId, Guid assistantId, CancellationToken ct = default)
        {
            var task = await _taskRepo.GetByIdAsync(taskId, ct);
            if (task != null && task.AssignedToAssistantId == assistantId && task.TaskStatus.StatusCode == "Assigned")
            {
                await _taskRepo.UpdateStatusAsync(taskId, "InProgress", ct);
            }
        }

        public Task<List<TaskStatusOption>> GetTaskStatusOptionsAsync(CancellationToken ct = default)
        {
            // Just mocked for simplicity without creating a new repo method, usually from a TaskStatus lookup table
            var list = new List<TaskStatusOption>
            {
                new TaskStatusOption { StatusCode = "Assigned", StatusName = "Assigned" },
                new TaskStatusOption { StatusCode = "InProgress", StatusName = "In Progress" },
                new TaskStatusOption { StatusCode = "Submitted", StatusName = "Submitted" },
                new TaskStatusOption { StatusCode = "Approved", StatusName = "Approved" },
                new TaskStatusOption { StatusCode = "Rejected", StatusName = "Rejected" },
                new TaskStatusOption { StatusCode = "RevisionRequired", StatusName = "Revision Required" },
                new TaskStatusOption { StatusCode = "Overdue", StatusName = "Overdue" }
            };
            return Task.FromResult(list);
        }

        public async Task<List<TaskDeadlineReminderDto>> GetTasksDueWithinHoursAsync(int hours, CancellationToken ct = default)
        {
            var tasks = await _taskRepo.GetTasksDueWithinHoursAsync(hours, ct);
            return tasks.Select(t => new TaskDeadlineReminderDto
            {
                TaskId = t.TaskId,
                Title = t.Title,
                AssignedToUserId = t.AssignedToAssistantId,
                Deadline = t.Deadline.Value
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





