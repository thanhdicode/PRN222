using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.DTOs.Submissions;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ITaskSubmissionRepository _submissionRepo;
        private readonly IProductionTaskRepository _taskRepo;
        private readonly INotificationRepository _notificationRepo;

        public SubmissionService(ITaskSubmissionRepository submissionRepo, IProductionTaskRepository taskRepo, INotificationRepository notificationRepo)
        {
            _submissionRepo = submissionRepo;
            _taskRepo = taskRepo;
            _notificationRepo = notificationRepo;
        }

        public async Task SubmitTaskAsync(SubmitTaskDto dto, Guid assistantId, CancellationToken ct = default)
        {
            var task = await _taskRepo.GetWithDetailsAsync(dto.TaskId, ct);
            if (task == null || task.AssignedToAssistantId != assistantId) return;

            var submission = new TaskSubmission
            {
                SubmissionId = Guid.NewGuid(),
                TaskId = dto.TaskId,
                SubmittedByAssistantId = assistantId,
                FileUrl = dto.FileUrl,
                Comment = dto.Notes,
                SubmissionStatusId = 1,
                SubmittedAt = DateTime.UtcNow
            };

            await _submissionRepo.AddAsync(submission, ct);
            await _taskRepo.UpdateStatusAsync(dto.TaskId, "Submitted", ct);

            // Optional Notification
            var mangakaId = task.Page?.Chapter?.Series?.SeriesTeamMembers.FirstOrDefault(tm => tm.RoleInSeries == "Mangaka")?.UserId;
            if (mangakaId.HasValue)
            {
                await _notificationRepo.AddAsync(new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = mangakaId.Value,
                    NotificationTypeId = 1,
                    Title = "New Submission",
                    Message = $"Task {task.Title} was submitted.",
                    ReferenceType = "TaskSubmission",
                    ReferenceId = submission.SubmissionId,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                }, ct);
            }
        }

        public async Task<List<PendingSubmissionDto>> GetPendingSubmissionsForMangakaAsync(Guid mangakaId, CancellationToken ct = default)
        {
            var submissions = await _submissionRepo.GetPendingSubmissionsForMangakaAsync(mangakaId, ct);
            return submissions.Select(s => new PendingSubmissionDto
            {
                SubmissionId = s.SubmissionId,
                TaskId = s.TaskId,
                TaskTitle = s.Task?.Title ?? "",
                AssistantName = s.Task?.AssignedToAssistant?.FullName ?? "",
                SubmittedAt = s.SubmittedAt,
                FileUrl = s.FileUrl
            }).ToList();
        }

        public async Task<SubmissionDetailDto?> GetSubmissionDetailAsync(Guid submissionId, CancellationToken ct = default)
        {
            var submission = await _submissionRepo.GetWithTaskAsync(submissionId, ct);
            if (submission == null) return null;

            return new SubmissionDetailDto
            {
                SubmissionId = submission.SubmissionId,
                TaskId = submission.TaskId,
                TaskTitle = submission.Task?.Title ?? "",
                AssistantName = submission.Task?.AssignedToAssistant?.FullName ?? "",
                SubmittedAt = submission.SubmittedAt,
                FileUrl = submission.FileUrl,
                Notes = submission.Comment,
                StatusCode = submission.SubmissionStatus.StatusCode
            };
        }

        public async Task ReviewSubmissionAsync(ReviewSubmissionDto dto, Guid mangakaId, CancellationToken ct = default)
        {
            var submission = await _submissionRepo.GetWithTaskAsync(dto.SubmissionId, ct);
            if (submission == null) return;

            submission.SubmissionStatus.StatusCode = dto.Decision == "Approved" ? "Approved" : 
                                    dto.Decision == "Rejected" ? "Rejected" : "RevisionRequired";
            submission.ReviewedAt = DateTime.UtcNow;
            submission.ReviewedByMangakaId = mangakaId;
            submission.ReviewNote = dto.Reason;

            await _submissionRepo.UpdateAsync(submission, ct);

            string taskStatus = dto.Decision == "Approved" ? "Approved" :
                                dto.Decision == "Rejected" ? "Rejected" : "RevisionRequired";
            
            await _taskRepo.UpdateStatusAsync(submission.TaskId, taskStatus, ct);

            // Notify Assistant
            await _notificationRepo.AddAsync(new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = submission.SubmittedByAssistantId,
                NotificationTypeId = 1,
                Title = "Submission Reviewed",
                Message = $"Review completed. Decision: {dto.Decision}",
                ReferenceType = "TaskSubmission",
                ReferenceId = submission.SubmissionId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            }, ct);
        }
    }
}






