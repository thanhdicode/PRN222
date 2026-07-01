/*
    MangaWorkflow full demo showcase seed.

    Purpose:
    - Add rich demo data for MVC CRUD, auth/authorization, Assistant workflow,
      Board review, rankings, notifications, SignalR, Worker logs, and AI Studio.
    - Use full remote image URLs for covers, manga pages, thumbnails, avatars,
      manuscripts, and submissions.
    - Be idempotent enough for local demo refreshes: rows are found by natural
      demo keys such as title, issue number, page number, label, and task title.

    Run after:
    - db/MangaWorkflowDB_v2_demo_ready.sql
    - db/MangaWorkflowDB_v3_extra_seed_demo_data.sql
    - Database/seed_notification_types_v2.sql (optional; this script also covers it)
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

/* ------------------------------------------------------------
   0. Required users and lookup values
   ------------------------------------------------------------ */

DECLARE
    @AdminId uniqueidentifier,
    @MangakaId uniqueidentifier,
    @Mangaka2Id uniqueidentifier,
    @AssistantId uniqueidentifier,
    @Assistant2Id uniqueidentifier,
    @Assistant3Id uniqueidentifier,
    @EditorId uniqueidentifier,
    @Editor2Id uniqueidentifier,
    @BoardId uniqueidentifier,
    @Board2Id uniqueidentifier,
    @Board3Id uniqueidentifier;

SELECT @AdminId = UserId FROM dbo.Users WHERE Email = 'admin@manga.local';
SELECT @MangakaId = UserId FROM dbo.Users WHERE Email = 'mangaka@manga.local';
SELECT @Mangaka2Id = UserId FROM dbo.Users WHERE Email = 'mangaka2@manga.local';
SELECT @AssistantId = UserId FROM dbo.Users WHERE Email = 'assistant@manga.local';
SELECT @Assistant2Id = UserId FROM dbo.Users WHERE Email = 'assistant2@manga.local';
SELECT @Assistant3Id = UserId FROM dbo.Users WHERE Email = 'assistant3@manga.local';
SELECT @EditorId = UserId FROM dbo.Users WHERE Email = 'editor@manga.local';
SELECT @Editor2Id = UserId FROM dbo.Users WHERE Email = 'editor2@manga.local';
SELECT @BoardId = UserId FROM dbo.Users WHERE Email = 'board@manga.local';
SELECT @Board2Id = UserId FROM dbo.Users WHERE Email = 'board2@manga.local';
SELECT @Board3Id = UserId FROM dbo.Users WHERE Email = 'board3@manga.local';

IF @AdminId IS NULL OR @MangakaId IS NULL OR @AssistantId IS NULL OR @EditorId IS NULL OR @BoardId IS NULL
    THROW 51000, 'Required demo users are missing. Run the base seed scripts first.', 1;

-- Make avatar URLs look good on user-management screens.
UPDATE dbo.Users SET AvatarUrl = 'https://api.dicebear.com/8.x/initials/svg?seed=Admin%20Aki&backgroundColor=0ea5e9' WHERE UserId = @AdminId;
UPDATE dbo.Users SET AvatarUrl = 'https://api.dicebear.com/8.x/initials/svg?seed=Mangaka%20Sora&backgroundColor=f97316' WHERE UserId = @MangakaId;
UPDATE dbo.Users SET AvatarUrl = 'https://api.dicebear.com/8.x/initials/svg?seed=Mangaka%20Mika&backgroundColor=8b5cf6' WHERE UserId = @Mangaka2Id;
UPDATE dbo.Users SET AvatarUrl = 'https://api.dicebear.com/8.x/initials/svg?seed=Assistant%20Mina&backgroundColor=22c55e' WHERE UserId = @AssistantId;
UPDATE dbo.Users SET AvatarUrl = 'https://api.dicebear.com/8.x/initials/svg?seed=Assistant%20Ken&backgroundColor=06b6d4' WHERE UserId = @Assistant2Id;
UPDATE dbo.Users SET AvatarUrl = 'https://api.dicebear.com/8.x/initials/svg?seed=Assistant%20Hana&backgroundColor=eab308' WHERE UserId = @Assistant3Id;
UPDATE dbo.Users SET AvatarUrl = 'https://api.dicebear.com/8.x/initials/svg?seed=Editor%20Ren&backgroundColor=ef4444' WHERE UserId = @EditorId;
UPDATE dbo.Users SET AvatarUrl = 'https://api.dicebear.com/8.x/initials/svg?seed=Board%20Yuki&backgroundColor=14b8a6' WHERE UserId = @BoardId;

-- Notification types used by SET 1, Worker, Board, and auth demos.
INSERT INTO dbo.NotificationTypes (TypeCode, TypeName)
SELECT v.TypeCode, v.TypeName
FROM (VALUES
    ('UserLogin', 'User Login'),
    ('UserLogout', 'User Logout'),
    ('SeriesSubmitted', 'Series Submitted'),
    ('BoardVoteCast', 'Board Vote Cast'),
    ('BoardVoteSubmitted', 'Board Vote Submitted'),
    ('SeriesApproved', 'Series Approved'),
    ('SeriesRejected', 'Series Rejected'),
    ('TaskAssigned', 'Task Assigned'),
    ('TaskApproved', 'Task Approved'),
    ('TaskRejected', 'Task Rejected'),
    ('DeadlineReminder', 'Deadline Reminder'),
    ('DeadlineWarning', 'Deadline Warning'),
    ('TaskOverdue', 'Task Overdue'),
    ('SubmissionUploaded', 'Submission Uploaded'),
    ('SubmissionReviewed', 'Submission Reviewed'),
    ('EditorCommentCreated', 'Editor Comment Created'),
    ('RankingUpdated', 'Ranking Updated'),
    ('RankingRisk', 'Ranking Risk'),
    ('System', 'System Notification')
) AS v(TypeCode, TypeName)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.NotificationTypes nt WHERE nt.TypeCode = v.TypeCode
);

DECLARE
    @SeriesDraft int = (SELECT SeriesStatusId FROM dbo.SeriesStatuses WHERE StatusCode = 'Draft'),
    @SeriesSubmitted int = (SELECT SeriesStatusId FROM dbo.SeriesStatuses WHERE StatusCode = 'Submitted'),
    @SeriesUnderReview int = (SELECT SeriesStatusId FROM dbo.SeriesStatuses WHERE StatusCode = 'UnderReview'),
    @SeriesApproved int = (SELECT SeriesStatusId FROM dbo.SeriesStatuses WHERE StatusCode = 'Approved'),
    @SeriesPublishing int = (SELECT SeriesStatusId FROM dbo.SeriesStatuses WHERE StatusCode = 'Publishing'),
    @ChapterDraft int = (SELECT ChapterStatusId FROM dbo.ChapterStatuses WHERE StatusCode = 'Draft'),
    @ChapterInProduction int = (SELECT ChapterStatusId FROM dbo.ChapterStatuses WHERE StatusCode = 'InProduction'),
    @ChapterEditorReview int = (SELECT ChapterStatusId FROM dbo.ChapterStatuses WHERE StatusCode = 'ReadyForEditorReview'),
    @ChapterMangakaReview int = (SELECT ChapterStatusId FROM dbo.ChapterStatuses WHERE StatusCode = 'PendingMangakaReview'),
    @ChapterPublished int = (SELECT ChapterStatusId FROM dbo.ChapterStatuses WHERE StatusCode = 'Published'),
    @PageUploaded int = (SELECT PageStatusId FROM dbo.PageStatuses WHERE StatusCode = 'Uploaded'),
    @PageInProduction int = (SELECT PageStatusId FROM dbo.PageStatuses WHERE StatusCode = 'InProduction'),
    @PagePendingReview int = (SELECT PageStatusId FROM dbo.PageStatuses WHERE StatusCode = 'PendingReview'),
    @PageApproved int = (SELECT PageStatusId FROM dbo.PageStatuses WHERE StatusCode = 'Approved'),
    @TaskAssigned int = (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'Assigned'),
    @TaskInProgress int = (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'InProgress'),
    @TaskSubmitted int = (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'Submitted'),
    @TaskApproved int = (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'Approved'),
    @TaskOverdue int = (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'Overdue'),
    @SubSubmitted int = (SELECT SubmissionStatusId FROM dbo.SubmissionStatuses WHERE StatusCode = 'Submitted'),
    @SubApproved int = (SELECT SubmissionStatusId FROM dbo.SubmissionStatuses WHERE StatusCode = 'Approved'),
    @CommentOpen int = (SELECT CommentStatusId FROM dbo.CommentStatuses WHERE StatusCode = 'Open'),
    @CommentResolved int = (SELECT CommentStatusId FROM dbo.CommentStatuses WHERE StatusCode = 'Resolved'),
    @EarnPending int = (SELECT EarningStatusId FROM dbo.EarningStatuses WHERE StatusCode = 'Pending'),
    @EarnPaid int = (SELECT EarningStatusId FROM dbo.EarningStatuses WHERE StatusCode = 'Paid'),
    @VoteApprove int = (SELECT VoteValueId FROM dbo.VoteValues WHERE VoteCode = 'Approve'),
    @VoteNeedRevision int = (SELECT VoteValueId FROM dbo.VoteValues WHERE VoteCode = 'NeedRevision'),
    @WeeklySchedule int = (SELECT PublicationScheduleId FROM dbo.PublicationSchedules WHERE ScheduleCode = 'Weekly'),
    @MonthlySchedule int = (SELECT PublicationScheduleId FROM dbo.PublicationSchedules WHERE ScheduleCode = 'Monthly');

