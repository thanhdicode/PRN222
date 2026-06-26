using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.DTOs.Submissions;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface ISubmissionService
    {
        Task SubmitTaskAsync(SubmitTaskDto dto, Guid assistantId, CancellationToken ct = default);
        Task<List<PendingSubmissionDto>> GetPendingSubmissionsForMangakaAsync(Guid mangakaId, CancellationToken ct = default);
        Task<SubmissionDetailDto?> GetSubmissionDetailAsync(Guid submissionId, CancellationToken ct = default);
        Task ReviewSubmissionAsync(ReviewSubmissionDto dto, Guid mangakaId, CancellationToken ct = default);
    }
}
