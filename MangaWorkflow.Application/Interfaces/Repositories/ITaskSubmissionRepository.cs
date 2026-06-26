using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface ITaskSubmissionRepository
    {
        Task<TaskSubmission?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<TaskSubmission>> GetPendingSubmissionsForMangakaAsync(Guid mangakaId, CancellationToken ct = default);
        Task<TaskSubmission?> GetWithTaskAsync(Guid submissionId, CancellationToken ct = default);
        Task AddAsync(TaskSubmission submission, CancellationToken ct = default);
        Task UpdateAsync(TaskSubmission submission, CancellationToken ct = default);
    }
}
