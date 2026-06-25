using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class User
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? AvatarUrl { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AiSegmentationJob> AiSegmentationJobs { get; set; } = new List<AiSegmentationJob>();

    public virtual ICollection<AssistantEarning> AssistantEarnings { get; set; } = new List<AssistantEarning>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<BoardVote> BoardVotes { get; set; } = new List<BoardVote>();

    public virtual ICollection<EditorComment> EditorCommentEditors { get; set; } = new List<EditorComment>();

    public virtual ICollection<EditorComment> EditorCommentResolvedByUsers { get; set; } = new List<EditorComment>();

    public virtual ICollection<MangaPage> MangaPages { get; set; } = new List<MangaPage>();

    public virtual ICollection<Manuscript> Manuscripts { get; set; } = new List<Manuscript>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PageRegion> PageRegions { get; set; } = new List<PageRegion>();

    public virtual ICollection<ProductionTask> ProductionTaskAssignedToAssistants { get; set; } = new List<ProductionTask>();

    public virtual ICollection<ProductionTask> ProductionTaskCreatedByMangakas { get; set; } = new List<ProductionTask>();

    public virtual ICollection<PublishingDecision> PublishingDecisions { get; set; } = new List<PublishingDecision>();

    public virtual ICollection<ReaderVoteDatum> ReaderVoteData { get; set; } = new List<ReaderVoteDatum>();

    public virtual ICollection<Series> SeriesMangakas { get; set; } = new List<Series>();

    public virtual ICollection<Series> SeriesTantouEditors { get; set; } = new List<Series>();

    public virtual ICollection<SeriesTeamMember> SeriesTeamMembers { get; set; } = new List<SeriesTeamMember>();

    public virtual ICollection<TaskSubmission> TaskSubmissionReviewedByMangakas { get; set; } = new List<TaskSubmission>();

    public virtual ICollection<TaskSubmission> TaskSubmissionSubmittedByAssistants { get; set; } = new List<TaskSubmission>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<WorkflowStatusHistory> WorkflowStatusHistories { get; set; } = new List<WorkflowStatusHistory>();
}