DECLARE
    @TypeCleanLine int = (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'CleanLine'),
    @TypeSpeech int = (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'SpeechBubbleCleanup'),
    @TypeBackground int = (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'BackgroundDrawing'),
    @TypeShading int = (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'Shading'),
    @TypeEffect int = (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'Effect'),
    @TypeTone int = (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'Tone'),
    @RegionPanel int = (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'Panel'),
    @RegionCharacter int = (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'Character'),
    @RegionSpeech int = (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'SpeechBubble'),
    @RegionBackground int = (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'Background'),
    @RegionEffect int = (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'Effect'),
    @RegionShading int = (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'Shading');

IF @SeriesDraft IS NULL OR @PageUploaded IS NULL OR @TaskAssigned IS NULL OR @SubSubmitted IS NULL OR @TypeCleanLine IS NULL OR @RegionPanel IS NULL
    THROW 51001, 'Required lookup rows are missing. Run the base seed scripts first.', 1;

/* ------------------------------------------------------------
   1. Publication issues
   ------------------------------------------------------------ */

IF NOT EXISTS (SELECT 1 FROM dbo.PublicationIssues WHERE IssueNumber = '2026-W30')
    INSERT INTO dbo.PublicationIssues (IssueNumber, IssueTitle, PublishedDate)
    VALUES ('2026-W30', N'Weekly Manga Issue 30/2026 - Summer Showcase', '2026-07-31');

IF NOT EXISTS (SELECT 1 FROM dbo.PublicationIssues WHERE IssueNumber = '2026-W31')
    INSERT INTO dbo.PublicationIssues (IssueNumber, IssueTitle, PublishedDate)
    VALUES ('2026-W31', N'Weekly Manga Issue 31/2026 - Rising Stars', '2026-08-07');

DECLARE
    @IssueW30 uniqueidentifier = (SELECT PublicationIssueId FROM dbo.PublicationIssues WHERE IssueNumber = '2026-W30'),
    @IssueW31 uniqueidentifier = (SELECT PublicationIssueId FROM dbo.PublicationIssues WHERE IssueNumber = '2026-W31');

/* ------------------------------------------------------------
   2. Series portfolio
   ------------------------------------------------------------ */

DECLARE
    @Neon uniqueidentifier,
    @Moonlit uniqueidentifier,
    @Azure uniqueidentifier,
    @Clockwork uniqueidentifier,
    @Lantern uniqueidentifier,
    @Signal uniqueidentifier;

IF NOT EXISTS (SELECT 1 FROM dbo.Series WHERE Title = N'Neon Ronin Academy')
    INSERT INTO dbo.Series (Title, AlternativeTitle, Description, Genre, CoverImageUrl, MangakaId, TantouEditorId, SeriesStatusId, PublicationScheduleId, CreatedAt, SubmittedAt, ApprovedAt, CancellationRiskScore, IsDeleted)
    VALUES (N'Neon Ronin Academy', N'Kougaku Ronin Gakuen', N'A near-future academy where apprentice swordsmen defend a neon city from rogue AI yokai.', N'Action / Sci-Fi', 'https://picsum.photos/seed/neon-ronin-cover/640/900', @MangakaId, @EditorId, @SeriesPublishing, @WeeklySchedule, DATEADD(day,-45,SYSUTCDATETIME()), DATEADD(day,-40,SYSUTCDATETIME()), DATEADD(day,-35,SYSUTCDATETIME()), 12.50, 0);
SELECT @Neon = SeriesId FROM dbo.Series WHERE Title = N'Neon Ronin Academy';
UPDATE dbo.Series SET CoverImageUrl = 'https://picsum.photos/seed/neon-ronin-cover/640/900', SeriesStatusId = @SeriesPublishing, TantouEditorId = @EditorId, PublicationScheduleId = @WeeklySchedule WHERE SeriesId = @Neon;

IF NOT EXISTS (SELECT 1 FROM dbo.Series WHERE Title = N'Moonlit Bento Detective')
    INSERT INTO dbo.Series (Title, AlternativeTitle, Description, Genre, CoverImageUrl, MangakaId, TantouEditorId, SeriesStatusId, PublicationScheduleId, CreatedAt, SubmittedAt, CancellationRiskScore, IsDeleted)
    VALUES (N'Moonlit Bento Detective', N'Tsukiyo Bento Tantei', N'A food-loving detective solves midnight market mysteries through recipes, clues, and found-family comedy.', N'Mystery / Slice of Life', 'https://picsum.photos/seed/moonlit-bento-cover/640/900', @MangakaId, @EditorId, @SeriesSubmitted, @MonthlySchedule, DATEADD(day,-18,SYSUTCDATETIME()), DATEADD(day,-1,SYSUTCDATETIME()), 28.00, 0);
SELECT @Moonlit = SeriesId FROM dbo.Series WHERE Title = N'Moonlit Bento Detective';
UPDATE dbo.Series SET CoverImageUrl = 'https://picsum.photos/seed/moonlit-bento-cover/640/900', SeriesStatusId = @SeriesSubmitted, SubmittedAt = COALESCE(SubmittedAt, DATEADD(day,-1,SYSUTCDATETIME())) WHERE SeriesId = @Moonlit;

IF NOT EXISTS (SELECT 1 FROM dbo.Series WHERE Title = N'Azure Dragon Courier')
    INSERT INTO dbo.Series (Title, AlternativeTitle, Description, Genre, CoverImageUrl, MangakaId, TantouEditorId, SeriesStatusId, PublicationScheduleId, CreatedAt, SubmittedAt, CancellationRiskScore, IsDeleted)
    VALUES (N'Azure Dragon Courier', N'Seiryu Hai Tatsujin', N'A sky courier and her tiny dragon deliver dangerous letters across floating islands.', N'Fantasy / Adventure', 'https://picsum.photos/seed/azure-dragon-cover/640/900', @Mangaka2Id, @Editor2Id, @SeriesUnderReview, @WeeklySchedule, DATEADD(day,-25,SYSUTCDATETIME()), DATEADD(day,-3,SYSUTCDATETIME()), 21.25, 0);
SELECT @Azure = SeriesId FROM dbo.Series WHERE Title = N'Azure Dragon Courier';
UPDATE dbo.Series SET CoverImageUrl = 'https://picsum.photos/seed/azure-dragon-cover/640/900', SeriesStatusId = @SeriesUnderReview, SubmittedAt = COALESCE(SubmittedAt, DATEADD(day,-3,SYSUTCDATETIME())) WHERE SeriesId = @Azure;

IF NOT EXISTS (SELECT 1 FROM dbo.Series WHERE Title = N'Clockwork Sakura Atelier')
    INSERT INTO dbo.Series (Title, AlternativeTitle, Description, Genre, CoverImageUrl, MangakaId, TantouEditorId, SeriesStatusId, PublicationScheduleId, CreatedAt, CancellationRiskScore, IsDeleted)
    VALUES (N'Clockwork Sakura Atelier', N'Karakuri Sakura Kobo', N'A gentle workshop drama about mechanical flowers, family repairs, and tiny miracles.', N'Drama / Steampunk', 'https://picsum.photos/seed/clockwork-sakura-cover/640/900', @MangakaId, @EditorId, @SeriesDraft, @MonthlySchedule, DATEADD(day,-5,SYSUTCDATETIME()), 8.00, 0);
SELECT @Clockwork = SeriesId FROM dbo.Series WHERE Title = N'Clockwork Sakura Atelier';
UPDATE dbo.Series SET CoverImageUrl = 'https://picsum.photos/seed/clockwork-sakura-cover/640/900', SeriesStatusId = @SeriesDraft WHERE SeriesId = @Clockwork;

IF NOT EXISTS (SELECT 1 FROM dbo.Series WHERE Title = N'Paper Lantern Ghosts')
    INSERT INTO dbo.Series (Title, AlternativeTitle, Description, Genre, CoverImageUrl, MangakaId, TantouEditorId, SeriesStatusId, PublicationScheduleId, CreatedAt, SubmittedAt, ApprovedAt, CancellationRiskScore, IsDeleted)
    VALUES (N'Paper Lantern Ghosts', N'Chouchin no Yuurei', N'A supernatural romance about a shrine apprentice who guides lost spirits during festival nights.', N'Supernatural / Romance', 'https://picsum.photos/seed/paper-lantern-cover/640/900', @Mangaka2Id, @EditorId, @SeriesApproved, @WeeklySchedule, DATEADD(day,-60,SYSUTCDATETIME()), DATEADD(day,-55,SYSUTCDATETIME()), DATEADD(day,-50,SYSUTCDATETIME()), 17.75, 0);
