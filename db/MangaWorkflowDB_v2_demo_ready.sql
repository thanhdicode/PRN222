
/*
PRN222 - Manga Creation Workflow and Publishing Management System
Database Script v2.0 - Demo-ready schema and seed data
Target: SQL Server 2019+
Recommended DB name: MangaWorkflowDB

How to run:
1. Open SQL Server Management Studio.
2. Connect to your local SQL Server.
3. Execute this script.
4. In ASP.NET Core, connect to database: MangaWorkflowDB.

Note:
- This script uses a custom Users/Roles model for PRN222-friendly implementation.
- If you later decide to use ASP.NET Core Identity, keep the domain tables and replace Users/Roles/UserRoles with Identity tables.
*/

USE master;
GO

IF DB_ID(N'MangaWorkflowDB') IS NULL
BEGIN
    CREATE DATABASE MangaWorkflowDB;
END
GO

USE MangaWorkflowDB;
GO

/* ============================================================
   1. CLEANUP FOR DEVELOPMENT RERUN
   ============================================================ */

IF OBJECT_ID('dbo.vw_SeriesLatestRanking', 'V') IS NOT NULL DROP VIEW dbo.vw_SeriesLatestRanking;
IF OBJECT_ID('dbo.vw_AssistantMonthlyEarnings', 'V') IS NOT NULL DROP VIEW dbo.vw_AssistantMonthlyEarnings;
IF OBJECT_ID('dbo.vw_ChapterProgress', 'V') IS NOT NULL DROP VIEW dbo.vw_ChapterProgress;
GO

DROP TABLE IF EXISTS dbo.AuditLogs;
DROP TABLE IF EXISTS dbo.WorkflowStatusHistories;
DROP TABLE IF EXISTS dbo.AiSegmentationJobs;
DROP TABLE IF EXISTS dbo.ChapterPublications;
DROP TABLE IF EXISTS dbo.PublicationIssues;
DROP TABLE IF EXISTS dbo.SeriesTeamMembers;
DROP TABLE IF EXISTS dbo.AssistantEarnings;
DROP TABLE IF EXISTS dbo.BackgroundJobLogs;
DROP TABLE IF EXISTS dbo.Notifications;
DROP TABLE IF EXISTS dbo.RankingRecords;
DROP TABLE IF EXISTS dbo.ReaderVoteData;
DROP TABLE IF EXISTS dbo.PublishingDecisions;
DROP TABLE IF EXISTS dbo.BoardVotes;
DROP TABLE IF EXISTS dbo.EditorComments;
DROP TABLE IF EXISTS dbo.TaskSubmissions;
DROP TABLE IF EXISTS dbo.ProductionTasks;
DROP TABLE IF EXISTS dbo.PageRegions;
DROP TABLE IF EXISTS dbo.MangaPages;
DROP TABLE IF EXISTS dbo.Manuscripts;
DROP TABLE IF EXISTS dbo.Chapters;
DROP TABLE IF EXISTS dbo.Series;
DROP TABLE IF EXISTS dbo.UserRoles;
DROP TABLE IF EXISTS dbo.Users;
DROP TABLE IF EXISTS dbo.Roles;

DROP TABLE IF EXISTS dbo.EarningStatuses;
DROP TABLE IF EXISTS dbo.NotificationTypes;
DROP TABLE IF EXISTS dbo.DecisionTypes;
DROP TABLE IF EXISTS dbo.VoteValues;
DROP TABLE IF EXISTS dbo.CommentStatuses;
DROP TABLE IF EXISTS dbo.SubmissionStatuses;
DROP TABLE IF EXISTS dbo.TaskStatuses;
DROP TABLE IF EXISTS dbo.TaskTypes;
DROP TABLE IF EXISTS dbo.RegionTypes;
DROP TABLE IF EXISTS dbo.PageStatuses;
DROP TABLE IF EXISTS dbo.ManuscriptStatuses;
DROP TABLE IF EXISTS dbo.ChapterStatuses;
DROP TABLE IF EXISTS dbo.PublicationSchedules;
DROP TABLE IF EXISTS dbo.SeriesStatuses;
GO

/* ============================================================
   2. LOOKUP / STATUS TABLES
   ============================================================ */

