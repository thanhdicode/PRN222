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
        private readonly ISubmissionStatusRepository _submissionStatusRepo;
        private readonly INotificationTypeRepository _notificationTypeRepo;

        public SubmissionService(
            ITaskSubmissionRepository submissionRepo,
            IProductionTaskRepository taskRepo,
            INotificationRepository notificationRepo,
            ISubmissionStatusRepository submissionStatusRepo,
            INotificationTypeRepository notificationTypeRepo)
        {
            _submissionRepo = submissionRepo;
            _taskRepo = taskRepo;
            _notificationRepo = notificationRepo;
            _submissionStatusRepo = submissionStatusRepo;
            _notificationTypeRepo = notificationTypeRepo;
        }

        public async Task SubmitTaskAsync(SubmitTaskDto dto, Guid assistantId, CancellationToken ct = default)
        {
            var task = await _taskRepo.GetWithDetailsAsync(dto.TaskId, ct);
            if (task == null || task.AssignedToAssistantId != assistantId) return;

            // Resolve SubmissionStatus by StatusCode — never hardcode IDs
            var submittedStatusId = await _submissionStatusRepo.GetIdByCodeAsync("Submitted", ct);

            var submission = new TaskSubmission
            {
                SubmissionId = Guid.NewGuid(),
                TaskId = dto.TaskId,
                SubmittedByAssistantId = assistantId,
                FileUrl = dto.FileUrl ?? string.Empty,
                Comment = dto.Notes,
                SubmissionStatusId = submittedStatusId,
                SubmittedAt = DateTime.UtcNow
            };

            await _submissionRepo.AddAsync(submission, ct);
            await _taskRepo.UpdateStatusAsync(dto.TaskId, "Submitted", ct);

            // Notify Mangaka: prefer Series.MangakaId, fallback to SeriesTeamMembers role
            var series = task.Page?.Chapter?.Series;
            Guid? mangakaId = null;
            if (series != null)
            {
                if (series.MangakaId != Guid.Empty)
                    mangakaId = series.MangakaId;
                else
                    mangakaId = series.SeriesTeamMembers
                        .FirstOrDefault(tm => tm.RoleInSeries.Contains("Mangaka"))?.UserId;
            }

            if (mangakaId.HasValue)
            {
                var notifTypeId = await _notificationTypeRepo.GetIdByCodeAsync("SubmissionUploaded", ct);
                if (notifTypeId.HasValue)
                {
                    await _notificationRepo.AddAsync(new Notification
                    {
                        NotificationId = Guid.NewGuid(),
                        UserId = mangakaId.Value,
                        NotificationTypeId = notifTypeId.Value,
                        Title = "New Submission",
                        Message = $"Task \"{task.Title}\" was submitted by an assistant.",
                        ReferenceType = "TaskSubmission",
                        ReferenceId = submission.SubmissionId,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    }, ct);
                }
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
                StatusCode = submission.SubmissionStatus?.StatusCode ?? ""
            };
        }

        public async Task ReviewSubmissionAsync(ReviewSubmissionDto dto, Guid mangakaId, CancellationToken ct = default)
        {
            var submission = await _submissionRepo.GetWithTaskAsync(dto.SubmissionId, ct);
            if (submission == null) return;

            // Map decision → StatusCode
            string newStatusCode = dto.Decision switch
            {
                "Approved" => "Approved",
                "Rejected" => "Rejected",
                _ => "RevisionRequired"
            };

            // Resolve SubmissionStatus by StatusCode — never mutate navigation property
            var newStatusId = await _submissionStatusRepo.GetIdByCodeAsync(newStatusCode, ct);
            submission.SubmissionStatusId = newStatusId;
            submission.ReviewedAt = DateTime.UtcNow;
            submission.ReviewedByMangakaId = mangakaId;
            submission.ReviewNote = dto.Reason;

            await _submissionRepo.UpdateAsync(submission, ct);

            string taskStatusCode = dto.Decision switch
            {
                "Approved" => "Approved",
                "Rejected" => "Rejected",
                _ => "RevisionRequired"
            };
            await _taskRepo.UpdateStatusAsync(submission.TaskId, taskStatusCode, ct);

            // Notify assistant about the review outcome
            var notifTypeId = await _notificationTypeRepo.GetIdByCodeAsync("SubmissionReviewed", ct);
            if (notifTypeId.HasValue)
            {
                await _notificationRepo.AddAsync(new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = submission.SubmittedByAssistantId,
                    NotificationTypeId = notifTypeId.Value,
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
}