SELECT @Lantern = SeriesId FROM dbo.Series WHERE Title = N'Paper Lantern Ghosts';
UPDATE dbo.Series SET CoverImageUrl = 'https://picsum.photos/seed/paper-lantern-cover/640/900', SeriesStatusId = @SeriesApproved WHERE SeriesId = @Lantern;

IF NOT EXISTS (SELECT 1 FROM dbo.Series WHERE Title = N'Signal Star Cafe')
    INSERT INTO dbo.Series (Title, AlternativeTitle, Description, Genre, CoverImageUrl, MangakaId, TantouEditorId, SeriesStatusId, PublicationScheduleId, CreatedAt, SubmittedAt, ApprovedAt, CancellationRiskScore, IsDeleted)
    VALUES (N'Signal Star Cafe', N'Hoshi no Shinario Cafe', N'A cozy idol-cafe story where live-stream signals from space change each episode''s stage performance.', N'Comedy / Music', 'https://picsum.photos/seed/signal-star-cover/640/900', @MangakaId, @Editor2Id, @SeriesApproved, @WeeklySchedule, DATEADD(day,-32,SYSUTCDATETIME()), DATEADD(day,-30,SYSUTCDATETIME()), DATEADD(day,-26,SYSUTCDATETIME()), 33.00, 0);
SELECT @Signal = SeriesId FROM dbo.Series WHERE Title = N'Signal Star Cafe';
UPDATE dbo.Series SET CoverImageUrl = 'https://picsum.photos/seed/signal-star-cover/640/900', SeriesStatusId = @SeriesApproved WHERE SeriesId = @Signal;

-- Series team memberships for dashboards and authorization-friendly views.
INSERT INTO dbo.SeriesTeamMembers (SeriesId, UserId, RoleInSeries, IsActive)
SELECT s.SeriesId, s.UserId, s.RoleInSeries, 1
FROM (VALUES
    (@Neon, @MangakaId, N'Mangaka'),
    (@Neon, @EditorId, N'TantouEditor'),
    (@Neon, @AssistantId, N'Lead Assistant'),
    (@Neon, @Assistant2Id, N'Lettering Assistant'),
    (@Neon, @Assistant3Id, N'Effects Assistant'),
    (@Moonlit, @MangakaId, N'Mangaka'),
    (@Moonlit, @EditorId, N'TantouEditor'),
    (@Moonlit, @AssistantId, N'Assistant'),
    (@Azure, @Mangaka2Id, N'Mangaka'),
    (@Azure, @Editor2Id, N'TantouEditor'),
    (@Azure, @Assistant2Id, N'Assistant'),
    (@Clockwork, @MangakaId, N'Mangaka'),
    (@Clockwork, @EditorId, N'TantouEditor'),
    (@Lantern, @Mangaka2Id, N'Mangaka'),
    (@Lantern, @EditorId, N'TantouEditor'),
    (@Signal, @MangakaId, N'Mangaka'),
    (@Signal, @Editor2Id, N'TantouEditor')
) AS s(SeriesId, UserId, RoleInSeries)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.SeriesTeamMembers stm
    WHERE stm.SeriesId = s.SeriesId AND stm.UserId = s.UserId AND stm.RoleInSeries = s.RoleInSeries
);

/* ------------------------------------------------------------
   3. Chapters and pages with full image URLs
   ------------------------------------------------------------ */

DECLARE
    @NeonCh1 uniqueidentifier, @NeonCh2 uniqueidentifier,
    @MoonCh1 uniqueidentifier, @AzureCh1 uniqueidentifier,
    @ClockCh1 uniqueidentifier, @LanternCh1 uniqueidentifier, @SignalCh1 uniqueidentifier;

IF NOT EXISTS (SELECT 1 FROM dbo.Chapters WHERE SeriesId = @Neon AND ChapterNumber = 1)
    INSERT INTO dbo.Chapters (SeriesId, ChapterNumber, Title, Summary, ChapterStatusId, Deadline, CreatedAt, CompletedAt)
    VALUES (@Neon, 1, N'Entrance Exam at Midnight', N'The new class survives a holographic yokai exam under the city railways.', @ChapterEditorReview, DATEADD(day, 4, SYSUTCDATETIME()), DATEADD(day,-38,SYSUTCDATETIME()), NULL);
SELECT @NeonCh1 = ChapterId FROM dbo.Chapters WHERE SeriesId = @Neon AND ChapterNumber = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.Chapters WHERE SeriesId = @Neon AND ChapterNumber = 2)
    INSERT INTO dbo.Chapters (SeriesId, ChapterNumber, Title, Summary, ChapterStatusId, Deadline, CreatedAt)
    VALUES (@Neon, 2, N'Blade Server Crash', N'A training server goes feral and locks the squad inside a combat simulation.', @ChapterInProduction, DATEADD(day, 10, SYSUTCDATETIME()), DATEADD(day,-18,SYSUTCDATETIME()));
SELECT @NeonCh2 = ChapterId FROM dbo.Chapters WHERE SeriesId = @Neon AND ChapterNumber = 2;

IF NOT EXISTS (SELECT 1 FROM dbo.Chapters WHERE SeriesId = @Moonlit AND ChapterNumber = 1)
    INSERT INTO dbo.Chapters (SeriesId, ChapterNumber, Title, Summary, ChapterStatusId, Deadline, CreatedAt)
    VALUES (@Moonlit, 1, N'The Vanishing Egg Roll', N'A bento clue trail points to the wrong suspect until the sauce pattern gives it away.', @ChapterMangakaReview, DATEADD(day, 6, SYSUTCDATETIME()), DATEADD(day,-16,SYSUTCDATETIME()));
SELECT @MoonCh1 = ChapterId FROM dbo.Chapters WHERE SeriesId = @Moonlit AND ChapterNumber = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.Chapters WHERE SeriesId = @Azure AND ChapterNumber = 1)
    INSERT INTO dbo.Chapters (SeriesId, ChapterNumber, Title, Summary, ChapterStatusId, Deadline, CreatedAt)
    VALUES (@Azure, 1, N'Letter to the Storm Island', N'The courier takes a sealed message through a dragon-thunder cloud corridor.', @ChapterEditorReview, DATEADD(day, 8, SYSUTCDATETIME()), DATEADD(day,-22,SYSUTCDATETIME()));
SELECT @AzureCh1 = ChapterId FROM dbo.Chapters WHERE SeriesId = @Azure AND ChapterNumber = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.Chapters WHERE SeriesId = @Clockwork AND ChapterNumber = 1)
    INSERT INTO dbo.Chapters (SeriesId, ChapterNumber, Title, Summary, ChapterStatusId, Deadline, CreatedAt)
    VALUES (@Clockwork, 1, N'The Brass Petal Request', N'A child asks the atelier to repair a flower that only blooms for one memory.', @ChapterDraft, DATEADD(day, 20, SYSUTCDATETIME()), DATEADD(day,-4,SYSUTCDATETIME()));
SELECT @ClockCh1 = ChapterId FROM dbo.Chapters WHERE SeriesId = @Clockwork AND ChapterNumber = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.Chapters WHERE SeriesId = @Lantern AND ChapterNumber = 1)
    INSERT INTO dbo.Chapters (SeriesId, ChapterNumber, Title, Summary, ChapterStatusId, Deadline, CreatedAt, CompletedAt)
    VALUES (@Lantern, 1, N'Festival of Returning Lights', N'The shrine apprentice meets a spirit who remembers every summer except her own.', @ChapterPublished, DATEADD(day,-12,SYSUTCDATETIME()), DATEADD(day,-48,SYSUTCDATETIME()), DATEADD(day,-14,SYSUTCDATETIME()));
SELECT @LanternCh1 = ChapterId FROM dbo.Chapters WHERE SeriesId = @Lantern AND ChapterNumber = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.Chapters WHERE SeriesId = @Signal AND ChapterNumber = 1)
    INSERT INTO dbo.Chapters (SeriesId, ChapterNumber, Title, Summary, ChapterStatusId, Deadline, CreatedAt)
    VALUES (@Signal, 1, N'Broadcast from Table Seven', N'A strange radio signal turns the cafe''s small stage into an interstellar concert.', @ChapterInProduction, DATEADD(day, 12, SYSUTCDATETIME()), DATEADD(day,-25,SYSUTCDATETIME()));
SELECT @SignalCh1 = ChapterId FROM dbo.Chapters WHERE SeriesId = @Signal AND ChapterNumber = 1;

DECLARE
    @NeonP1 uniqueidentifier, @NeonP2 uniqueidentifier, @NeonP3 uniqueidentifier, @NeonP4 uniqueidentifier,
    @MoonP1 uniqueidentifier, @MoonP2 uniqueidentifier,
    @AzureP1 uniqueidentifier, @AzureP2 uniqueidentifier,
    @ClockP1 uniqueidentifier,
    @LanternP1 uniqueidentifier, @LanternP2 uniqueidentifier,
    @SignalP1 uniqueidentifier, @SignalP2 uniqueidentifier;