CREATE TABLE dbo.SeriesStatuses (
    SeriesStatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusCode VARCHAR(50) NOT NULL UNIQUE,
    StatusName NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.PublicationSchedules (
    PublicationScheduleId INT IDENTITY(1,1) PRIMARY KEY,
    ScheduleCode VARCHAR(50) NOT NULL UNIQUE,
    ScheduleName NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.ChapterStatuses (
    ChapterStatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusCode VARCHAR(50) NOT NULL UNIQUE,
    StatusName NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.ManuscriptStatuses (
    ManuscriptStatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusCode VARCHAR(50) NOT NULL UNIQUE,
    StatusName NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.PageStatuses (
    PageStatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusCode VARCHAR(50) NOT NULL UNIQUE,
    StatusName NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.RegionTypes (
    RegionTypeId INT IDENTITY(1,1) PRIMARY KEY,
    TypeCode VARCHAR(50) NOT NULL UNIQUE,
    TypeName NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.TaskTypes (
    TaskTypeId INT IDENTITY(1,1) PRIMARY KEY,
    TypeCode VARCHAR(50) NOT NULL UNIQUE,
    TypeName NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.TaskStatuses (
    TaskStatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusCode VARCHAR(50) NOT NULL UNIQUE,
    StatusName NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.SubmissionStatuses (
    SubmissionStatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusCode VARCHAR(50) NOT NULL UNIQUE,
    StatusName NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.CommentStatuses (
    CommentStatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusCode VARCHAR(50) NOT NULL UNIQUE,
    StatusName NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.VoteValues (
    VoteValueId INT IDENTITY(1,1) PRIMARY KEY,
    VoteCode VARCHAR(50) NOT NULL UNIQUE,
    VoteName NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.DecisionTypes (
    DecisionTypeId INT IDENTITY(1,1) PRIMARY KEY,
    DecisionCode VARCHAR(50) NOT NULL UNIQUE,
    DecisionName NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.NotificationTypes (
    NotificationTypeId INT IDENTITY(1,1) PRIMARY KEY,
    TypeCode VARCHAR(50) NOT NULL UNIQUE,
    TypeName NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.EarningStatuses (
    EarningStatusId INT IDENTITY(1,1) PRIMARY KEY,
    StatusCode VARCHAR(50) NOT NULL UNIQUE,
    StatusName NVARCHAR(100) NOT NULL
);
GO

/* ============================================================
   3. AUTHENTICATION / AUTHORIZATION TABLES
   ============================================================ */

CREATE TABLE dbo.Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleCode VARCHAR(50) NOT NULL UNIQUE,
    RoleName NVARCHAR(100) NOT NULL
);

CREATE TABLE dbo.Users (
    UserId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NULL,
    AvatarUrl NVARCHAR(1000) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL
);

CREATE TABLE dbo.UserRoles (
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleId INT NOT NULL,
    AssignedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId)
);
GO

/* ============================================================
   4. CORE DOMAIN TABLES
   ============================================================ */

CREATE TABLE dbo.Series (
    SeriesId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    AlternativeTitle NVARCHAR(200) NULL,
    Description NVARCHAR(MAX) NULL,
    Genre NVARCHAR(100) NULL,
    CoverImageUrl NVARCHAR(1000) NULL,

    MangakaId UNIQUEIDENTIFIER NOT NULL,
    TantouEditorId UNIQUEIDENTIFIER NULL,

    SeriesStatusId INT NOT NULL,
    PublicationScheduleId INT NULL,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    SubmittedAt DATETIME2 NULL,
    ApprovedAt DATETIME2 NULL,
    RejectedAt DATETIME2 NULL,
    CancelledAt DATETIME2 NULL,

    CancellationRiskScore DECIMAL(5,2) NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Series_Mangaka FOREIGN KEY (MangakaId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Series_TantouEditor FOREIGN KEY (TantouEditorId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Series_Status FOREIGN KEY (SeriesStatusId) REFERENCES dbo.SeriesStatuses(SeriesStatusId),
    CONSTRAINT FK_Series_PublicationSchedule FOREIGN KEY (PublicationScheduleId) REFERENCES dbo.PublicationSchedules(PublicationScheduleId),
    CONSTRAINT CK_Series_CancellationRiskScore CHECK (CancellationRiskScore >= 0 AND CancellationRiskScore <= 100)
);


CREATE TABLE dbo.SeriesTeamMembers (
    SeriesTeamMemberId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    SeriesId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleInSeries NVARCHAR(100) NOT NULL,
    JoinedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_SeriesTeamMembers_Series FOREIGN KEY (SeriesId) REFERENCES dbo.Series(SeriesId),
    CONSTRAINT FK_SeriesTeamMembers_User FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT UQ_SeriesTeamMembers_Series_User_Role UNIQUE (SeriesId, UserId, RoleInSeries)
);

CREATE TABLE dbo.PublicationIssues (
    PublicationIssueId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    IssueNumber NVARCHAR(50) NOT NULL UNIQUE,
    IssueTitle NVARCHAR(200) NULL,
    PublishedDate DATE NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE dbo.Chapters (
    ChapterId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    SeriesId UNIQUEIDENTIFIER NOT NULL,
    ChapterNumber INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Summary NVARCHAR(MAX) NULL,
    ChapterStatusId INT NOT NULL,
    Deadline DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CompletedAt DATETIME2 NULL,

    CONSTRAINT FK_Chapters_Series FOREIGN KEY (SeriesId) REFERENCES dbo.Series(SeriesId),
    CONSTRAINT FK_Chapters_Status FOREIGN KEY (ChapterStatusId) REFERENCES dbo.ChapterStatuses(ChapterStatusId),
    CONSTRAINT UQ_Chapters_Series_ChapterNumber UNIQUE (SeriesId, ChapterNumber),
    CONSTRAINT CK_Chapters_ChapterNumber CHECK (ChapterNumber > 0)
);


CREATE TABLE dbo.ChapterPublications (
    ChapterPublicationId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    ChapterId UNIQUEIDENTIFIER NOT NULL,
    PublicationIssueId UNIQUEIDENTIFIER NOT NULL,
    PublicationOrder INT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_ChapterPublications_Chapter FOREIGN KEY (ChapterId) REFERENCES dbo.Chapters(ChapterId),
    CONSTRAINT FK_ChapterPublications_Issue FOREIGN KEY (PublicationIssueId) REFERENCES dbo.PublicationIssues(PublicationIssueId),
    CONSTRAINT UQ_ChapterPublications_Chapter UNIQUE (ChapterId),
    CONSTRAINT CK_ChapterPublications_Order CHECK (PublicationOrder > 0)
);

CREATE TABLE dbo.Manuscripts (
    ManuscriptId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    SeriesId UNIQUEIDENTIFIER NOT NULL,
    ChapterId UNIQUEIDENTIFIER NULL,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    FileUrl NVARCHAR(1000) NOT NULL,
    FileName NVARCHAR(255) NULL,
    ContentType NVARCHAR(100) NULL,
    FileSizeBytes BIGINT NULL,
    VersionNo INT NOT NULL DEFAULT 1,
    ManuscriptStatusId INT NOT NULL,
    UploadedByUserId UNIQUEIDENTIFIER NOT NULL,
    UploadedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Manuscripts_Series FOREIGN KEY (SeriesId) REFERENCES dbo.Series(SeriesId),
    CONSTRAINT FK_Manuscripts_Chapters FOREIGN KEY (ChapterId) REFERENCES dbo.Chapters(ChapterId),
    CONSTRAINT FK_Manuscripts_Status FOREIGN KEY (ManuscriptStatusId) REFERENCES dbo.ManuscriptStatuses(ManuscriptStatusId),
    CONSTRAINT FK_Manuscripts_UploadedBy FOREIGN KEY (UploadedByUserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT CK_Manuscripts_VersionNo CHECK (VersionNo > 0),
    CONSTRAINT CK_Manuscripts_FileSize CHECK (FileSizeBytes IS NULL OR FileSizeBytes >= 0)
);

CREATE TABLE dbo.MangaPages (
    PageId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    ChapterId UNIQUEIDENTIFIER NOT NULL,
    PageNumber INT NOT NULL,
    ImageUrl NVARCHAR(1000) NOT NULL,
    ThumbnailUrl NVARCHAR(1000) NULL,
    FileName NVARCHAR(255) NULL,
    ContentType NVARCHAR(100) NULL,
    FileSizeBytes BIGINT NULL,
    VersionNo INT NOT NULL DEFAULT 1,
    PageStatusId INT NOT NULL,
    UploadedByUserId UNIQUEIDENTIFIER NOT NULL,
    UploadedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_MangaPages_Chapters FOREIGN KEY (ChapterId) REFERENCES dbo.Chapters(ChapterId),
    CONSTRAINT FK_MangaPages_Status FOREIGN KEY (PageStatusId) REFERENCES dbo.PageStatuses(PageStatusId),
    CONSTRAINT FK_MangaPages_UploadedBy FOREIGN KEY (UploadedByUserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT UQ_MangaPages_Chapter_Page_Version UNIQUE (ChapterId, PageNumber, VersionNo),
    CONSTRAINT CK_MangaPages_PageNumber CHECK (PageNumber > 0),
    CONSTRAINT CK_MangaPages_VersionNo CHECK (VersionNo > 0),
    CONSTRAINT CK_MangaPages_FileSize CHECK (FileSizeBytes IS NULL OR FileSizeBytes >= 0)
);

CREATE TABLE dbo.PageRegions (
    RegionId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    PageId UNIQUEIDENTIFIER NOT NULL,
    RegionTypeId INT NOT NULL,

    X DECIMAL(10,2) NOT NULL,
    Y DECIMAL(10,2) NOT NULL,
    Width DECIMAL(10,2) NOT NULL,
    Height DECIMAL(10,2) NOT NULL,

    Label NVARCHAR(150) NULL,
    Confidence DECIMAL(5,4) NULL,
    SourceType VARCHAR(20) NOT NULL DEFAULT 'Manual',
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_PageRegions_Pages FOREIGN KEY (PageId) REFERENCES dbo.MangaPages(PageId),
    CONSTRAINT FK_PageRegions_RegionTypes FOREIGN KEY (RegionTypeId) REFERENCES dbo.RegionTypes(RegionTypeId),
    CONSTRAINT FK_PageRegions_CreatedBy FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT CK_PageRegions_Coordinates CHECK (X >= 0 AND Y >= 0 AND Width > 0 AND Height > 0),
    CONSTRAINT CK_PageRegions_Confidence CHECK (Confidence IS NULL OR (Confidence >= 0 AND Confidence <= 1)),
    CONSTRAINT CK_PageRegions_SourceType CHECK (SourceType IN ('Manual', 'AI'))
);


CREATE TABLE dbo.AiSegmentationJobs (
    AiSegmentationJobId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    PageId UNIQUEIDENTIFIER NOT NULL,
    RequestedByUserId UNIQUEIDENTIFIER NOT NULL,
    ModelName NVARCHAR(150) NOT NULL,
    Status VARCHAR(30) NOT NULL DEFAULT 'Queued',
    RawResultJson NVARCHAR(MAX) NULL,
    ErrorMessage NVARCHAR(MAX) NULL,
    StartedAt DATETIME2 NULL,
    CompletedAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_AiSegmentationJobs_Page FOREIGN KEY (PageId) REFERENCES dbo.MangaPages(PageId),
    CONSTRAINT FK_AiSegmentationJobs_RequestedBy FOREIGN KEY (RequestedByUserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT CK_AiSegmentationJobs_Status CHECK (Status IN ('Queued', 'Running', 'Succeeded', 'Failed'))
);

CREATE TABLE dbo.ProductionTasks (
    TaskId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    PageId UNIQUEIDENTIFIER NOT NULL,
    RegionId UNIQUEIDENTIFIER NULL,
    AssignedToAssistantId UNIQUEIDENTIFIER NOT NULL,
    CreatedByMangakaId UNIQUEIDENTIFIER NOT NULL,

    TaskTypeId INT NOT NULL,
    TaskStatusId INT NOT NULL,

    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Priority INT NOT NULL DEFAULT 2,
    Deadline DATETIME2 NULL,
    Price DECIMAL(12,2) NOT NULL DEFAULT 0,

    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    StartedAt DATETIME2 NULL,
    CompletedAt DATETIME2 NULL,
    UpdatedAt DATETIME2 NULL,

    CONSTRAINT FK_ProductionTasks_Page FOREIGN KEY (PageId) REFERENCES dbo.MangaPages(PageId),
    CONSTRAINT FK_ProductionTasks_Region FOREIGN KEY (RegionId) REFERENCES dbo.PageRegions(RegionId),
    CONSTRAINT FK_ProductionTasks_Assistant FOREIGN KEY (AssignedToAssistantId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_ProductionTasks_Mangaka FOREIGN KEY (CreatedByMangakaId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_ProductionTasks_TaskType FOREIGN KEY (TaskTypeId) REFERENCES dbo.TaskTypes(TaskTypeId),
    CONSTRAINT FK_ProductionTasks_Status FOREIGN KEY (TaskStatusId) REFERENCES dbo.TaskStatuses(TaskStatusId),
    CONSTRAINT CK_ProductionTasks_Priority CHECK (Priority BETWEEN 1 AND 5),
    CONSTRAINT CK_ProductionTasks_Price CHECK (Price >= 0)
);

CREATE TABLE dbo.TaskSubmissions (
    SubmissionId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    TaskId UNIQUEIDENTIFIER NOT NULL,
    SubmittedByAssistantId UNIQUEIDENTIFIER NOT NULL,

    FileUrl NVARCHAR(1000) NOT NULL,
    FileName NVARCHAR(255) NULL,
    ContentType NVARCHAR(100) NULL,
    FileSizeBytes BIGINT NULL,
    Comment NVARCHAR(MAX) NULL,

    SubmissionStatusId INT NOT NULL,
    SubmittedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ReviewedAt DATETIME2 NULL,
    ReviewedByMangakaId UNIQUEIDENTIFIER NULL,
    ReviewNote NVARCHAR(MAX) NULL,

    CONSTRAINT FK_TaskSubmissions_Task FOREIGN KEY (TaskId) REFERENCES dbo.ProductionTasks(TaskId),
    CONSTRAINT FK_TaskSubmissions_Assistant FOREIGN KEY (SubmittedByAssistantId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_TaskSubmissions_ReviewedBy FOREIGN KEY (ReviewedByMangakaId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_TaskSubmissions_Status FOREIGN KEY (SubmissionStatusId) REFERENCES dbo.SubmissionStatuses(SubmissionStatusId),
    CONSTRAINT CK_TaskSubmissions_FileSize CHECK (FileSizeBytes IS NULL OR FileSizeBytes >= 0)
);

CREATE TABLE dbo.EditorComments (
    CommentId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    PageId UNIQUEIDENTIFIER NOT NULL,
    EditorId UNIQUEIDENTIFIER NOT NULL,

    X DECIMAL(10,2) NULL,
    Y DECIMAL(10,2) NULL,
    Width DECIMAL(10,2) NULL,
    Height DECIMAL(10,2) NULL,

    CommentText NVARCHAR(MAX) NOT NULL,
    CommentStatusId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ResolvedAt DATETIME2 NULL,
    ResolvedByUserId UNIQUEIDENTIFIER NULL,

    CONSTRAINT FK_EditorComments_Page FOREIGN KEY (PageId) REFERENCES dbo.MangaPages(PageId),
    CONSTRAINT FK_EditorComments_Editor FOREIGN KEY (EditorId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_EditorComments_ResolvedBy FOREIGN KEY (ResolvedByUserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_EditorComments_Status FOREIGN KEY (CommentStatusId) REFERENCES dbo.CommentStatuses(CommentStatusId),
    CONSTRAINT CK_EditorComments_Box CHECK (
        (X IS NULL AND Y IS NULL AND Width IS NULL AND Height IS NULL)
        OR
        (X >= 0 AND Y >= 0 AND Width > 0 AND Height > 0)
    )
);

CREATE TABLE dbo.BoardVotes (
    VoteId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    SeriesId UNIQUEIDENTIFIER NOT NULL,
    BoardMemberId UNIQUEIDENTIFIER NOT NULL,
    VoteValueId INT NOT NULL,
    Comment NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_BoardVotes_Series FOREIGN KEY (SeriesId) REFERENCES dbo.Series(SeriesId),
    CONSTRAINT FK_BoardVotes_BoardMember FOREIGN KEY (BoardMemberId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_BoardVotes_Value FOREIGN KEY (VoteValueId) REFERENCES dbo.VoteValues(VoteValueId),
    CONSTRAINT UQ_BoardVotes_Series_BoardMember UNIQUE (SeriesId, BoardMemberId)
);

CREATE TABLE dbo.PublishingDecisions (
    DecisionId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    SeriesId UNIQUEIDENTIFIER NOT NULL,
    DecisionTypeId INT NOT NULL,
    DecidedByUserId UNIQUEIDENTIFIER NOT NULL,
    Reason NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_PublishingDecisions_Series FOREIGN KEY (SeriesId) REFERENCES dbo.Series(SeriesId),
    CONSTRAINT FK_PublishingDecisions_DecisionType FOREIGN KEY (DecisionTypeId) REFERENCES dbo.DecisionTypes(DecisionTypeId),
    CONSTRAINT FK_PublishingDecisions_User FOREIGN KEY (DecidedByUserId) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.ReaderVoteData (
    ReaderVoteDataId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    SeriesId UNIQUEIDENTIFIER NOT NULL,
    IssueNumber NVARCHAR(50) NOT NULL,
    VoteCount INT NOT NULL,
    RankPosition INT NOT NULL,
    ImportedByUserId UNIQUEIDENTIFIER NOT NULL,
    ImportedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    Notes NVARCHAR(MAX) NULL,

    CONSTRAINT FK_ReaderVoteData_Series FOREIGN KEY (SeriesId) REFERENCES dbo.Series(SeriesId),
    CONSTRAINT FK_ReaderVoteData_ImportedBy FOREIGN KEY (ImportedByUserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT UQ_ReaderVoteData_Series_Issue UNIQUE (SeriesId, IssueNumber),
    CONSTRAINT CK_ReaderVoteData_VoteCount CHECK (VoteCount >= 0),
    CONSTRAINT CK_ReaderVoteData_RankPosition CHECK (RankPosition > 0)
);

CREATE TABLE dbo.RankingRecords (
    RankingRecordId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    SeriesId UNIQUEIDENTIFIER NOT NULL,
    IssueNumber NVARCHAR(50) NOT NULL,
    VoteCount INT NOT NULL,
    RankPosition INT NOT NULL,
    PreviousRankPosition INT NULL,
    Trend VARCHAR(20) NOT NULL DEFAULT 'Stable',
    CalculatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_RankingRecords_Series FOREIGN KEY (SeriesId) REFERENCES dbo.Series(SeriesId),
    CONSTRAINT CK_RankingRecords_VoteCount CHECK (VoteCount >= 0),
    CONSTRAINT CK_RankingRecords_Rank CHECK (RankPosition > 0),
    CONSTRAINT CK_RankingRecords_PreviousRank CHECK (PreviousRankPosition IS NULL OR PreviousRankPosition > 0),
    CONSTRAINT CK_RankingRecords_Trend CHECK (Trend IN ('Up', 'Down', 'Stable', 'New'))
);

CREATE TABLE dbo.Notifications (
    NotificationId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    NotificationTypeId INT NOT NULL,
    Title NVARCHAR(200) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    ReferenceType VARCHAR(100) NULL,
    ReferenceId UNIQUEIDENTIFIER NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ReadAt DATETIME2 NULL,

    CONSTRAINT FK_Notifications_User FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_Notifications_Type FOREIGN KEY (NotificationTypeId) REFERENCES dbo.NotificationTypes(NotificationTypeId)
);

CREATE TABLE dbo.BackgroundJobLogs (
    JobLogId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    JobName NVARCHAR(150) NOT NULL,
    Status VARCHAR(50) NOT NULL,
    Message NVARCHAR(MAX) NULL,
    StartedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    FinishedAt DATETIME2 NULL,

    CONSTRAINT CK_BackgroundJobLogs_Status CHECK (Status IN ('Started', 'Succeeded', 'Failed', 'Skipped'))
);

CREATE TABLE dbo.AssistantEarnings (
    EarningId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    AssistantId UNIQUEIDENTIFIER NOT NULL,
    TaskId UNIQUEIDENTIFIER NOT NULL,
    Amount DECIMAL(12,2) NOT NULL,
    EarningStatusId INT NOT NULL,
    CalculatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    PaidAt DATETIME2 NULL,

    CONSTRAINT FK_AssistantEarnings_Assistant FOREIGN KEY (AssistantId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_AssistantEarnings_Task FOREIGN KEY (TaskId) REFERENCES dbo.ProductionTasks(TaskId),
    CONSTRAINT FK_AssistantEarnings_Status FOREIGN KEY (EarningStatusId) REFERENCES dbo.EarningStatuses(EarningStatusId),
    CONSTRAINT UQ_AssistantEarnings_Task UNIQUE (TaskId),
    CONSTRAINT CK_AssistantEarnings_Amount CHECK (Amount >= 0)
);


CREATE TABLE dbo.WorkflowStatusHistories (
    WorkflowHistoryId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    EntityName NVARCHAR(100) NOT NULL,
    EntityId UNIQUEIDENTIFIER NOT NULL,
    FromStatusCode VARCHAR(50) NULL,
    ToStatusCode VARCHAR(50) NOT NULL,
    ChangedByUserId UNIQUEIDENTIFIER NULL,
    Note NVARCHAR(MAX) NULL,
    ChangedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_WorkflowStatusHistories_ChangedBy FOREIGN KEY (ChangedByUserId) REFERENCES dbo.Users(UserId)
);

CREATE TABLE dbo.AuditLogs (
    AuditLogId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    ActorUserId UNIQUEIDENTIFIER NULL,
    EntityName NVARCHAR(100) NOT NULL,
    EntityId UNIQUEIDENTIFIER NULL,
    ActionName NVARCHAR(100) NOT NULL,
    Details NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_AuditLogs_Actor FOREIGN KEY (ActorUserId) REFERENCES dbo.Users(UserId)
);
GO

/* ============================================================
   5. INDEXES
   ============================================================ */

CREATE INDEX IX_UserRoles_RoleId ON dbo.UserRoles(RoleId);

CREATE INDEX IX_Series_MangakaId ON dbo.Series(MangakaId);
CREATE INDEX IX_Series_TantouEditorId ON dbo.Series(TantouEditorId);
CREATE INDEX IX_Series_Status ON dbo.Series(SeriesStatusId);
CREATE INDEX IX_Series_CreatedAt ON dbo.Series(CreatedAt);


CREATE INDEX IX_SeriesTeamMembers_SeriesId ON dbo.SeriesTeamMembers(SeriesId);
CREATE INDEX IX_SeriesTeamMembers_UserId ON dbo.SeriesTeamMembers(UserId);
CREATE INDEX IX_PublicationIssues_PublishedDate ON dbo.PublicationIssues(PublishedDate);

CREATE INDEX IX_Chapters_SeriesId ON dbo.Chapters(SeriesId);
CREATE INDEX IX_Chapters_Status_Deadline ON dbo.Chapters(ChapterStatusId, Deadline);

CREATE INDEX IX_Manuscripts_SeriesId ON dbo.Manuscripts(SeriesId);
CREATE INDEX IX_Manuscripts_ChapterId ON dbo.Manuscripts(ChapterId);

CREATE INDEX IX_MangaPages_ChapterId ON dbo.MangaPages(ChapterId);
CREATE INDEX IX_MangaPages_Status ON dbo.MangaPages(PageStatusId);

CREATE INDEX IX_PageRegions_PageId ON dbo.PageRegions(PageId);
CREATE INDEX IX_PageRegions_RegionType ON dbo.PageRegions(RegionTypeId);


CREATE INDEX IX_ChapterPublications_Issue ON dbo.ChapterPublications(PublicationIssueId);
CREATE INDEX IX_AiSegmentationJobs_Page_Status ON dbo.AiSegmentationJobs(PageId, Status);

CREATE INDEX IX_ProductionTasks_PageId ON dbo.ProductionTasks(PageId);
CREATE INDEX IX_ProductionTasks_RegionId ON dbo.ProductionTasks(RegionId);
CREATE INDEX IX_ProductionTasks_Assistant_Status ON dbo.ProductionTasks(AssignedToAssistantId, TaskStatusId);
CREATE INDEX IX_ProductionTasks_Mangaka ON dbo.ProductionTasks(CreatedByMangakaId);
CREATE INDEX IX_ProductionTasks_Deadline ON dbo.ProductionTasks(Deadline);

CREATE INDEX IX_TaskSubmissions_TaskId ON dbo.TaskSubmissions(TaskId);
CREATE INDEX IX_TaskSubmissions_Assistant ON dbo.TaskSubmissions(SubmittedByAssistantId);
CREATE INDEX IX_TaskSubmissions_Status ON dbo.TaskSubmissions(SubmissionStatusId);

CREATE INDEX IX_EditorComments_PageId ON dbo.EditorComments(PageId);
CREATE INDEX IX_EditorComments_Editor ON dbo.EditorComments(EditorId);
CREATE INDEX IX_EditorComments_Status ON dbo.EditorComments(CommentStatusId);

CREATE INDEX IX_BoardVotes_SeriesId ON dbo.BoardVotes(SeriesId);
CREATE INDEX IX_BoardVotes_BoardMember ON dbo.BoardVotes(BoardMemberId);

CREATE INDEX IX_ReaderVoteData_Series_Issue ON dbo.ReaderVoteData(SeriesId, IssueNumber);
CREATE INDEX IX_RankingRecords_Series_Issue ON dbo.RankingRecords(SeriesId, IssueNumber);

CREATE INDEX IX_Notifications_User_IsRead ON dbo.Notifications(UserId, IsRead);
CREATE INDEX IX_Notifications_CreatedAt ON dbo.Notifications(CreatedAt);

CREATE INDEX IX_BackgroundJobLogs_JobName_StartedAt ON dbo.BackgroundJobLogs(JobName, StartedAt);

CREATE INDEX IX_AssistantEarnings_Assistant ON dbo.AssistantEarnings(AssistantId);
CREATE INDEX IX_AssistantEarnings_Status ON dbo.AssistantEarnings(EarningStatusId);


CREATE INDEX IX_WorkflowStatusHistories_Entity ON dbo.WorkflowStatusHistories(EntityName, EntityId);
CREATE INDEX IX_WorkflowStatusHistories_ChangedAt ON dbo.WorkflowStatusHistories(ChangedAt);

CREATE INDEX IX_AuditLogs_ActorUserId ON dbo.AuditLogs(ActorUserId);
CREATE INDEX IX_AuditLogs_Entity ON dbo.AuditLogs(EntityName, EntityId);
GO

/* ============================================================
   6. SEED LOOKUP DATA
   ============================================================ */

INSERT INTO dbo.Roles (RoleCode, RoleName)
VALUES
('Admin', N'Administrator'),
('Mangaka', N'Mangaka'),
('Assistant', N'Assistant'),
('TantouEditor', N'Tantou Editor'),
('EditorialBoard', N'Editorial Board Member');

INSERT INTO dbo.SeriesStatuses (StatusCode, StatusName)
VALUES
('Draft', N'Draft'),
('Submitted', N'Submitted'),
('UnderReview', N'Under Review'),
('RevisionRequired', N'Revision Required'),
('Approved', N'Approved'),
('Rejected', N'Rejected'),
('Publishing', N'Publishing'),
('Cancelled', N'Cancelled');

INSERT INTO dbo.PublicationSchedules (ScheduleCode, ScheduleName)
VALUES
('Weekly', N'Weekly'),
('Monthly', N'Monthly'),
('Special', N'Special Issue'),
('Paused', N'Paused');

INSERT INTO dbo.ChapterStatuses (StatusCode, StatusName)
VALUES
('Draft', N'Draft'),
('InProduction', N'In Production'),
('PendingMangakaReview', N'Pending Mangaka Review'),
('ReadyForEditorReview', N'Ready For Editor Review'),
('EditorRevisionRequired', N'Editor Revision Required'),
('ReadyForBoardDecision', N'Ready For Board Decision'),
('ReadyForPublication', N'Ready For Publication'),
('Published', N'Published'),
('Cancelled', N'Cancelled');

INSERT INTO dbo.ManuscriptStatuses (StatusCode, StatusName)
VALUES
('Uploaded', N'Uploaded'),
('Submitted', N'Submitted'),
('UnderReview', N'Under Review'),
('RevisionRequired', N'Revision Required'),
('Approved', N'Approved'),
('Rejected', N'Rejected');

INSERT INTO dbo.PageStatuses (StatusCode, StatusName)
VALUES
('Uploaded', N'Uploaded'),
('InProduction', N'In Production'),
('PendingReview', N'Pending Review'),
('RevisionRequired', N'Revision Required'),
('Approved', N'Approved');

INSERT INTO dbo.RegionTypes (TypeCode, TypeName)
VALUES
('Panel', N'Panel'),
('SpeechBubble', N'Speech Bubble'),
('Character', N'Character'),
('Background', N'Background'),
('Shading', N'Shading'),
('Effect', N'Effect'),
('Other', N'Other');

INSERT INTO dbo.TaskTypes (TypeCode, TypeName)
VALUES
('BackgroundDrawing', N'Background Drawing'),
('Shading', N'Shading'),
('Effect', N'Effect'),
('CleanLine', N'Clean Line'),
('Tone', N'Tone'),
('SpeechBubbleCleanup', N'Speech Bubble Cleanup'),
('Other', N'Other');

INSERT INTO dbo.TaskStatuses (StatusCode, StatusName)
VALUES
('Assigned', N'Assigned'),
('InProgress', N'In Progress'),
('Submitted', N'Submitted'),
('RevisionRequired', N'Revision Required'),
('Approved', N'Approved'),
('Rejected', N'Rejected'),
('Overdue', N'Overdue'),
('Cancelled', N'Cancelled');

INSERT INTO dbo.SubmissionStatuses (StatusCode, StatusName)
VALUES
('Submitted', N'Submitted'),
('Approved', N'Approved'),
('Rejected', N'Rejected'),
('RevisionRequired', N'Revision Required');

INSERT INTO dbo.CommentStatuses (StatusCode, StatusName)
VALUES
('Open', N'Open'),
('Resolved', N'Resolved'),
('Cancelled', N'Cancelled');

INSERT INTO dbo.VoteValues (VoteCode, VoteName)
VALUES
('Approve', N'Approve'),
('Reject', N'Reject'),
('NeedRevision', N'Need Revision'),
('Abstain', N'Abstain');

INSERT INTO dbo.DecisionTypes (DecisionCode, DecisionName)
VALUES
('Continue', N'Continue Publishing'),
('Cancel', N'Cancel Series'),
('ChangeSchedule', N'Change Publication Schedule'),
('Pause', N'Pause Series'),
('Promote', N'Promote Series');

INSERT INTO dbo.NotificationTypes (TypeCode, TypeName)
VALUES
('TaskAssigned', N'Task Assigned'),
('SubmissionUploaded', N'Submission Uploaded'),
('SubmissionReviewed', N'Submission Reviewed'),
('EditorCommentCreated', N'Editor Comment Created'),
('BoardVoteSubmitted', N'Board Vote Submitted'),
('RankingUpdated', N'Ranking Updated'),
('DeadlineWarning', N'Deadline Warning'),
('System', N'System');

INSERT INTO dbo.EarningStatuses (StatusCode, StatusName)
VALUES
('Pending', N'Pending'),
('Approved', N'Approved'),
('Paid', N'Paid'),
('Cancelled', N'Cancelled');
GO

/* ============================================================
   7. OPTIONAL SAMPLE USERS FOR DEVELOPMENT
   Password for all seeded accounts: test123@.
   PasswordHash values are BCrypt hashes, not plaintext.
   ============================================================ */

DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
DECLARE @MangakaId UNIQUEIDENTIFIER = NEWID();
DECLARE @AssistantId UNIQUEIDENTIFIER = NEWID();
DECLARE @EditorId UNIQUEIDENTIFIER = NEWID();
DECLARE @BoardId UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Users (UserId, FullName, Email, PasswordHash)
VALUES
(@AdminId, N'Admin User', N'admin@manga.local', N'$2a$12$Rwo5eRb9x0E/6Q9MUw1OcuwhQwT4lNIprqDIZfSU6l/B3jEg7Ve2m'),
(@MangakaId, N'Aki Tanaka', N'mangaka@manga.local', N'$2a$12$Rwo5eRb9x0E/6Q9MUw1OcuwhQwT4lNIprqDIZfSU6l/B3jEg7Ve2m'),
(@AssistantId, N'Mina Sato', N'assistant@manga.local', N'$2a$12$Rwo5eRb9x0E/6Q9MUw1OcuwhQwT4lNIprqDIZfSU6l/B3jEg7Ve2m'),
(@EditorId, N'Kobayashi Editor', N'editor@manga.local', N'$2a$12$Rwo5eRb9x0E/6Q9MUw1OcuwhQwT4lNIprqDIZfSU6l/B3jEg7Ve2m'),
(@BoardId, N'Board Member 01', N'board@manga.local', N'$2a$12$Rwo5eRb9x0E/6Q9MUw1OcuwhQwT4lNIprqDIZfSU6l/B3jEg7Ve2m');

INSERT INTO dbo.UserRoles (UserId, RoleId)
SELECT @AdminId, RoleId FROM dbo.Roles WHERE RoleCode = 'Admin'
UNION ALL
SELECT @MangakaId, RoleId FROM dbo.Roles WHERE RoleCode = 'Mangaka'
UNION ALL
SELECT @AssistantId, RoleId FROM dbo.Roles WHERE RoleCode = 'Assistant'
UNION ALL
SELECT @EditorId, RoleId FROM dbo.Roles WHERE RoleCode = 'TantouEditor'
UNION ALL
SELECT @BoardId, RoleId FROM dbo.Roles WHERE RoleCode = 'EditorialBoard';
GO


/* ============================================================
   8. DEMO DOMAIN DATA FOR END-TO-END PRESENTATION
   ============================================================ */

DECLARE @DemoMangakaId UNIQUEIDENTIFIER = (SELECT UserId FROM dbo.Users WHERE Email = N'mangaka@manga.local');
DECLARE @DemoAssistantId UNIQUEIDENTIFIER = (SELECT UserId FROM dbo.Users WHERE Email = N'assistant@manga.local');
DECLARE @DemoEditorId UNIQUEIDENTIFIER = (SELECT UserId FROM dbo.Users WHERE Email = N'editor@manga.local');
DECLARE @DemoBoardId UNIQUEIDENTIFIER = (SELECT UserId FROM dbo.Users WHERE Email = N'board@manga.local');
DECLARE @DemoAdminId UNIQUEIDENTIFIER = (SELECT UserId FROM dbo.Users WHERE Email = N'admin@manga.local');

DECLARE @SeriesId UNIQUEIDENTIFIER = NEWID();
DECLARE @ChapterId UNIQUEIDENTIFIER = NEWID();
DECLARE @Page1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Page2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Region1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Region2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Region3Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Task1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Task2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Task3Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Issue1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Issue2Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.Series (
    SeriesId, Title, AlternativeTitle, Description, Genre, CoverImageUrl,
    MangakaId, TantouEditorId, SeriesStatusId, PublicationScheduleId,
    CreatedAt, SubmittedAt, ApprovedAt, CancellationRiskScore
)
VALUES (
    @SeriesId,
    N'Shadow Ink',
    N'Bóng Mực',
    N'A demo manga series used to present the end-to-end workflow from proposal approval to task assignment, submission, editor review, ranking and warning.',
    N'Action / Mystery',
    N'/demo/series/shadow-ink-cover.png',
    @DemoMangakaId,
    @DemoEditorId,
    (SELECT SeriesStatusId FROM dbo.SeriesStatuses WHERE StatusCode = 'Publishing'),
    (SELECT PublicationScheduleId FROM dbo.PublicationSchedules WHERE ScheduleCode = 'Weekly'),
    DATEADD(DAY, -20, SYSUTCDATETIME()),
    DATEADD(DAY, -18, SYSUTCDATETIME()),
    DATEADD(DAY, -15, SYSUTCDATETIME()),
    72.50
);

INSERT INTO dbo.SeriesTeamMembers (SeriesId, UserId, RoleInSeries)
VALUES
(@SeriesId, @DemoMangakaId, N'Main Mangaka'),
(@SeriesId, @DemoAssistantId, N'Background / Tone Assistant'),
(@SeriesId, @DemoEditorId, N'Tantou Editor');

INSERT INTO dbo.Chapters (
    ChapterId, SeriesId, ChapterNumber, Title, Summary, ChapterStatusId, Deadline, CreatedAt
)
VALUES (
    @ChapterId,
    @SeriesId,
    1,
    N'The First Black Page',
    N'Chapter 1 demo: the protagonist discovers an ink mark that changes the city at midnight.',
    (SELECT ChapterStatusId FROM dbo.ChapterStatuses WHERE StatusCode = 'InProduction'),
    DATEADD(DAY, 3, SYSUTCDATETIME()),
    DATEADD(DAY, -10, SYSUTCDATETIME())
);

INSERT INTO dbo.PublicationIssues (PublicationIssueId, IssueNumber, IssueTitle, PublishedDate)
VALUES
(@Issue1Id, N'2026-W25', N'Weekly Manga Issue 25/2026', DATEADD(DAY, -7, CAST(GETDATE() AS DATE))),
(@Issue2Id, N'2026-W26', N'Weekly Manga Issue 26/2026', CAST(GETDATE() AS DATE));

INSERT INTO dbo.ChapterPublications (ChapterId, PublicationIssueId, PublicationOrder)
VALUES (@ChapterId, @Issue2Id, 3);

INSERT INTO dbo.Manuscripts (
    SeriesId, ChapterId, Title, Description, FileUrl, FileName, ContentType, FileSizeBytes,
    VersionNo, ManuscriptStatusId, UploadedByUserId
)
VALUES (
    @SeriesId,
    @ChapterId,
    N'Chapter 1 storyboard manuscript',
    N'Demo manuscript file for editor and board review.',
    N'/demo/manuscripts/shadow-ink-chapter-1-storyboard.pdf',
    N'shadow-ink-chapter-1-storyboard.pdf',
    N'application/pdf',
    2048000,
    1,
    (SELECT ManuscriptStatusId FROM dbo.ManuscriptStatuses WHERE StatusCode = 'Approved'),
    @DemoMangakaId
);

INSERT INTO dbo.MangaPages (
    PageId, ChapterId, PageNumber, ImageUrl, ThumbnailUrl, FileName, ContentType, FileSizeBytes,
    VersionNo, PageStatusId, UploadedByUserId
)
VALUES
(@Page1Id, @ChapterId, 1, N'/demo/pages/shadow-ink-c1-p1.png', N'/demo/pages/thumb-shadow-ink-c1-p1.png', N'shadow-ink-c1-p1.png', N'image/png', 880000, 1, (SELECT PageStatusId FROM dbo.PageStatuses WHERE StatusCode = 'InProduction'), @DemoMangakaId),
(@Page2Id, @ChapterId, 2, N'/demo/pages/shadow-ink-c1-p2.png', N'/demo/pages/thumb-shadow-ink-c1-p2.png', N'shadow-ink-c1-p2.png', N'image/png', 930000, 1, (SELECT PageStatusId FROM dbo.PageStatuses WHERE StatusCode = 'PendingReview'), @DemoMangakaId);

INSERT INTO dbo.PageRegions (
    RegionId, PageId, RegionTypeId, X, Y, Width, Height, Label, Confidence, SourceType, CreatedByUserId
)
VALUES
(@Region1Id, @Page1Id, (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'Background'), 40, 60, 460, 300, N'City alley background', NULL, 'Manual', @DemoMangakaId),
(@Region2Id, @Page1Id, (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'SpeechBubble'), 520, 90, 180, 95, N'Main dialogue bubble', 0.9300, 'AI', @DemoMangakaId),
(@Region3Id, @Page2Id, (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'Effect'), 100, 420, 340, 180, N'Ink splash effect', NULL, 'Manual', @DemoMangakaId);

INSERT INTO dbo.AiSegmentationJobs (
    PageId, RequestedByUserId, ModelName, Status, RawResultJson, StartedAt, CompletedAt
)
VALUES (
    @Page1Id,
    @DemoMangakaId,
    N'Mock-SAM-Manga-v1',
    'Succeeded',
    N'[{"regionType":"SpeechBubble","x":520,"y":90,"width":180,"height":95,"confidence":0.93},{"regionType":"Character","x":210,"y":250,"width":250,"height":520,"confidence":0.87}]',
    DATEADD(MINUTE, -30, SYSUTCDATETIME()),
    DATEADD(MINUTE, -29, SYSUTCDATETIME())
);

INSERT INTO dbo.ProductionTasks (
    TaskId, PageId, RegionId, AssignedToAssistantId, CreatedByMangakaId, TaskTypeId, TaskStatusId,
    Title, Description, Priority, Deadline, Price, CreatedAt, StartedAt, CompletedAt
)
VALUES
(@Task1Id, @Page1Id, @Region1Id, @DemoAssistantId, @DemoMangakaId, (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'BackgroundDrawing'), (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'Approved'), N'Draw alley background', N'Add dark alley background details behind the main character.', 2, DATEADD(DAY, -2, SYSUTCDATETIME()), 120000, DATEADD(DAY, -8, SYSUTCDATETIME()), DATEADD(DAY, -7, SYSUTCDATETIME()), DATEADD(DAY, -5, SYSUTCDATETIME())),
(@Task2Id, @Page1Id, @Region2Id, @DemoAssistantId, @DemoMangakaId, (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'SpeechBubbleCleanup'), (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'Submitted'), N'Clean dialogue bubble', N'Clean and align the main dialogue bubble.', 3, DATEADD(DAY, 1, SYSUTCDATETIME()), 50000, DATEADD(DAY, -3, SYSUTCDATETIME()), DATEADD(DAY, -2, SYSUTCDATETIME()), NULL),
(@Task3Id, @Page2Id, @Region3Id, @DemoAssistantId, @DemoMangakaId, (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'Effect'), (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'Overdue'), N'Add ink splash effect', N'Create a dramatic ink splash effect for the final panel.', 1, DATEADD(DAY, -1, SYSUTCDATETIME()), 90000, DATEADD(DAY, -5, SYSUTCDATETIME()), DATEADD(DAY, -4, SYSUTCDATETIME()), NULL);

INSERT INTO dbo.TaskSubmissions (
    TaskId, SubmittedByAssistantId, FileUrl, FileName, ContentType, FileSizeBytes, Comment,
    SubmissionStatusId, SubmittedAt, ReviewedAt, ReviewedByMangakaId, ReviewNote
)
VALUES
(@Task1Id, @DemoAssistantId, N'/demo/submissions/task1-background-v1.png', N'task1-background-v1.png', N'image/png', 760000, N'Background completed with extra alley details.', (SELECT SubmissionStatusId FROM dbo.SubmissionStatuses WHERE StatusCode = 'Approved'), DATEADD(DAY, -6, SYSUTCDATETIME()), DATEADD(DAY, -5, SYSUTCDATETIME()), @DemoMangakaId, N'Approved. Good perspective and mood.'),
(@Task2Id, @DemoAssistantId, N'/demo/submissions/task2-bubble-v1.png', N'task2-bubble-v1.png', N'image/png', 320000, N'Cleaned the bubble and aligned text area.', (SELECT SubmissionStatusId FROM dbo.SubmissionStatuses WHERE StatusCode = 'Submitted'), DATEADD(HOUR, -8, SYSUTCDATETIME()), NULL, NULL, NULL);

INSERT INTO dbo.EditorComments (
    PageId, EditorId, X, Y, Width, Height, CommentText, CommentStatusId, CreatedAt, ResolvedAt, ResolvedByUserId
)
VALUES
(@Page1Id, @DemoEditorId, 520, 90, 180, 95, N'The dialogue here should be shorter and more direct.', (SELECT CommentStatusId FROM dbo.CommentStatuses WHERE StatusCode = 'Open'), DATEADD(HOUR, -5, SYSUTCDATETIME()), NULL, NULL),
(@Page2Id, @DemoEditorId, 100, 420, 340, 180, N'This effect is important for the chapter ending. Please prioritize it.', (SELECT CommentStatusId FROM dbo.CommentStatuses WHERE StatusCode = 'Open'), DATEADD(HOUR, -3, SYSUTCDATETIME()), NULL, NULL);

INSERT INTO dbo.BoardVotes (SeriesId, BoardMemberId, VoteValueId, Comment, CreatedAt)
VALUES
(@SeriesId, @DemoBoardId, (SELECT VoteValueId FROM dbo.VoteValues WHERE VoteCode = 'Approve'), N'Approved for weekly publication. Strong visual hook and manageable production scope.', DATEADD(DAY, -15, SYSUTCDATETIME()));

INSERT INTO dbo.PublishingDecisions (SeriesId, DecisionTypeId, DecidedByUserId, Reason, CreatedAt)
VALUES
(@SeriesId, (SELECT DecisionTypeId FROM dbo.DecisionTypes WHERE DecisionCode = 'Continue'), @DemoBoardId, N'Continue publication but monitor ranking because the latest issue dropped.', DATEADD(DAY, -1, SYSUTCDATETIME()));

INSERT INTO dbo.ReaderVoteData (SeriesId, IssueNumber, VoteCount, RankPosition, ImportedByUserId, ImportedAt, Notes)
VALUES
(@SeriesId, N'2026-W25', 1840, 5, @DemoBoardId, DATEADD(DAY, -7, SYSUTCDATETIME()), N'Good initial reception.'),
(@SeriesId, N'2026-W26', 1210, 12, @DemoBoardId, DATEADD(HOUR, -12, SYSUTCDATETIME()), N'Ranking dropped after slower chapter pacing.');

INSERT INTO dbo.RankingRecords (SeriesId, IssueNumber, VoteCount, RankPosition, PreviousRankPosition, Trend, CalculatedAt)
VALUES
(@SeriesId, N'2026-W25', 1840, 5, NULL, 'New', DATEADD(DAY, -7, SYSUTCDATETIME())),
(@SeriesId, N'2026-W26', 1210, 12, 5, 'Down', DATEADD(HOUR, -12, SYSUTCDATETIME()));

INSERT INTO dbo.Notifications (UserId, NotificationTypeId, Title, Message, ReferenceType, ReferenceId, IsRead, CreatedAt)
VALUES
(@DemoAssistantId, (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'TaskAssigned'), N'New manga task assigned', N'You have been assigned: Add ink splash effect.', N'ProductionTask', @Task3Id, 0, DATEADD(DAY, -5, SYSUTCDATETIME())),
(@DemoMangakaId, (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'SubmissionUploaded'), N'Assistant submitted work', N'Mina submitted the dialogue bubble cleanup task.', N'TaskSubmission', @Task2Id, 0, DATEADD(HOUR, -8, SYSUTCDATETIME())),
(@DemoMangakaId, (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'RankingUpdated'), N'Ranking updated', N'Shadow Ink dropped from rank 5 to rank 12 in issue 2026-W26.', N'Series', @SeriesId, 0, DATEADD(HOUR, -12, SYSUTCDATETIME())),
(@DemoEditorId, (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'DeadlineWarning'), N'Chapter deadline risk', N'Chapter 1 has an overdue high-priority task.', N'Chapter', @ChapterId, 0, DATEADD(HOUR, -1, SYSUTCDATETIME()));

INSERT INTO dbo.AssistantEarnings (AssistantId, TaskId, Amount, EarningStatusId, CalculatedAt)
VALUES
(@DemoAssistantId, @Task1Id, 120000, (SELECT EarningStatusId FROM dbo.EarningStatuses WHERE StatusCode = 'Approved'), DATEADD(DAY, -5, SYSUTCDATETIME()));

INSERT INTO dbo.BackgroundJobLogs (JobName, Status, Message, StartedAt, FinishedAt)
VALUES
(N'DeadlineReminderJob', 'Succeeded', N'Sent deadline reminder for Chapter 1 and overdue task warnings.', DATEADD(HOUR, -1, SYSUTCDATETIME()), DATEADD(MINUTE, -58, SYSUTCDATETIME())),
(N'RankingRiskScannerJob', 'Succeeded', N'Shadow Ink marked as high cancellation risk due to ranking drop.', DATEADD(HOUR, -12, SYSUTCDATETIME()), DATEADD(HOUR, -12, DATEADD(MINUTE, 2, SYSUTCDATETIME()))),
(N'NotificationCleanupJob', 'Skipped', N'No old notifications found for cleanup.', DATEADD(DAY, -1, SYSUTCDATETIME()), DATEADD(DAY, -1, DATEADD(MINUTE, 1, SYSUTCDATETIME())));

INSERT INTO dbo.WorkflowStatusHistories (EntityName, EntityId, FromStatusCode, ToStatusCode, ChangedByUserId, Note, ChangedAt)
VALUES
(N'Series', @SeriesId, 'Draft', 'Submitted', @DemoMangakaId, N'Mangaka submitted series proposal.', DATEADD(DAY, -18, SYSUTCDATETIME())),
(N'Series', @SeriesId, 'Submitted', 'Approved', @DemoBoardId, N'Board approved the series.', DATEADD(DAY, -15, SYSUTCDATETIME())),
(N'ProductionTask', @Task1Id, 'Submitted', 'Approved', @DemoMangakaId, N'Mangaka approved assistant submission.', DATEADD(DAY, -5, SYSUTCDATETIME())),
(N'ProductionTask', @Task3Id, 'InProgress', 'Overdue', @DemoAdminId, N'Background worker marked task as overdue.', DATEADD(HOUR, -1, SYSUTCDATETIME()));

INSERT INTO dbo.AuditLogs (ActorUserId, EntityName, EntityId, ActionName, Details, CreatedAt)
VALUES
(@DemoMangakaId, N'Series', @SeriesId, N'CreateSeries', N'Demo series created.', DATEADD(DAY, -20, SYSUTCDATETIME())),
(@DemoMangakaId, N'ProductionTask', @Task3Id, N'AssignTask', N'Assigned high-priority effect task to assistant.', DATEADD(DAY, -5, SYSUTCDATETIME())),
(@DemoBoardId, N'ReaderVoteData', @SeriesId, N'ImportReaderVoteData', N'Imported issue 2026-W26 reader votes.', DATEADD(HOUR, -12, SYSUTCDATETIME()));
GO

/* ============================================================
   9. DASHBOARD VIEWS
   ============================================================ */

CREATE VIEW dbo.vw_ChapterProgress AS
SELECT
    c.ChapterId,
    c.SeriesId,
    s.Title AS SeriesTitle,
    c.ChapterNumber,
    c.Title AS ChapterTitle,
    cs.StatusCode AS ChapterStatus,
    COUNT(t.TaskId) AS TotalTasks,
    SUM(CASE WHEN ts.StatusCode = 'Approved' THEN 1 ELSE 0 END) AS ApprovedTasks,
    SUM(CASE WHEN ts.StatusCode = 'Overdue' THEN 1 ELSE 0 END) AS OverdueTasks,
    CASE
        WHEN COUNT(t.TaskId) = 0 THEN CAST(0 AS DECIMAL(5,2))
        ELSE CAST(SUM(CASE WHEN ts.StatusCode = 'Approved' THEN 1 ELSE 0 END) * 100.0 / COUNT(t.TaskId) AS DECIMAL(5,2))
    END AS ProgressPercent,
    c.Deadline
FROM dbo.Chapters c
JOIN dbo.Series s ON c.SeriesId = s.SeriesId
JOIN dbo.ChapterStatuses cs ON c.ChapterStatusId = cs.ChapterStatusId
LEFT JOIN dbo.MangaPages p ON c.ChapterId = p.ChapterId
LEFT JOIN dbo.ProductionTasks t ON p.PageId = t.PageId
LEFT JOIN dbo.TaskStatuses ts ON t.TaskStatusId = ts.TaskStatusId
GROUP BY
    c.ChapterId, c.SeriesId, s.Title, c.ChapterNumber, c.Title, cs.StatusCode, c.Deadline;
GO

CREATE VIEW dbo.vw_AssistantMonthlyEarnings AS
SELECT
    ae.AssistantId,
    u.FullName AS AssistantName,
    YEAR(ae.CalculatedAt) AS EarningYear,
    MONTH(ae.CalculatedAt) AS EarningMonth,
    COUNT(ae.EarningId) AS TotalPaidTasks,
    SUM(ae.Amount) AS TotalAmount
FROM dbo.AssistantEarnings ae
JOIN dbo.Users u ON ae.AssistantId = u.UserId
JOIN dbo.EarningStatuses es ON ae.EarningStatusId = es.EarningStatusId
WHERE es.StatusCode IN ('Approved', 'Paid')
GROUP BY ae.AssistantId, u.FullName, YEAR(ae.CalculatedAt), MONTH(ae.CalculatedAt);
GO

CREATE VIEW dbo.vw_SeriesLatestRanking AS
WITH LatestRanking AS (
    SELECT
        rr.*,
        ROW_NUMBER() OVER (PARTITION BY rr.SeriesId ORDER BY rr.CalculatedAt DESC) AS RowNum
    FROM dbo.RankingRecords rr
)
SELECT
    s.SeriesId,
    s.Title AS SeriesTitle,
    lr.IssueNumber,
    lr.VoteCount,
    lr.RankPosition,
    lr.PreviousRankPosition,
    lr.Trend,
    lr.CalculatedAt,
    s.CancellationRiskScore
FROM dbo.Series s
LEFT JOIN LatestRanking lr ON s.SeriesId = lr.SeriesId AND lr.RowNum = 1;
GO

/* ============================================================
   10. QUICK VALIDATION QUERIES
   ============================================================ */

SELECT 'Database MangaWorkflowDB created successfully.' AS Message;
SELECT RoleCode, RoleName FROM dbo.Roles;
SELECT StatusCode, StatusName FROM dbo.SeriesStatuses;
SELECT FullName, Email FROM dbo.Users;
SELECT TOP 10 * FROM dbo.vw_ChapterProgress;
SELECT TOP 10 * FROM dbo.vw_SeriesLatestRanking;
SELECT TOP 10 * FROM dbo.Notifications ORDER BY CreatedAt DESC;
GO
