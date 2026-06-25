using System;
using System.Collections.Generic;
using MangaWorkflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MangaWorkflow.Infrastructure.Persistence;

public partial class MangaWorkflowDbContext : DbContext
{
    public MangaWorkflowDbContext(DbContextOptions<MangaWorkflowDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AiSegmentationJob> AiSegmentationJobs { get; set; }

    public virtual DbSet<AssistantEarning> AssistantEarnings { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BackgroundJobLog> BackgroundJobLogs { get; set; }

    public virtual DbSet<BoardVote> BoardVotes { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<ChapterPublication> ChapterPublications { get; set; }

    public virtual DbSet<ChapterStatus> ChapterStatuses { get; set; }

    public virtual DbSet<CommentStatus> CommentStatuses { get; set; }

    public virtual DbSet<DecisionType> DecisionTypes { get; set; }

    public virtual DbSet<EarningStatus> EarningStatuses { get; set; }

    public virtual DbSet<EditorComment> EditorComments { get; set; }

    public virtual DbSet<MangaPage> MangaPages { get; set; }

    public virtual DbSet<Manuscript> Manuscripts { get; set; }

    public virtual DbSet<ManuscriptStatus> ManuscriptStatuses { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationType> NotificationTypes { get; set; }

    public virtual DbSet<PageRegion> PageRegions { get; set; }

    public virtual DbSet<PageStatus> PageStatuses { get; set; }

    public virtual DbSet<ProductionTask> ProductionTasks { get; set; }

    public virtual DbSet<PublicationIssue> PublicationIssues { get; set; }

    public virtual DbSet<PublicationSchedule> PublicationSchedules { get; set; }

    public virtual DbSet<PublishingDecision> PublishingDecisions { get; set; }

    public virtual DbSet<RankingRecord> RankingRecords { get; set; }

    public virtual DbSet<ReaderVoteDatum> ReaderVoteData { get; set; }

    public virtual DbSet<RegionType> RegionTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Series> Series { get; set; }

    public virtual DbSet<SeriesStatus> SeriesStatuses { get; set; }

    public virtual DbSet<SeriesTeamMember> SeriesTeamMembers { get; set; }

    public virtual DbSet<SubmissionStatus> SubmissionStatuses { get; set; }

    public virtual DbSet<MangaWorkflow.Domain.Entities.TaskStatus> TaskStatuses { get; set; }

    public virtual DbSet<TaskSubmission> TaskSubmissions { get; set; }

    public virtual DbSet<TaskType> TaskTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<VoteValue> VoteValues { get; set; }

    public virtual DbSet<VwAssistantMonthlyEarning> VwAssistantMonthlyEarnings { get; set; }

    public virtual DbSet<VwChapterProgress> VwChapterProgresses { get; set; }

    public virtual DbSet<VwSeriesLatestRanking> VwSeriesLatestRankings { get; set; }

    public virtual DbSet<WorkflowStatusHistory> WorkflowStatusHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AiSegmentationJob>(entity =>
        {
            entity.HasKey(e => e.AiSegmentationJobId).HasName("PK__AiSegmen__54D3B6F16D79FBEB");

            entity.HasIndex(e => new { e.PageId, e.Status }, "IX_AiSegmentationJobs_Page_Status");

            entity.Property(e => e.AiSegmentationJobId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ModelName).HasMaxLength(150);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Queued");

            entity.HasOne(d => d.Page).WithMany(p => p.AiSegmentationJobs)
                .HasForeignKey(d => d.PageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AiSegmentationJobs_Page");

            entity.HasOne(d => d.RequestedByUser).WithMany(p => p.AiSegmentationJobs)
                .HasForeignKey(d => d.RequestedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AiSegmentationJobs_RequestedBy");
        });

        modelBuilder.Entity<AssistantEarning>(entity =>
        {
            entity.HasKey(e => e.EarningId).HasName("PK__Assistan__2418A122A323D5C4");

            entity.HasIndex(e => e.AssistantId, "IX_AssistantEarnings_Assistant");

            entity.HasIndex(e => e.EarningStatusId, "IX_AssistantEarnings_Status");

            entity.HasIndex(e => e.TaskId, "UQ_AssistantEarnings_Task").IsUnique();

            entity.Property(e => e.EarningId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.CalculatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Assistant).WithMany(p => p.AssistantEarnings)
                .HasForeignKey(d => d.AssistantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssistantEarnings_Assistant");

            entity.HasOne(d => d.EarningStatus).WithMany(p => p.AssistantEarnings)
                .HasForeignKey(d => d.EarningStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssistantEarnings_Status");

            entity.HasOne(d => d.Task).WithOne(p => p.AssistantEarning)
                .HasForeignKey<AssistantEarning>(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssistantEarnings_Task");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId).HasName("PK__AuditLog__EB5F6CBD04838B7F");

            entity.HasIndex(e => e.ActorUserId, "IX_AuditLogs_ActorUserId");

            entity.HasIndex(e => new { e.EntityName, e.EntityId }, "IX_AuditLogs_Entity");

            entity.Property(e => e.AuditLogId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ActionName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.EntityName).HasMaxLength(100);

            entity.HasOne(d => d.ActorUser).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.ActorUserId)
                .HasConstraintName("FK_AuditLogs_Actor");
        });

        modelBuilder.Entity<BackgroundJobLog>(entity =>
        {
            entity.HasKey(e => e.JobLogId).HasName("PK__Backgrou__2B515D3E41DF7824");

            entity.HasIndex(e => new { e.JobName, e.StartedAt }, "IX_BackgroundJobLogs_JobName_StartedAt");

            entity.Property(e => e.JobLogId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.JobName).HasMaxLength(150);
            entity.Property(e => e.StartedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<BoardVote>(entity =>
        {
            entity.HasKey(e => e.VoteId).HasName("PK__BoardVot__52F015C2EB7CCEE7");

            entity.HasIndex(e => e.BoardMemberId, "IX_BoardVotes_BoardMember");

            entity.HasIndex(e => e.SeriesId, "IX_BoardVotes_SeriesId");

            entity.HasIndex(e => new { e.SeriesId, e.BoardMemberId }, "UQ_BoardVotes_Series_BoardMember").IsUnique();

            entity.Property(e => e.VoteId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.BoardMember).WithMany(p => p.BoardVotes)
                .HasForeignKey(d => d.BoardMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BoardVotes_BoardMember");

            entity.HasOne(d => d.Series).WithMany(p => p.BoardVotes)
                .HasForeignKey(d => d.SeriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BoardVotes_Series");

            entity.HasOne(d => d.VoteValue).WithMany(p => p.BoardVotes)
                .HasForeignKey(d => d.VoteValueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BoardVotes_Value");
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(e => e.ChapterId).HasName("PK__Chapters__0893A36A8F89CABA");

            entity.HasIndex(e => e.SeriesId, "IX_Chapters_SeriesId");

            entity.HasIndex(e => new { e.ChapterStatusId, e.Deadline }, "IX_Chapters_Status_Deadline");

            entity.HasIndex(e => new { e.SeriesId, e.ChapterNumber }, "UQ_Chapters_Series_ChapterNumber").IsUnique();

            entity.Property(e => e.ChapterId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.ChapterStatus).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.ChapterStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chapters_Status");

            entity.HasOne(d => d.Series).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.SeriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chapters_Series");
        });

        modelBuilder.Entity<ChapterPublication>(entity =>
        {
            entity.HasKey(e => e.ChapterPublicationId).HasName("PK__ChapterP__69FC20559CC4262D");

            entity.HasIndex(e => e.PublicationIssueId, "IX_ChapterPublications_Issue");

            entity.HasIndex(e => e.ChapterId, "UQ_ChapterPublications_Chapter").IsUnique();

            entity.Property(e => e.ChapterPublicationId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.PublicationOrder).HasDefaultValue(1);

            entity.HasOne(d => d.Chapter).WithOne(p => p.ChapterPublication)
                .HasForeignKey<ChapterPublication>(d => d.ChapterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChapterPublications_Chapter");

            entity.HasOne(d => d.PublicationIssue).WithMany(p => p.ChapterPublications)
                .HasForeignKey(d => d.PublicationIssueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChapterPublications_Issue");
        });

        modelBuilder.Entity<ChapterStatus>(entity =>
        {
            entity.HasKey(e => e.ChapterStatusId).HasName("PK__ChapterS__B2A1BE0EB1DB4F66");

            entity.HasIndex(e => e.StatusCode, "UQ__ChapterS__6A7B44FC95B17BBA").IsUnique();

            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StatusName).HasMaxLength(100);
        });

        modelBuilder.Entity<CommentStatus>(entity =>
        {
            entity.HasKey(e => e.CommentStatusId).HasName("PK__CommentS__F5BF43926A99BCFC");

            entity.HasIndex(e => e.StatusCode, "UQ__CommentS__6A7B44FC47E16D6A").IsUnique();

            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StatusName).HasMaxLength(100);
        });

        modelBuilder.Entity<DecisionType>(entity =>
        {
            entity.HasKey(e => e.DecisionTypeId).HasName("PK__Decision__5765D7FDBD6EC4E8");

            entity.HasIndex(e => e.DecisionCode, "UQ__Decision__249613CF8D71C492").IsUnique();

            entity.Property(e => e.DecisionCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DecisionName).HasMaxLength(100);
        });

        modelBuilder.Entity<EarningStatus>(entity =>
        {
            entity.HasKey(e => e.EarningStatusId).HasName("PK__EarningS__21A1CC75EB135728");

            entity.HasIndex(e => e.StatusCode, "UQ__EarningS__6A7B44FCD65FF8D5").IsUnique();

            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StatusName).HasMaxLength(100);
        });

        modelBuilder.Entity<EditorComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__EditorCo__C3B4DFCAC8ED845C");

            entity.HasIndex(e => e.EditorId, "IX_EditorComments_Editor");

            entity.HasIndex(e => e.PageId, "IX_EditorComments_PageId");

            entity.HasIndex(e => e.CommentStatusId, "IX_EditorComments_Status");

            entity.Property(e => e.CommentId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.X).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Y).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.CommentStatus).WithMany(p => p.EditorComments)
                .HasForeignKey(d => d.CommentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EditorComments_Status");

            entity.HasOne(d => d.Editor).WithMany(p => p.EditorCommentEditors)
                .HasForeignKey(d => d.EditorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EditorComments_Editor");

            entity.HasOne(d => d.Page).WithMany(p => p.EditorComments)
                .HasForeignKey(d => d.PageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EditorComments_Page");

            entity.HasOne(d => d.ResolvedByUser).WithMany(p => p.EditorCommentResolvedByUsers)
                .HasForeignKey(d => d.ResolvedByUserId)
                .HasConstraintName("FK_EditorComments_ResolvedBy");
        });

        modelBuilder.Entity<MangaPage>(entity =>
        {
            entity.HasKey(e => e.PageId).HasName("PK__MangaPag__C565B1040442FEFC");

            entity.HasIndex(e => e.ChapterId, "IX_MangaPages_ChapterId");

            entity.HasIndex(e => e.PageStatusId, "IX_MangaPages_Status");

            entity.HasIndex(e => new { e.ChapterId, e.PageNumber, e.VersionNo }, "UQ_MangaPages_Chapter_Page_Version").IsUnique();

            entity.Property(e => e.PageId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(1000);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.VersionNo).HasDefaultValue(1);

            entity.HasOne(d => d.Chapter).WithMany(p => p.MangaPages)
                .HasForeignKey(d => d.ChapterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MangaPages_Chapters");

            entity.HasOne(d => d.PageStatus).WithMany(p => p.MangaPages)
                .HasForeignKey(d => d.PageStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MangaPages_Status");

            entity.HasOne(d => d.UploadedByUser).WithMany(p => p.MangaPages)
                .HasForeignKey(d => d.UploadedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MangaPages_UploadedBy");
        });

        modelBuilder.Entity<Manuscript>(entity =>
        {
            entity.HasKey(e => e.ManuscriptId).HasName("PK__Manuscri__1631BEA50B4BAC4B");

            entity.HasIndex(e => e.ChapterId, "IX_Manuscripts_ChapterId");

            entity.HasIndex(e => e.SeriesId, "IX_Manuscripts_SeriesId");

            entity.Property(e => e.ManuscriptId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FileUrl).HasMaxLength(1000);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.VersionNo).HasDefaultValue(1);

            entity.HasOne(d => d.Chapter).WithMany(p => p.Manuscripts)
                .HasForeignKey(d => d.ChapterId)
                .HasConstraintName("FK_Manuscripts_Chapters");

            entity.HasOne(d => d.ManuscriptStatus).WithMany(p => p.Manuscripts)
                .HasForeignKey(d => d.ManuscriptStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Manuscripts_Status");

            entity.HasOne(d => d.Series).WithMany(p => p.Manuscripts)
                .HasForeignKey(d => d.SeriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Manuscripts_Series");

            entity.HasOne(d => d.UploadedByUser).WithMany(p => p.Manuscripts)
                .HasForeignKey(d => d.UploadedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Manuscripts_UploadedBy");
        });

        modelBuilder.Entity<ManuscriptStatus>(entity =>
        {
            entity.HasKey(e => e.ManuscriptStatusId).HasName("PK__Manuscri__A2D0A193FE188F3B");

            entity.HasIndex(e => e.StatusCode, "UQ__Manuscri__6A7B44FC1E285F81").IsUnique();

            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StatusName).HasMaxLength(100);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E12E002E6C6");

            entity.HasIndex(e => e.CreatedAt, "IX_Notifications_CreatedAt");

            entity.HasIndex(e => new { e.UserId, e.IsRead }, "IX_Notifications_User_IsRead");

            entity.Property(e => e.NotificationId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ReferenceType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.NotificationType).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.NotificationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_Type");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_User");
        });

        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.HasKey(e => e.NotificationTypeId).HasName("PK__Notifica__299002C120684B68");

            entity.HasIndex(e => e.TypeCode, "UQ__Notifica__3E1CDC7C7D7D5E14").IsUnique();

            entity.Property(e => e.TypeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<PageRegion>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("PK__PageRegi__ACD844A3D4C17D0D");

            entity.HasIndex(e => e.PageId, "IX_PageRegions_PageId");

            entity.HasIndex(e => e.RegionTypeId, "IX_PageRegions_RegionType");

            entity.Property(e => e.RegionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Confidence).HasColumnType("decimal(5, 4)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Height).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Label).HasMaxLength(150);
            entity.Property(e => e.SourceType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Manual");
            entity.Property(e => e.Width).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.X).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Y).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.PageRegions)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PageRegions_CreatedBy");

            entity.HasOne(d => d.Page).WithMany(p => p.PageRegions)
                .HasForeignKey(d => d.PageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PageRegions_Pages");

            entity.HasOne(d => d.RegionType).WithMany(p => p.PageRegions)
                .HasForeignKey(d => d.RegionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PageRegions_RegionTypes");
        });

        modelBuilder.Entity<PageStatus>(entity =>
        {
            entity.HasKey(e => e.PageStatusId).HasName("PK__PageStat__4D4BC8309BD32B74");

            entity.HasIndex(e => e.StatusCode, "UQ__PageStat__6A7B44FC292A59C0").IsUnique();

            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StatusName).HasMaxLength(100);
        });

        modelBuilder.Entity<ProductionTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Producti__7C6949B127124670");

            entity.HasIndex(e => new { e.AssignedToAssistantId, e.TaskStatusId }, "IX_ProductionTasks_Assistant_Status");

            entity.HasIndex(e => e.Deadline, "IX_ProductionTasks_Deadline");

            entity.HasIndex(e => e.CreatedByMangakaId, "IX_ProductionTasks_Mangaka");

            entity.HasIndex(e => e.PageId, "IX_ProductionTasks_PageId");

            entity.HasIndex(e => e.RegionId, "IX_ProductionTasks_RegionId");

            entity.Property(e => e.TaskId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Price).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Priority).HasDefaultValue(2);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.AssignedToAssistant).WithMany(p => p.ProductionTaskAssignedToAssistants)
                .HasForeignKey(d => d.AssignedToAssistantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionTasks_Assistant");

            entity.HasOne(d => d.CreatedByMangaka).WithMany(p => p.ProductionTaskCreatedByMangakas)
                .HasForeignKey(d => d.CreatedByMangakaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionTasks_Mangaka");

            entity.HasOne(d => d.Page).WithMany(p => p.ProductionTasks)
                .HasForeignKey(d => d.PageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionTasks_Page");

            entity.HasOne(d => d.Region).WithMany(p => p.ProductionTasks)
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_ProductionTasks_Region");

            entity.HasOne(d => d.TaskStatus).WithMany(p => p.ProductionTasks)
                .HasForeignKey(d => d.TaskStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionTasks_Status");

            entity.HasOne(d => d.TaskType).WithMany(p => p.ProductionTasks)
                .HasForeignKey(d => d.TaskTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductionTasks_TaskType");
        });

        modelBuilder.Entity<PublicationIssue>(entity =>
        {
            entity.HasKey(e => e.PublicationIssueId).HasName("PK__Publicat__7C6FC8AE219BED06");

            entity.HasIndex(e => e.PublishedDate, "IX_PublicationIssues_PublishedDate");

            entity.HasIndex(e => e.IssueNumber, "UQ__Publicat__5703F26C6F615BDD").IsUnique();

            entity.Property(e => e.PublicationIssueId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IssueNumber).HasMaxLength(50);
            entity.Property(e => e.IssueTitle).HasMaxLength(200);
        });

        modelBuilder.Entity<PublicationSchedule>(entity =>
        {
            entity.HasKey(e => e.PublicationScheduleId).HasName("PK__Publicat__3CBCB454CA461818");

            entity.HasIndex(e => e.ScheduleCode, "UQ__Publicat__D559BD20CAA3CD46").IsUnique();

            entity.Property(e => e.ScheduleCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ScheduleName).HasMaxLength(100);
        });

        modelBuilder.Entity<PublishingDecision>(entity =>
        {
            entity.HasKey(e => e.DecisionId).HasName("PK__Publishi__C0F28986137DD8CD");

            entity.Property(e => e.DecisionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.DecidedByUser).WithMany(p => p.PublishingDecisions)
                .HasForeignKey(d => d.DecidedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PublishingDecisions_User");

            entity.HasOne(d => d.DecisionType).WithMany(p => p.PublishingDecisions)
                .HasForeignKey(d => d.DecisionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PublishingDecisions_DecisionType");

            entity.HasOne(d => d.Series).WithMany(p => p.PublishingDecisions)
                .HasForeignKey(d => d.SeriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PublishingDecisions_Series");
        });

        modelBuilder.Entity<RankingRecord>(entity =>
        {
            entity.HasKey(e => e.RankingRecordId).HasName("PK__RankingR__6F8C5D46BDFB2066");

            entity.HasIndex(e => new { e.SeriesId, e.IssueNumber }, "IX_RankingRecords_Series_Issue");

            entity.Property(e => e.RankingRecordId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CalculatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IssueNumber).HasMaxLength(50);
            entity.Property(e => e.Trend)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Stable");

            entity.HasOne(d => d.Series).WithMany(p => p.RankingRecords)
                .HasForeignKey(d => d.SeriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RankingRecords_Series");
        });

        modelBuilder.Entity<ReaderVoteDatum>(entity =>
        {
            entity.HasKey(e => e.ReaderVoteDataId).HasName("PK__ReaderVo__2EDBA43EAF1314FD");

            entity.HasIndex(e => new { e.SeriesId, e.IssueNumber }, "IX_ReaderVoteData_Series_Issue");

            entity.HasIndex(e => new { e.SeriesId, e.IssueNumber }, "UQ_ReaderVoteData_Series_Issue").IsUnique();

            entity.Property(e => e.ReaderVoteDataId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ImportedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IssueNumber).HasMaxLength(50);

            entity.HasOne(d => d.ImportedByUser).WithMany(p => p.ReaderVoteData)
                .HasForeignKey(d => d.ImportedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReaderVoteData_ImportedBy");

            entity.HasOne(d => d.Series).WithMany(p => p.ReaderVoteData)
                .HasForeignKey(d => d.SeriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReaderVoteData_Series");
        });

        modelBuilder.Entity<RegionType>(entity =>
        {
            entity.HasKey(e => e.RegionTypeId).HasName("PK__RegionTy__E7CC170B1F8CF245");

            entity.HasIndex(e => e.TypeCode, "UQ__RegionTy__3E1CDC7C7B008D38").IsUnique();

            entity.Property(e => e.TypeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A404930BC");

            entity.HasIndex(e => e.RoleCode, "UQ__Roles__D62CB59CB6589832").IsUnique();

            entity.Property(e => e.RoleCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<Series>(entity =>
        {
            entity.HasKey(e => e.SeriesId).HasName("PK__Series__F3A1C16127422291");

            entity.HasIndex(e => e.CreatedAt, "IX_Series_CreatedAt");

            entity.HasIndex(e => e.MangakaId, "IX_Series_MangakaId");

            entity.HasIndex(e => e.SeriesStatusId, "IX_Series_Status");

            entity.HasIndex(e => e.TantouEditorId, "IX_Series_TantouEditorId");

            entity.Property(e => e.SeriesId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AlternativeTitle).HasMaxLength(200);
            entity.Property(e => e.CancellationRiskScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.CoverImageUrl).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Genre).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Mangaka).WithMany(p => p.SeriesMangakas)
                .HasForeignKey(d => d.MangakaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Series_Mangaka");

            entity.HasOne(d => d.PublicationSchedule).WithMany(p => p.Series)
                .HasForeignKey(d => d.PublicationScheduleId)
                .HasConstraintName("FK_Series_PublicationSchedule");

            entity.HasOne(d => d.SeriesStatus).WithMany(p => p.Series)
                .HasForeignKey(d => d.SeriesStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Series_Status");

            entity.HasOne(d => d.TantouEditor).WithMany(p => p.SeriesTantouEditors)
                .HasForeignKey(d => d.TantouEditorId)
                .HasConstraintName("FK_Series_TantouEditor");
        });

        modelBuilder.Entity<SeriesStatus>(entity =>
        {
            entity.HasKey(e => e.SeriesStatusId).HasName("PK__SeriesSt__37DCD1A7C97D37BF");

            entity.HasIndex(e => e.StatusCode, "UQ__SeriesSt__6A7B44FCB5D2FB1D").IsUnique();

            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StatusName).HasMaxLength(100);
        });

        modelBuilder.Entity<SeriesTeamMember>(entity =>
        {
            entity.HasKey(e => e.SeriesTeamMemberId).HasName("PK__SeriesTe__24FE730440A7041E");

            entity.HasIndex(e => e.SeriesId, "IX_SeriesTeamMembers_SeriesId");

            entity.HasIndex(e => e.UserId, "IX_SeriesTeamMembers_UserId");

            entity.HasIndex(e => new { e.SeriesId, e.UserId, e.RoleInSeries }, "UQ_SeriesTeamMembers_Series_User_Role").IsUnique();

            entity.Property(e => e.SeriesTeamMemberId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.RoleInSeries).HasMaxLength(100);

            entity.HasOne(d => d.Series).WithMany(p => p.SeriesTeamMembers)
                .HasForeignKey(d => d.SeriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SeriesTeamMembers_Series");

            entity.HasOne(d => d.User).WithMany(p => p.SeriesTeamMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SeriesTeamMembers_User");
        });

        modelBuilder.Entity<SubmissionStatus>(entity =>
        {
            entity.HasKey(e => e.SubmissionStatusId).HasName("PK__Submissi__FC996AB144464C87");

            entity.HasIndex(e => e.StatusCode, "UQ__Submissi__6A7B44FC82F5A78B").IsUnique();

            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StatusName).HasMaxLength(100);
        });

        modelBuilder.Entity<MangaWorkflow.Domain.Entities.TaskStatus>(entity =>
        {
            entity.HasKey(e => e.TaskStatusId).HasName("PK__TaskStat__C023DD6C9ACF3287");

            entity.HasIndex(e => e.StatusCode, "UQ__TaskStat__6A7B44FCDD2E7CE8").IsUnique();

            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StatusName).HasMaxLength(100);
        });

        modelBuilder.Entity<TaskSubmission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__TaskSubm__449EE125CCCAA367");

            entity.HasIndex(e => e.SubmittedByAssistantId, "IX_TaskSubmissions_Assistant");

            entity.HasIndex(e => e.SubmissionStatusId, "IX_TaskSubmissions_Status");

            entity.HasIndex(e => e.TaskId, "IX_TaskSubmissions_TaskId");

            entity.Property(e => e.SubmissionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FileUrl).HasMaxLength(1000);
            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.ReviewedByMangaka).WithMany(p => p.TaskSubmissionReviewedByMangakas)
                .HasForeignKey(d => d.ReviewedByMangakaId)
                .HasConstraintName("FK_TaskSubmissions_ReviewedBy");

            entity.HasOne(d => d.SubmissionStatus).WithMany(p => p.TaskSubmissions)
                .HasForeignKey(d => d.SubmissionStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskSubmissions_Status");

            entity.HasOne(d => d.SubmittedByAssistant).WithMany(p => p.TaskSubmissionSubmittedByAssistants)
                .HasForeignKey(d => d.SubmittedByAssistantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskSubmissions_Assistant");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskSubmissions)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TaskSubmissions_Task");
        });

        modelBuilder.Entity<TaskType>(entity =>
        {
            entity.HasKey(e => e.TaskTypeId).HasName("PK__TaskType__66B23E33D101D6DA");

            entity.HasIndex(e => e.TypeCode, "UQ__TaskType__3E1CDC7C06E00E52").IsUnique();

            entity.Property(e => e.TypeCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C6B41F947");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534A12D8FD7").IsUnique();

            entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AvatarUrl).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.HasIndex(e => e.RoleId, "IX_UserRoles_RoleId");

            entity.Property(e => e.AssignedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRoles_Users");
        });

        modelBuilder.Entity<VoteValue>(entity =>
        {
            entity.HasKey(e => e.VoteValueId).HasName("PK__VoteValu__172E0E9A8748724B");

            entity.HasIndex(e => e.VoteCode, "UQ__VoteValu__BF3CE1574DD5D313").IsUnique();

            entity.Property(e => e.VoteCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.VoteName).HasMaxLength(100);
        });

        modelBuilder.Entity<VwAssistantMonthlyEarning>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_AssistantMonthlyEarnings");

            entity.Property(e => e.AssistantName).HasMaxLength(150);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(38, 2)");
        });

        modelBuilder.Entity<VwChapterProgress>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_ChapterProgress");

            entity.Property(e => e.ChapterStatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ChapterTitle).HasMaxLength(200);
            entity.Property(e => e.ProgressPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.SeriesTitle).HasMaxLength(200);
        });

        modelBuilder.Entity<VwSeriesLatestRanking>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_SeriesLatestRanking");

            entity.Property(e => e.CancellationRiskScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.IssueNumber).HasMaxLength(50);
            entity.Property(e => e.SeriesTitle).HasMaxLength(200);
            entity.Property(e => e.Trend)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WorkflowStatusHistory>(entity =>
        {
            entity.HasKey(e => e.WorkflowHistoryId).HasName("PK__Workflow__4469D555FB6C23D8");

            entity.HasIndex(e => e.ChangedAt, "IX_WorkflowStatusHistories_ChangedAt");

            entity.HasIndex(e => new { e.EntityName, e.EntityId }, "IX_WorkflowStatusHistories_Entity");

            entity.Property(e => e.WorkflowHistoryId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ChangedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.EntityName).HasMaxLength(100);
            entity.Property(e => e.FromStatusCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ToStatusCode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ChangedByUser).WithMany(p => p.WorkflowStatusHistories)
                .HasForeignKey(d => d.ChangedByUserId)
                .HasConstraintName("FK_WorkflowStatusHistories_ChangedBy");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