-- Helper pattern: one page per natural key (chapter, page number, version).
IF NOT EXISTS (SELECT 1 FROM dbo.MangaPages WHERE ChapterId = @NeonCh1 AND PageNumber = 1 AND VersionNo = 1)
    INSERT INTO dbo.MangaPages (ChapterId, PageNumber, ImageUrl, ThumbnailUrl, FileName, ContentType, FileSizeBytes, VersionNo, PageStatusId, UploadedByUserId, UploadedAt)
    VALUES (@NeonCh1, 1, 'https://picsum.photos/seed/neon-ronin-page-01/900/1300', 'https://picsum.photos/seed/neon-ronin-page-01/320/460', 'neon-ronin-ch1-p01.png', 'image/png', 842120, 1, @PagePendingReview, @MangakaId, DATEADD(day,-12,SYSUTCDATETIME()));
SELECT @NeonP1 = PageId FROM dbo.MangaPages WHERE ChapterId = @NeonCh1 AND PageNumber = 1 AND VersionNo = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.MangaPages WHERE ChapterId = @NeonCh1 AND PageNumber = 2 AND VersionNo = 1)
    INSERT INTO dbo.MangaPages (ChapterId, PageNumber, ImageUrl, ThumbnailUrl, FileName, ContentType, FileSizeBytes, VersionNo, PageStatusId, UploadedByUserId, UploadedAt)
    VALUES (@NeonCh1, 2, 'https://picsum.photos/seed/neon-ronin-page-02/900/1300', 'https://picsum.photos/seed/neon-ronin-page-02/320/460', 'neon-ronin-ch1-p02.png', 'image/png', 813902, 1, @PageApproved, @MangakaId, DATEADD(day,-11,SYSUTCDATETIME()));
SELECT @NeonP2 = PageId FROM dbo.MangaPages WHERE ChapterId = @NeonCh1 AND PageNumber = 2 AND VersionNo = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.MangaPages WHERE ChapterId = @NeonCh2 AND PageNumber = 1 AND VersionNo = 1)
    INSERT INTO dbo.MangaPages (ChapterId, PageNumber, ImageUrl, ThumbnailUrl, FileName, ContentType, FileSizeBytes, VersionNo, PageStatusId, UploadedByUserId, UploadedAt)
    VALUES (@NeonCh2, 1, 'https://picsum.photos/seed/neon-ronin-page-03/900/1300', 'https://picsum.photos/seed/neon-ronin-page-03/320/460', 'neon-ronin-ch2-p01.png', 'image/png', 799001, 1, @PageInProduction, @MangakaId, DATEADD(day,-4,SYSUTCDATETIME()));
SELECT @NeonP3 = PageId FROM dbo.MangaPages WHERE ChapterId = @NeonCh2 AND PageNumber = 1 AND VersionNo = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.MangaPages WHERE ChapterId = @NeonCh2 AND PageNumber = 2 AND VersionNo = 1)
    INSERT INTO dbo.MangaPages (ChapterId, PageNumber, ImageUrl, ThumbnailUrl, FileName, ContentType, FileSizeBytes, VersionNo, PageStatusId, UploadedByUserId, UploadedAt)
    VALUES (@NeonCh2, 2, 'https://picsum.photos/seed/neon-ronin-page-04/900/1300', 'https://picsum.photos/seed/neon-ronin-page-04/320/460', 'neon-ronin-ch2-p02.png', 'image/png', 801450, 1, @PageInProduction, @MangakaId, DATEADD(day,-3,SYSUTCDATETIME()));
SELECT @NeonP4 = PageId FROM dbo.MangaPages WHERE ChapterId = @NeonCh2 AND PageNumber = 2 AND VersionNo = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.MangaPages WHERE ChapterId = @MoonCh1 AND PageNumber = 1 AND VersionNo = 1)
    INSERT INTO dbo.MangaPages (ChapterId, PageNumber, ImageUrl, ThumbnailUrl, FileName, ContentType, FileSizeBytes, VersionNo, PageStatusId, UploadedByUserId, UploadedAt)
    VALUES (@MoonCh1, 1, 'https://picsum.photos/seed/moonlit-bento-page-01/900/1300', 'https://picsum.photos/seed/moonlit-bento-page-01/320/460', 'moonlit-bento-ch1-p01.png', 'image/png', 702340, 1, @PagePendingReview, @MangakaId, DATEADD(day,-7,SYSUTCDATETIME()));
SELECT @MoonP1 = PageId FROM dbo.MangaPages WHERE ChapterId = @MoonCh1 AND PageNumber = 1 AND VersionNo = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.MangaPages WHERE ChapterId = @MoonCh1 AND PageNumber = 2 AND VersionNo = 1)
    INSERT INTO dbo.MangaPages (ChapterId, PageNumber, ImageUrl, ThumbnailUrl, FileName, ContentType, FileSizeBytes, VersionNo, PageStatusId, UploadedByUserId, UploadedAt)
    VALUES (@MoonCh1, 2, 'https://picsum.photos/seed/moonlit-bento-page-02/900/1300', 'https://picsum.photos/seed/moonlit-bento-page-02/320/460', 'moonlit-bento-ch1-p02.png', 'image/png', 709128, 1, @PageUploaded, @MangakaId, DATEADD(day,-6,SYSUTCDATETIME()));
SELECT @MoonP2 = PageId FROM dbo.MangaPages WHERE ChapterId = @MoonCh1 AND PageNumber = 2 AND VersionNo = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.MangaPages WHERE ChapterId = @AzureCh1 AND PageNumber = 1 AND VersionNo = 1)
    INSERT INTO dbo.MangaPages (ChapterId, PageNumber, ImageUrl, ThumbnailUrl, FileName, ContentType, FileSizeBytes, VersionNo, PageStatusId, UploadedByUserId, UploadedAt)
    VALUES (@AzureCh1, 1, 'https://picsum.photos/seed/azure-dragon-page-01/900/1300', 'https://picsum.photos/seed/azure-dragon-page-01/320/460', 'azure-dragon-ch1-p01.png', 'image/png', 932810, 1, @PagePendingReview, @Mangaka2Id, DATEADD(day,-8,SYSUTCDATETIME()));
SELECT @AzureP1 = PageId FROM dbo.MangaPages WHERE ChapterId = @AzureCh1 AND PageNumber = 1 AND VersionNo = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.MangaPages WHERE ChapterId = @AzureCh1 AND PageNumber = 2 AND VersionNo = 1)
    INSERT INTO dbo.MangaPages (ChapterId, PageNumber, ImageUrl, ThumbnailUrl, FileName, ContentType, FileSizeBytes, VersionNo, PageStatusId, UploadedByUserId, UploadedAt)
    VALUES (@AzureCh1, 2, 'https://picsum.photos/seed/azure-dragon-page-02/900/1300', 'https://picsum.photos/seed/azure-dragon-page-02/320/460', 'azure-dragon-ch1-p02.png', 'image/png', 914001, 1, @PageInProduction, @Mangaka2Id, DATEADD(day,-5,SYSUTCDATETIME()));
SELECT @AzureP2 = PageId FROM dbo.MangaPages WHERE ChapterId = @AzureCh1 AND PageNumber = 2 AND VersionNo = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.MangaPages WHERE ChapterId = @ClockCh1 AND PageNumber = 1 AND VersionNo = 1)
    INSERT INTO dbo.MangaPages (ChapterId, PageNumber, ImageUrl, ThumbnailUrl, FileName, ContentType, FileSizeBytes, VersionNo, PageStatusId, UploadedByUserId, UploadedAt)
    VALUES (@ClockCh1, 1, 'https://picsum.photos/seed/clockwork-sakura-page-01/900/1300', 'https://picsum.photos/seed/clockwork-sakura-page-01/320/460', 'clockwork-sakura-ch1-p01.png', 'image/png', 682010, 1, @PageUploaded, @MangakaId, DATEADD(day,-2,SYSUTCDATETIME()));
SELECT @ClockP1 = PageId FROM dbo.MangaPages WHERE ChapterId = @ClockCh1 AND PageNumber = 1 AND VersionNo = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.MangaPages WHERE ChapterId = @LanternCh1 AND PageNumber = 1 AND VersionNo = 1)
    INSERT INTO dbo.MangaPages (ChapterId, PageNumber, ImageUrl, ThumbnailUrl, FileName, ContentType, FileSizeBytes, VersionNo, PageStatusId, UploadedByUserId, UploadedAt)
    VALUES (@LanternCh1, 1, 'https://picsum.photos/seed/paper-lantern-page-01/900/1300', 'https://picsum.photos/seed/paper-lantern-page-01/320/460', 'paper-lantern-ch1-p01.png', 'image/png', 734212, 1, @PageApproved, @Mangaka2Id, DATEADD(day,-25,SYSUTCDATETIME()));
