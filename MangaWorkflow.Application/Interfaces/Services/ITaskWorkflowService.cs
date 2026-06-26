using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.DTOs.Tasks;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface ITaskWorkflowService
    {
        Task<List<TaskListItemDto>> GetAssistantTasksAsync(Guid assistantId, string? statusFilter, CancellationToken ct = default);
        Task<TaskDetailDto?> GetTaskDetailAsync(Guid taskId, CancellationToken ct = default);
        Task StartTaskAsync(Guid taskId, Guid assistantId, CancellationToken ct = default);
        Task<List<TaskStatusOption>> GetTaskStatusOptionsAsync(CancellationToken ct = default);
    }
}