SELECT @LanternP1 = PageId FROM dbo.MangaPages WHERE ChapterId = @LanternCh1 AND PageNumber = 1 AND VersionNo = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.MangaPages WHERE ChapterId = @LanternCh1 AND PageNumber = 2 AND VersionNo = 1)
    INSERT INTO dbo.MangaPages (ChapterId, PageNumber, ImageUrl, ThumbnailUrl, FileName, ContentType, FileSizeBytes, VersionNo, PageStatusId, UploadedByUserId, UploadedAt)
    VALUES (@LanternCh1, 2, 'https://picsum.photos/seed/paper-lantern-page-02/900/1300', 'https://picsum.photos/seed/paper-lantern-page-02/320/460', 'paper-lantern-ch1-p02.png', 'image/png', 756112, 1, @PageApproved, @Mangaka2Id, DATEADD(day,-24,SYSUTCDATETIME()));
SELECT @LanternP2 = PageId FROM dbo.MangaPages WHERE ChapterId = @LanternCh1 AND PageNumber = 2 AND VersionNo = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.MangaPages WHERE ChapterId = @SignalCh1 AND PageNumber = 1 AND VersionNo = 1)
    INSERT INTO dbo.MangaPages (ChapterId, PageNumber, ImageUrl, ThumbnailUrl, FileName, ContentType, FileSizeBytes, VersionNo, PageStatusId, UploadedByUserId, UploadedAt)
    VALUES (@SignalCh1, 1, 'https://picsum.photos/seed/signal-star-page-01/900/1300', 'https://picsum.photos/seed/signal-star-page-01/320/460', 'signal-star-ch1-p01.png', 'image/png', 777321, 1, @PageInProduction, @MangakaId, DATEADD(day,-6,SYSUTCDATETIME()));
SELECT @SignalP1 = PageId FROM dbo.MangaPages WHERE ChapterId = @SignalCh1 AND PageNumber = 1 AND VersionNo = 1;

IF NOT EXISTS (SELECT 1 FROM dbo.MangaPages WHERE ChapterId = @SignalCh1 AND PageNumber = 2 AND VersionNo = 1)
    INSERT INTO dbo.MangaPages (ChapterId, PageNumber, ImageUrl, ThumbnailUrl, FileName, ContentType, FileSizeBytes, VersionNo, PageStatusId, UploadedByUserId, UploadedAt)
    VALUES (@SignalCh1, 2, 'https://picsum.photos/seed/signal-star-page-02/900/1300', 'https://picsum.photos/seed/signal-star-page-02/320/460', 'signal-star-ch1-p02.png', 'image/png', 766210, 1, @PageUploaded, @MangakaId, DATEADD(day,-4,SYSUTCDATETIME()));
SELECT @SignalP2 = PageId FROM dbo.MangaPages WHERE ChapterId = @SignalCh1 AND PageNumber = 2 AND VersionNo = 1;

/* ------------------------------------------------------------
   4. Regions, tasks, submissions, earnings, and editor comments
   ------------------------------------------------------------ */

DECLARE
    @RNeonChar uniqueidentifier, @RNeonSpeech uniqueidentifier, @RNeonBg uniqueidentifier, @RNeonFx uniqueidentifier,
    @RMoonSpeech uniqueidentifier, @RMoonBg uniqueidentifier,
    @RAzureDragon uniqueidentifier, @RSignalStage uniqueidentifier;

IF NOT EXISTS (SELECT 1 FROM dbo.PageRegions WHERE PageId = @NeonP1 AND Label = N'Hero character face')
    INSERT INTO dbo.PageRegions (PageId, RegionTypeId, X, Y, Width, Height, Label, Confidence, SourceType, CreatedByUserId)
    VALUES (@NeonP1, @RegionCharacter, 12.00, 8.00, 38.00, 52.00, N'Hero character face', 0.98, 'Manual', @MangakaId);
SELECT @RNeonChar = RegionId FROM dbo.PageRegions WHERE PageId = @NeonP1 AND Label = N'Hero character face';

IF NOT EXISTS (SELECT 1 FROM dbo.PageRegions WHERE PageId = @NeonP1 AND Label = N'Opening speech bubbles')
    INSERT INTO dbo.PageRegions (PageId, RegionTypeId, X, Y, Width, Height, Label, Confidence, SourceType, CreatedByUserId)
    VALUES (@NeonP1, @RegionSpeech, 52.00, 10.00, 32.00, 20.00, N'Opening speech bubbles', 0.93, 'AI', @MangakaId);
SELECT @RNeonSpeech = RegionId FROM dbo.PageRegions WHERE PageId = @NeonP1 AND Label = N'Opening speech bubbles';

IF NOT EXISTS (SELECT 1 FROM dbo.PageRegions WHERE PageId = @NeonP2 AND Label = N'Cyber dojo background')
    INSERT INTO dbo.PageRegions (PageId, RegionTypeId, X, Y, Width, Height, Label, Confidence, SourceType, CreatedByUserId)
    VALUES (@NeonP2, @RegionBackground, 0.00, 0.00, 100.00, 65.00, N'Cyber dojo background', 0.89, 'Manual', @EditorId);
SELECT @RNeonBg = RegionId FROM dbo.PageRegions WHERE PageId = @NeonP2 AND Label = N'Cyber dojo background';

IF NOT EXISTS (SELECT 1 FROM dbo.PageRegions WHERE PageId = @NeonP3 AND Label = N'Hologram slash effect')
    INSERT INTO dbo.PageRegions (PageId, RegionTypeId, X, Y, Width, Height, Label, Confidence, SourceType, CreatedByUserId)
    VALUES (@NeonP3, @RegionEffect, 20.00, 30.00, 58.00, 28.00, N'Hologram slash effect', 0.87, 'AI', @MangakaId);
SELECT @RNeonFx = RegionId FROM dbo.PageRegions WHERE PageId = @NeonP3 AND Label = N'Hologram slash effect';

IF NOT EXISTS (SELECT 1 FROM dbo.PageRegions WHERE PageId = @MoonP1 AND Label = N'Kitchen clue bubbles')
    INSERT INTO dbo.PageRegions (PageId, RegionTypeId, X, Y, Width, Height, Label, Confidence, SourceType, CreatedByUserId)
    VALUES (@MoonP1, @RegionSpeech, 15.00, 6.00, 70.00, 22.00, N'Kitchen clue bubbles', 0.91, 'Manual', @MangakaId);
SELECT @RMoonSpeech = RegionId FROM dbo.PageRegions WHERE PageId = @MoonP1 AND Label = N'Kitchen clue bubbles';

IF NOT EXISTS (SELECT 1 FROM dbo.PageRegions WHERE PageId = @MoonP2 AND Label = N'Night market background')
    INSERT INTO dbo.PageRegions (PageId, RegionTypeId, X, Y, Width, Height, Label, Confidence, SourceType, CreatedByUserId)
    VALUES (@MoonP2, @RegionBackground, 0.00, 0.00, 100.00, 76.00, N'Night market background', 0.84, 'Manual', @MangakaId);
SELECT @RMoonBg = RegionId FROM dbo.PageRegions WHERE PageId = @MoonP2 AND Label = N'Night market background';

IF NOT EXISTS (SELECT 1 FROM dbo.PageRegions WHERE PageId = @AzureP1 AND Label = N'Dragon wing shadow')
    INSERT INTO dbo.PageRegions (PageId, RegionTypeId, X, Y, Width, Height, Label, Confidence, SourceType, CreatedByUserId)
    VALUES (@AzureP1, @RegionShading, 30.00, 18.00, 45.00, 44.00, N'Dragon wing shadow', 0.90, 'AI', @Mangaka2Id);
SELECT @RAzureDragon = RegionId FROM dbo.PageRegions WHERE PageId = @AzureP1 AND Label = N'Dragon wing shadow';

IF NOT EXISTS (SELECT 1 FROM dbo.PageRegions WHERE PageId = @SignalP1 AND Label = N'Cafe stage crowd')
    INSERT INTO dbo.PageRegions (PageId, RegionTypeId, X, Y, Width, Height, Label, Confidence, SourceType, CreatedByUserId)
    VALUES (@SignalP1, @RegionBackground, 4.00, 40.00, 92.00, 50.00, N'Cafe stage crowd', 0.86, 'Manual', @MangakaId);
SELECT @RSignalStage = RegionId FROM dbo.PageRegions WHERE PageId = @SignalP1 AND Label = N'Cafe stage crowd';

DECLARE
    @TNeonClean uniqueidentifier, @TNeonSpeech uniqueidentifier, @TNeonBg uniqueidentifier, @TNeonFx uniqueidentifier,
    @TMoonSpeech uniqueidentifier, @TMoonBg uniqueidentifier,
    @TAzureShade uniqueidentifier, @TSignalStage uniqueidentifier;

IF NOT EXISTS (SELECT 1 FROM dbo.ProductionTasks WHERE PageId = @NeonP1 AND Title = N'Clean hero line art')
    INSERT INTO dbo.ProductionTasks (PageId, RegionId, AssignedToAssistantId, CreatedByMangakaId, TaskTypeId, TaskStatusId, Title, Description, Priority, Deadline, Price, CreatedAt, StartedAt)
    VALUES (@NeonP1, @RNeonChar, @AssistantId, @MangakaId, @TypeCleanLine, @TaskInProgress, N'Clean hero line art', N'Polish face contour and sword hand; preserve expression intensity.', 2, DATEADD(day, 2, SYSUTCDATETIME()), 350000, DATEADD(day,-4,SYSUTCDATETIME()), DATEADD(day,-3,SYSUTCDATETIME()));
SELECT @TNeonClean = TaskId FROM dbo.ProductionTasks WHERE PageId = @NeonP1 AND Title = N'Clean hero line art';

IF NOT EXISTS (SELECT 1 FROM dbo.ProductionTasks WHERE PageId = @NeonP1 AND Title = N'Letter opening challenge dialogue')
    INSERT INTO dbo.ProductionTasks (PageId, RegionId, AssignedToAssistantId, CreatedByMangakaId, TaskTypeId, TaskStatusId, Title, Description, Priority, Deadline, Price, CreatedAt, StartedAt, CompletedAt)
    VALUES (@NeonP1, @RNeonSpeech, @Assistant2Id, @MangakaId, @TypeSpeech, @TaskSubmitted, N'Letter opening challenge dialogue', N'Clean speech bubbles and align Vietnamese dialogue with reading order.', 1, DATEADD(day, 1, SYSUTCDATETIME()), 180000, DATEADD(day,-5,SYSUTCDATETIME()), DATEADD(day,-4,SYSUTCDATETIME()), DATEADD(hour,-6,SYSUTCDATETIME()));
SELECT @TNeonSpeech = TaskId FROM dbo.ProductionTasks WHERE PageId = @NeonP1 AND Title = N'Letter opening challenge dialogue';

IF NOT EXISTS (SELECT 1 FROM dbo.ProductionTasks WHERE PageId = @NeonP2 AND Title = N'Paint cyber dojo background')
    INSERT INTO dbo.ProductionTasks (PageId, RegionId, AssignedToAssistantId, CreatedByMangakaId, TaskTypeId, TaskStatusId, Title, Description, Priority, Deadline, Price, CreatedAt, StartedAt, CompletedAt)
    VALUES (@NeonP2, @RNeonBg, @AssistantId, @MangakaId, @TypeBackground, @TaskApproved, N'Paint cyber dojo background', N'Add depth, holographic panels, and high-contrast rim light.', 2, DATEADD(day,-2,SYSUTCDATETIME()), 420000, DATEADD(day,-10,SYSUTCDATETIME()), DATEADD(day,-9,SYSUTCDATETIME()), DATEADD(day,-3,SYSUTCDATETIME()));
SELECT @TNeonBg = TaskId FROM dbo.ProductionTasks WHERE PageId = @NeonP2 AND Title = N'Paint cyber dojo background';

IF NOT EXISTS (SELECT 1 FROM dbo.ProductionTasks WHERE PageId = @NeonP3 AND Title = N'Add hologram slash effects')
    INSERT INTO dbo.ProductionTasks (PageId, RegionId, AssignedToAssistantId, CreatedByMangakaId, TaskTypeId, TaskStatusId, Title, Description, Priority, Deadline, Price, CreatedAt, StartedAt)
    VALUES (@NeonP3, @RNeonFx, @Assistant3Id, @MangakaId, @TypeEffect, @TaskOverdue, N'Add hologram slash effects', N'Create impact streaks and glitch shards. This is intentionally overdue for worker demo.', 3, DATEADD(day,-1,SYSUTCDATETIME()), 280000, DATEADD(day,-6,SYSUTCDATETIME()), DATEADD(day,-5,SYSUTCDATETIME()));
SELECT @TNeonFx = TaskId FROM dbo.ProductionTasks WHERE PageId = @NeonP3 AND Title = N'Add hologram slash effects';

IF NOT EXISTS (SELECT 1 FROM dbo.ProductionTasks WHERE PageId = @MoonP1 AND Title = N'Clean bento clue dialogue')
    INSERT INTO dbo.ProductionTasks (PageId, RegionId, AssignedToAssistantId, CreatedByMangakaId, TaskTypeId, TaskStatusId, Title, Description, Priority, Deadline, Price, CreatedAt, StartedAt, CompletedAt)
    VALUES (@MoonP1, @RMoonSpeech, @AssistantId, @MangakaId, @TypeSpeech, @TaskSubmitted, N'Clean bento clue dialogue', N'Letter food clue bubbles; emphasize punchline panel.', 1, DATEADD(day, 1, SYSUTCDATETIME()), 160000, DATEADD(day,-3,SYSUTCDATETIME()), DATEADD(day,-2,SYSUTCDATETIME()), DATEADD(hour,-2,SYSUTCDATETIME()));
SELECT @TMoonSpeech = TaskId FROM dbo.ProductionTasks WHERE PageId = @MoonP1 AND Title = N'Clean bento clue dialogue';

IF NOT EXISTS (SELECT 1 FROM dbo.ProductionTasks WHERE PageId = @MoonP2 AND Title = N'Render midnight market stalls')
    INSERT INTO dbo.ProductionTasks (PageId, RegionId, AssignedToAssistantId, CreatedByMangakaId, TaskTypeId, TaskStatusId, Title, Description, Priority, Deadline, Price, CreatedAt)
    VALUES (@MoonP2, @RMoonBg, @Assistant2Id, @MangakaId, @TypeBackground, @TaskAssigned, N'Render midnight market stalls', N'Background details for food stalls, crowd, and signage.', 2, DATEADD(day, 5, SYSUTCDATETIME()), 300000, DATEADD(day,-1,SYSUTCDATETIME()));
SELECT @TMoonBg = TaskId FROM dbo.ProductionTasks WHERE PageId = @MoonP2 AND Title = N'Render midnight market stalls';

IF NOT EXISTS (SELECT 1 FROM dbo.ProductionTasks WHERE PageId = @AzureP1 AND Title = N'Shade dragon wing over courier')
    INSERT INTO dbo.ProductionTasks (PageId, RegionId, AssignedToAssistantId, CreatedByMangakaId, TaskTypeId, TaskStatusId, Title, Description, Priority, Deadline, Price, CreatedAt, StartedAt)
    VALUES (@AzureP1, @RAzureDragon, @Assistant2Id, @Mangaka2Id, @TypeShading, @TaskInProgress, N'Shade dragon wing over courier', N'Soft shadow pass over courier cloak and wing edge.', 2, DATEADD(day, 3, SYSUTCDATETIME()), 260000, DATEADD(day,-2,SYSUTCDATETIME()), DATEADD(day,-1,SYSUTCDATETIME()));
SELECT @TAzureShade = TaskId FROM dbo.ProductionTasks WHERE PageId = @AzureP1 AND Title = N'Shade dragon wing over courier';

IF NOT EXISTS (SELECT 1 FROM dbo.ProductionTasks WHERE PageId = @SignalP1 AND Title = N'Background crowd toning')
    INSERT INTO dbo.ProductionTasks (PageId, RegionId, AssignedToAssistantId, CreatedByMangakaId, TaskTypeId, TaskStatusId, Title, Description, Priority, Deadline, Price, CreatedAt)
    VALUES (@SignalP1, @RSignalStage, @Assistant3Id, @MangakaId, @TypeTone, @TaskAssigned, N'Background crowd toning', N'Tone cafe audience while keeping idols readable.', 1, DATEADD(day, 4, SYSUTCDATETIME()), 210000, DATEADD(day,-1,SYSUTCDATETIME()));
SELECT @TSignalStage = TaskId FROM dbo.ProductionTasks WHERE PageId = @SignalP1 AND Title = N'Background crowd toning';

-- Submissions for review pages and workflow demo.
IF NOT EXISTS (SELECT 1 FROM dbo.TaskSubmissions WHERE TaskId = @TNeonSpeech)
    INSERT INTO dbo.TaskSubmissions (TaskId, SubmittedByAssistantId, FileUrl, FileName, ContentType, FileSizeBytes, Comment, SubmissionStatusId, SubmittedAt)
    VALUES (@TNeonSpeech, @Assistant2Id, 'https://picsum.photos/seed/neon-lettering-submission/900/1300', 'neon-lettering-submission.png', 'image/png', 542120, N'Lettering pass complete; please check bubble 3 emphasis.', @SubSubmitted, DATEADD(hour,-6,SYSUTCDATETIME()));

IF NOT EXISTS (SELECT 1 FROM dbo.TaskSubmissions WHERE TaskId = @TMoonSpeech)
    INSERT INTO dbo.TaskSubmissions (TaskId, SubmittedByAssistantId, FileUrl, FileName, ContentType, FileSizeBytes, Comment, SubmissionStatusId, SubmittedAt)
    VALUES (@TMoonSpeech, @AssistantId, 'https://picsum.photos/seed/moon-bento-submission/900/1300', 'moon-bento-dialogue-submission.png', 'image/png', 488201, N'Cleaned dialogue and added small laugh SFX.', @SubSubmitted, DATEADD(hour,-2,SYSUTCDATETIME()));

IF NOT EXISTS (SELECT 1 FROM dbo.TaskSubmissions WHERE TaskId = @TNeonBg)
    INSERT INTO dbo.TaskSubmissions (TaskId, SubmittedByAssistantId, FileUrl, FileName, ContentType, FileSizeBytes, Comment, SubmissionStatusId, SubmittedAt, ReviewedAt, ReviewedByMangakaId, ReviewNote)
    VALUES (@TNeonBg, @AssistantId, 'https://picsum.photos/seed/neon-background-approved/900/1300', 'neon-background-approved.png', 'image/png', 603430, N'Final background pass uploaded.', @SubApproved, DATEADD(day,-4,SYSUTCDATETIME()), DATEADD(day,-3,SYSUTCDATETIME()), @MangakaId, N'Approved. Strong lighting and perspective.');

IF NOT EXISTS (SELECT 1 FROM dbo.AssistantEarnings WHERE TaskId = @TNeonBg)
    INSERT INTO dbo.AssistantEarnings (AssistantId, TaskId, Amount, EarningStatusId, CalculatedAt, PaidAt)
    VALUES (@AssistantId, @TNeonBg, 420000, COALESCE(@EarnPaid, @EarnPending), DATEADD(day,-3,SYSUTCDATETIME()), DATEADD(day,-2,SYSUTCDATETIME()));

-- Editor comments to populate editor workflow.
IF NOT EXISTS (SELECT 1 FROM dbo.EditorComments WHERE PageId = @NeonP1 AND CommentText = N'Panel 2 eye-line needs stronger focus before final approval.')
    INSERT INTO dbo.EditorComments (PageId, EditorId, X, Y, Width, Height, CommentText, CommentStatusId, CreatedAt)
    VALUES (@NeonP1, @EditorId, 20.00, 15.00, 28.00, 18.00, N'Panel 2 eye-line needs stronger focus before final approval.', @CommentOpen, DATEADD(hour,-10,SYSUTCDATETIME()));

IF NOT EXISTS (SELECT 1 FROM dbo.EditorComments WHERE PageId = @LanternP1 AND CommentText = N'Resolved: festival lantern density looks balanced.')
    INSERT INTO dbo.EditorComments (PageId, EditorId, X, Y, Width, Height, CommentText, CommentStatusId, CreatedAt, ResolvedAt, ResolvedByUserId)
    VALUES (@LanternP1, @EditorId, 10.00, 12.00, 35.00, 25.00, N'Resolved: festival lantern density looks balanced.', @CommentResolved, DATEADD(day,-18,SYSUTCDATETIME()), DATEADD(day,-16,SYSUTCDATETIME()), @EditorId);

/* ------------------------------------------------------------
   5. Board votes, rankings, reader votes, publications
   ------------------------------------------------------------ */

INSERT INTO dbo.BoardVotes (SeriesId, BoardMemberId, VoteValueId, Comment, CreatedAt)
SELECT @Azure, v.BoardMemberId, v.VoteValueId, v.Comment, v.CreatedAt
FROM (VALUES
    (@BoardId, @VoteApprove, N'Strong visual hook and very demo-friendly worldbuilding.', DATEADD(day,-2,SYSUTCDATETIME())),
    (@Board2Id, @VoteNeedRevision, N'Approve after tightening the first delivery stakes.', DATEADD(day,-1,SYSUTCDATETIME()))
) AS v(BoardMemberId, VoteValueId, Comment, CreatedAt)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.BoardVotes bv WHERE bv.SeriesId = @Azure AND bv.BoardMemberId = v.BoardMemberId
);

-- Keep Moonlit with zero votes so Board Review has a clean pending item.

INSERT INTO dbo.RankingRecords (SeriesId, IssueNumber, VoteCount, RankPosition, PreviousRankPosition, Trend, CalculatedAt)
SELECT r.SeriesId, r.IssueNumber, r.VoteCount, r.RankPosition, r.PreviousRankPosition, r.Trend, r.CalculatedAt
FROM (VALUES
    (@Neon, N'2026-W30', 18942, 2, 4, 'Up', DATEADD(hour,-8,SYSUTCDATETIME())),
    (@Lantern, N'2026-W30', 16618, 4, 3, 'Down', DATEADD(hour,-8,SYSUTCDATETIME())),
    (@Signal, N'2026-W30', 10305, 11, 8, 'Down', DATEADD(hour,-8,SYSUTCDATETIME())),
    (@Azure, N'2026-W30', 14220, 6, 9, 'Up', DATEADD(hour,-8,SYSUTCDATETIME())),
    (@Moonlit, N'2026-W30', 8104, 18, 24, 'Up', DATEADD(hour,-8,SYSUTCDATETIME()))
) AS r(SeriesId, IssueNumber, VoteCount, RankPosition, PreviousRankPosition, Trend, CalculatedAt)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.RankingRecords rr WHERE rr.SeriesId = r.SeriesId AND rr.IssueNumber = r.IssueNumber
);

INSERT INTO dbo.ReaderVoteData (SeriesId, IssueNumber, VoteCount, RankPosition, ImportedByUserId, ImportedAt, Notes)
SELECT r.SeriesId, r.IssueNumber, r.VoteCount, r.RankPosition, @AdminId, r.ImportedAt, r.Notes
FROM (VALUES
    (@Neon, N'2026-W30', 18942, 2, DATEADD(hour,-10,SYSUTCDATETIME()), N'Imported from simulated reader survey batch A.'),
    (@Lantern, N'2026-W30', 16618, 4, DATEADD(hour,-10,SYSUTCDATETIME()), N'Festival arc continued to perform strongly.'),
    (@Signal, N'2026-W30', 10305, 11, DATEADD(hour,-10,SYSUTCDATETIME()), N'Ranking risk candidate for worker demo.'),
    (@Azure, N'2026-W30', 14220, 6, DATEADD(hour,-10,SYSUTCDATETIME()), N'Newcomer lift after board buzz.'),
    (@Moonlit, N'2026-W30', 8104, 18, DATEADD(hour,-10,SYSUTCDATETIME()), N'Pending board review; use as before/after comparison.')
) AS r(SeriesId, IssueNumber, VoteCount, RankPosition, ImportedAt, Notes)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.ReaderVoteData rvd WHERE rvd.SeriesId = r.SeriesId AND rvd.IssueNumber = r.IssueNumber
);

IF NOT EXISTS (SELECT 1 FROM dbo.ChapterPublications WHERE ChapterId = @LanternCh1)
    INSERT INTO dbo.ChapterPublications (ChapterId, PublicationIssueId, PublicationOrder)
    VALUES (@LanternCh1, @IssueW30, 3);

IF NOT EXISTS (SELECT 1 FROM dbo.ChapterPublications WHERE ChapterId = @NeonCh1)
    INSERT INTO dbo.ChapterPublications (ChapterId, PublicationIssueId, PublicationOrder)
    VALUES (@NeonCh1, @IssueW31, 1);

/* ------------------------------------------------------------
   6. Notifications, worker logs, workflow history
   ------------------------------------------------------------ */

DECLARE
    @NotifTaskAssigned int = (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'TaskAssigned'),
    @NotifSubmissionUploaded int = (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'SubmissionUploaded'),
    @NotifRankingRisk int = (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'RankingRisk'),
    @NotifSeriesSubmitted int = (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'SeriesSubmitted'),
    @NotifBoardVote int = (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'BoardVoteCast'),
    @NotifSystem int = (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'System');

INSERT INTO dbo.Notifications (UserId, NotificationTypeId, Title, Message, ReferenceType, ReferenceId, IsRead, CreatedAt)
SELECT n.UserId, n.NotificationTypeId, n.Title, n.Message, n.ReferenceType, n.ReferenceId, n.IsRead, n.CreatedAt
FROM (VALUES
    (@AssistantId, @NotifTaskAssigned, N'New high-priority task assigned', N'Clean hero line art for Neon Ronin Academy is ready to start.', 'ProductionTask', @TNeonClean, CAST(0 AS bit), DATEADD(hour,-5,SYSUTCDATETIME())),
    (@Assistant2Id, @NotifTaskAssigned, N'Background task assigned', N'Render midnight market stalls for Moonlit Bento Detective.', 'ProductionTask', @TMoonBg, CAST(0 AS bit), DATEADD(hour,-3,SYSUTCDATETIME())),
    (@MangakaId, @NotifSubmissionUploaded, N'New submission waiting for review', N'Assistant submitted lettering for Neon Ronin Academy.', 'TaskSubmission', @TNeonSpeech, CAST(0 AS bit), DATEADD(hour,-6,SYSUTCDATETIME())),
    (@MangakaId, @NotifSubmissionUploaded, N'Bento clue dialogue submitted', N'Moonlit Bento Detective has a submitted dialogue cleanup.', 'TaskSubmission', @TMoonSpeech, CAST(0 AS bit), DATEADD(hour,-2,SYSUTCDATETIME())),
    (@AdminId, @NotifRankingRisk, N'Ranking risk detected', N'Signal Star Cafe dropped three positions and needs review.', 'Series', @Signal, CAST(0 AS bit), DATEADD(hour,-8,SYSUTCDATETIME())),
    (@BoardId, @NotifSeriesSubmitted, N'New series awaiting board review', N'Moonlit Bento Detective is ready for your vote.', 'Series', @Moonlit, CAST(0 AS bit), DATEADD(day,-1,SYSUTCDATETIME())),
    (@Mangaka2Id, @NotifBoardVote, N'Board vote recorded', N'Azure Dragon Courier received a board vote.', 'Series', @Azure, CAST(0 AS bit), DATEADD(day,-1,SYSUTCDATETIME())),
    (@AdminId, @NotifSystem, N'Demo showcase data loaded', N'Full demo data with images, tasks, rankings, and notifications is available.', 'Series', @Neon, CAST(0 AS bit), SYSUTCDATETIME())
) AS n(UserId, NotificationTypeId, Title, Message, ReferenceType, ReferenceId, IsRead, CreatedAt)
WHERE n.NotificationTypeId IS NOT NULL
  AND NOT EXISTS (
    SELECT 1 FROM dbo.Notifications existing
    WHERE existing.UserId = n.UserId
      AND existing.NotificationTypeId = n.NotificationTypeId
      AND existing.Title = n.Title
      AND existing.ReferenceId = n.ReferenceId
  );

INSERT INTO dbo.BackgroundJobLogs (JobName, Status, Message, StartedAt, FinishedAt)
SELECT j.JobName, j.Status, j.Message, j.StartedAt, j.FinishedAt
FROM (VALUES
    (N'DeadlineReminderWorker', 'Succeeded', N'Demo run: 5 deadline reminders sent for active manga tasks.', DATEADD(hour,-2,SYSUTCDATETIME()), DATEADD(hour,-2,DATEADD(second,9,SYSUTCDATETIME()))),
    (N'OverdueTaskScannerWorker', 'Succeeded', N'Demo run: 1 overdue task detected for Neon Ronin Academy.', DATEADD(hour,-1,SYSUTCDATETIME()), DATEADD(hour,-1,DATEADD(second,7,SYSUTCDATETIME()))),
    (N'RankingRiskWorker', 'Succeeded', N'Demo run: Signal Star Cafe flagged as ranking risk.', DATEADD(minute,-45,SYSUTCDATETIME()), DATEADD(minute,-44,SYSUTCDATETIME())),
    (N'MonthlyEarningCalculatorWorker', 'Succeeded', N'Demo run: assistant earnings calculated for approved production work.', DATEADD(minute,-25,SYSUTCDATETIME()), DATEADD(minute,-24,SYSUTCDATETIME()))
) AS j(JobName, Status, Message, StartedAt, FinishedAt)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.BackgroundJobLogs b
    WHERE b.JobName = j.JobName AND b.Message = j.Message
);

INSERT INTO dbo.WorkflowStatusHistories (EntityName, EntityId, FromStatusCode, ToStatusCode, ChangedByUserId, Note, ChangedAt)
SELECT h.EntityName, h.EntityId, h.FromStatusCode, h.ToStatusCode, h.ChangedByUserId, h.Note, h.ChangedAt
FROM (VALUES
    (N'Series', @Moonlit, 'Draft', 'Submitted', @MangakaId, N'Mangaka submitted Moonlit Bento Detective for board review.', DATEADD(day,-1,SYSUTCDATETIME())),
    (N'Series', @Azure, 'Submitted', 'UnderReview', @BoardId, N'Board review started after first vote.', DATEADD(day,-2,SYSUTCDATETIME())),
    (N'ProductionTask', @TNeonClean, 'Assigned', 'InProgress', @AssistantId, N'Assistant started clean line work.', DATEADD(day,-3,SYSUTCDATETIME())),
    (N'ProductionTask', @TNeonSpeech, 'InProgress', 'Submitted', @Assistant2Id, N'Assistant uploaded lettering submission.', DATEADD(hour,-6,SYSUTCDATETIME()))
) AS h(EntityName, EntityId, FromStatusCode, ToStatusCode, ChangedByUserId, Note, ChangedAt)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.WorkflowStatusHistories wh
    WHERE wh.EntityName = h.EntityName AND wh.EntityId = h.EntityId AND wh.ToStatusCode = h.ToStatusCode AND wh.Note = h.Note
);

/* ------------------------------------------------------------
   7. AI Studio demo data
   ------------------------------------------------------------ */

DECLARE @ModelVersion uniqueidentifier, @Inference uniqueidentifier, @AiDetected uniqueidentifier;

IF NOT EXISTS (SELECT 1 FROM dbo.AiModelVersions WHERE ModelName = N'Demo YOLO Manga Panel Segmenter' AND VersionLabel = N'v1-demo-showcase')
    INSERT INTO dbo.AiModelVersions (ModelName, ModelType, VersionLabel, Framework, ModelPath, IsActive, Notes)
    VALUES (N'Demo YOLO Manga Panel Segmenter', 'Segmentation', N'v1-demo-showcase', 'PyTorch', 'https://example.com/models/manga-yolo-v1-demo.onnx', 1, N'Demo metadata only; app can fall back to mock inference.');
SELECT @ModelVersion = ModelVersionId FROM dbo.AiModelVersions WHERE ModelName = N'Demo YOLO Manga Panel Segmenter' AND VersionLabel = N'v1-demo-showcase';

IF NOT EXISTS (SELECT 1 FROM dbo.AiInferenceRequests WHERE PageId = @NeonP1 AND RequestType = 'Segmentation' AND Status = 'Completed')
    INSERT INTO dbo.AiInferenceRequests (PageId, RequestedByUserId, ModelVersionId, RequestType, Status, StartedAt, FinishedAt)
    VALUES (@NeonP1, @MangakaId, @ModelVersion, 'Segmentation', 'Completed', DATEADD(hour,-12,SYSUTCDATETIME()), DATEADD(hour,-12,DATEADD(second,4,SYSUTCDATETIME())));
SELECT TOP 1 @Inference = InferenceRequestId FROM dbo.AiInferenceRequests WHERE PageId = @NeonP1 AND RequestType = 'Segmentation' AND Status = 'Completed' ORDER BY CreatedAt DESC;

IF NOT EXISTS (SELECT 1 FROM dbo.AiDetectedRegions WHERE InferenceRequestId = @Inference AND RegionTypeCode = 'SpeechBubble' AND X = 52.00)
    INSERT INTO dbo.AiDetectedRegions (InferenceRequestId, PageId, PageRegionId, RegionTypeCode, X, Y, Width, Height, Confidence, PolygonJson, IsAccepted)
    VALUES (@Inference, @NeonP1, @RNeonSpeech, 'SpeechBubble', 52.00, 10.00, 32.00, 20.00, 0.9321, N'[[52,10],[84,10],[84,30],[52,30]]', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.AiTaskSuggestions WHERE PageId = @NeonP1 AND Title = N'AI suggestion: clean opening speech bubbles')
    INSERT INTO dbo.AiTaskSuggestions (PageId, PageRegionId, SuggestedAssistantId, TaskTypeCode, Title, Description, ComplexityScore, EstimatedHours, EstimatedAmount, Status)
    VALUES (@NeonP1, @RNeonSpeech, @Assistant2Id, 'SpeechBubbleCleanup', N'AI suggestion: clean opening speech bubbles', N'AI detected dense opening dialogue. Suggested lettering cleanup and bubble alignment.', 1.50, 2.00, 180000, 'Approved');

/* ------------------------------------------------------------
   8. Final summary
   ------------------------------------------------------------ */

COMMIT TRANSACTION;

SELECT 'Series' AS Entity, COUNT(*) AS TotalRows FROM dbo.Series
UNION ALL SELECT 'Chapters', COUNT(*) FROM dbo.Chapters
UNION ALL SELECT 'MangaPages', COUNT(*) FROM dbo.MangaPages
UNION ALL SELECT 'PageRegions', COUNT(*) FROM dbo.PageRegions
UNION ALL SELECT 'ProductionTasks', COUNT(*) FROM dbo.ProductionTasks
UNION ALL SELECT 'TaskSubmissions', COUNT(*) FROM dbo.TaskSubmissions
UNION ALL SELECT 'EditorComments', COUNT(*) FROM dbo.EditorComments
UNION ALL SELECT 'BoardVotes', COUNT(*) FROM dbo.BoardVotes
UNION ALL SELECT 'RankingRecords', COUNT(*) FROM dbo.RankingRecords
UNION ALL SELECT 'ReaderVoteData', COUNT(*) FROM dbo.ReaderVoteData
UNION ALL SELECT 'Notifications', COUNT(*) FROM dbo.Notifications
UNION ALL SELECT 'BackgroundJobLogs', COUNT(*) FROM dbo.BackgroundJobLogs
UNION ALL SELECT 'AiDetectedRegions', COUNT(*) FROM dbo.AiDetectedRegions
ORDER BY Entity;
