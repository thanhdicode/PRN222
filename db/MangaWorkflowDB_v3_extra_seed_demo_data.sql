
/*
PRN222 - Manga Creation Workflow and Publishing Management System
Extra Seed Script v3.0
Run this AFTER MangaWorkflowDB_v2_demo_ready.sql.

Purpose:
- Add richer demo data for coding MVC/Razor Pages/Blazor/SignalR/Worker Service.
- Provide records for every major status/filter screen.
- Safe to re-run: uses a sentinel check to avoid duplicate business data.

Database: MangaWorkflowDB
*/

USE MangaWorkflowDB;
GO

SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF EXISTS (SELECT 1 FROM dbo.Series WHERE Title = N'Neon Samurai')
    BEGIN
        PRINT N'Extra seed data already exists. No duplicate data inserted.';
        COMMIT TRANSACTION;

        SELECT 'Extra seed already exists. Nothing inserted.' AS Message;
        SELECT COUNT(*) AS TotalUsers FROM dbo.Users;
        SELECT COUNT(*) AS TotalSeries FROM dbo.Series;
        SELECT COUNT(*) AS TotalChapters FROM dbo.Chapters;
        SELECT COUNT(*) AS TotalPages FROM dbo.MangaPages;
        SELECT COUNT(*) AS TotalTasks FROM dbo.ProductionTasks;
        SELECT COUNT(*) AS TotalNotifications FROM dbo.Notifications;
        RETURN;
    END

    DECLARE @Now DATETIME2 = SYSUTCDATETIME();

    /* ============================================================
       1. LOOKUP IDS
       ============================================================ */

    DECLARE @RoleAdmin INT = (SELECT RoleId FROM dbo.Roles WHERE RoleCode = 'Admin');
    DECLARE @RoleMangaka INT = (SELECT RoleId FROM dbo.Roles WHERE RoleCode = 'Mangaka');
    DECLARE @RoleAssistant INT = (SELECT RoleId FROM dbo.Roles WHERE RoleCode = 'Assistant');
    DECLARE @RoleEditor INT = (SELECT RoleId FROM dbo.Roles WHERE RoleCode = 'TantouEditor');
    DECLARE @RoleBoard INT = (SELECT RoleId FROM dbo.Roles WHERE RoleCode = 'EditorialBoard');

    DECLARE @SeriesDraft INT = (SELECT SeriesStatusId FROM dbo.SeriesStatuses WHERE StatusCode = 'Draft');
    DECLARE @SeriesSubmitted INT = (SELECT SeriesStatusId FROM dbo.SeriesStatuses WHERE StatusCode = 'Submitted');
    DECLARE @SeriesUnderReview INT = (SELECT SeriesStatusId FROM dbo.SeriesStatuses WHERE StatusCode = 'UnderReview');
    DECLARE @SeriesRevisionRequired INT = (SELECT SeriesStatusId FROM dbo.SeriesStatuses WHERE StatusCode = 'RevisionRequired');
    DECLARE @SeriesApproved INT = (SELECT SeriesStatusId FROM dbo.SeriesStatuses WHERE StatusCode = 'Approved');
    DECLARE @SeriesRejected INT = (SELECT SeriesStatusId FROM dbo.SeriesStatuses WHERE StatusCode = 'Rejected');
    DECLARE @SeriesPublishing INT = (SELECT SeriesStatusId FROM dbo.SeriesStatuses WHERE StatusCode = 'Publishing');
    DECLARE @SeriesCancelled INT = (SELECT SeriesStatusId FROM dbo.SeriesStatuses WHERE StatusCode = 'Cancelled');

    DECLARE @ScheduleWeekly INT = (SELECT PublicationScheduleId FROM dbo.PublicationSchedules WHERE ScheduleCode = 'Weekly');
    DECLARE @ScheduleMonthly INT = (SELECT PublicationScheduleId FROM dbo.PublicationSchedules WHERE ScheduleCode = 'Monthly');
    DECLARE @ScheduleSpecial INT = (SELECT PublicationScheduleId FROM dbo.PublicationSchedules WHERE ScheduleCode = 'Special');
    DECLARE @SchedulePaused INT = (SELECT PublicationScheduleId FROM dbo.PublicationSchedules WHERE ScheduleCode = 'Paused');

    DECLARE @ChapterDraft INT = (SELECT ChapterStatusId FROM dbo.ChapterStatuses WHERE StatusCode = 'Draft');
    DECLARE @ChapterInProduction INT = (SELECT ChapterStatusId FROM dbo.ChapterStatuses WHERE StatusCode = 'InProduction');
    DECLARE @ChapterPendingMangakaReview INT = (SELECT ChapterStatusId FROM dbo.ChapterStatuses WHERE StatusCode = 'PendingMangakaReview');
    DECLARE @ChapterReadyForEditorReview INT = (SELECT ChapterStatusId FROM dbo.ChapterStatuses WHERE StatusCode = 'ReadyForEditorReview');
    DECLARE @ChapterEditorRevisionRequired INT = (SELECT ChapterStatusId FROM dbo.ChapterStatuses WHERE StatusCode = 'EditorRevisionRequired');
    DECLARE @ChapterReadyForBoardDecision INT = (SELECT ChapterStatusId FROM dbo.ChapterStatuses WHERE StatusCode = 'ReadyForBoardDecision');
    DECLARE @ChapterReadyForPublication INT = (SELECT ChapterStatusId FROM dbo.ChapterStatuses WHERE StatusCode = 'ReadyForPublication');
    DECLARE @ChapterPublished INT = (SELECT ChapterStatusId FROM dbo.ChapterStatuses WHERE StatusCode = 'Published');
    DECLARE @ChapterCancelled INT = (SELECT ChapterStatusId FROM dbo.ChapterStatuses WHERE StatusCode = 'Cancelled');

    DECLARE @ManuscriptUploaded INT = (SELECT ManuscriptStatusId FROM dbo.ManuscriptStatuses WHERE StatusCode = 'Uploaded');
    DECLARE @ManuscriptSubmitted INT = (SELECT ManuscriptStatusId FROM dbo.ManuscriptStatuses WHERE StatusCode = 'Submitted');
    DECLARE @ManuscriptUnderReview INT = (SELECT ManuscriptStatusId FROM dbo.ManuscriptStatuses WHERE StatusCode = 'UnderReview');
    DECLARE @ManuscriptRevisionRequired INT = (SELECT ManuscriptStatusId FROM dbo.ManuscriptStatuses WHERE StatusCode = 'RevisionRequired');
    DECLARE @ManuscriptApproved INT = (SELECT ManuscriptStatusId FROM dbo.ManuscriptStatuses WHERE StatusCode = 'Approved');
    DECLARE @ManuscriptRejected INT = (SELECT ManuscriptStatusId FROM dbo.ManuscriptStatuses WHERE StatusCode = 'Rejected');

    DECLARE @PageUploaded INT = (SELECT PageStatusId FROM dbo.PageStatuses WHERE StatusCode = 'Uploaded');
    DECLARE @PageInProduction INT = (SELECT PageStatusId FROM dbo.PageStatuses WHERE StatusCode = 'InProduction');
    DECLARE @PagePendingReview INT = (SELECT PageStatusId FROM dbo.PageStatuses WHERE StatusCode = 'PendingReview');
    DECLARE @PageRevisionRequired INT = (SELECT PageStatusId FROM dbo.PageStatuses WHERE StatusCode = 'RevisionRequired');
    DECLARE @PageApproved INT = (SELECT PageStatusId FROM dbo.PageStatuses WHERE StatusCode = 'Approved');

    DECLARE @RegionPanel INT = (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'Panel');
    DECLARE @RegionSpeechBubble INT = (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'SpeechBubble');
    DECLARE @RegionCharacter INT = (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'Character');
    DECLARE @RegionBackground INT = (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'Background');
    DECLARE @RegionShading INT = (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'Shading');
    DECLARE @RegionEffect INT = (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'Effect');
    DECLARE @RegionOther INT = (SELECT RegionTypeId FROM dbo.RegionTypes WHERE TypeCode = 'Other');

    DECLARE @TaskBackgroundDrawing INT = (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'BackgroundDrawing');
    DECLARE @TaskShading INT = (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'Shading');
    DECLARE @TaskEffect INT = (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'Effect');
    DECLARE @TaskCleanLine INT = (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'CleanLine');
    DECLARE @TaskTone INT = (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'Tone');
    DECLARE @TaskSpeechBubbleCleanup INT = (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'SpeechBubbleCleanup');
    DECLARE @TaskOther INT = (SELECT TaskTypeId FROM dbo.TaskTypes WHERE TypeCode = 'Other');

    DECLARE @TaskAssigned INT = (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'Assigned');
    DECLARE @TaskInProgress INT = (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'InProgress');
    DECLARE @TaskSubmitted INT = (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'Submitted');
    DECLARE @TaskRevisionRequired INT = (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'RevisionRequired');
    DECLARE @TaskApproved INT = (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'Approved');
    DECLARE @TaskRejected INT = (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'Rejected');
    DECLARE @TaskOverdue INT = (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'Overdue');
    DECLARE @TaskCancelled INT = (SELECT TaskStatusId FROM dbo.TaskStatuses WHERE StatusCode = 'Cancelled');

    DECLARE @SubSubmitted INT = (SELECT SubmissionStatusId FROM dbo.SubmissionStatuses WHERE StatusCode = 'Submitted');
    DECLARE @SubApproved INT = (SELECT SubmissionStatusId FROM dbo.SubmissionStatuses WHERE StatusCode = 'Approved');
    DECLARE @SubRejected INT = (SELECT SubmissionStatusId FROM dbo.SubmissionStatuses WHERE StatusCode = 'Rejected');
    DECLARE @SubRevisionRequired INT = (SELECT SubmissionStatusId FROM dbo.SubmissionStatuses WHERE StatusCode = 'RevisionRequired');

    DECLARE @CommentOpen INT = (SELECT CommentStatusId FROM dbo.CommentStatuses WHERE StatusCode = 'Open');
    DECLARE @CommentResolved INT = (SELECT CommentStatusId FROM dbo.CommentStatuses WHERE StatusCode = 'Resolved');
    DECLARE @CommentCancelled INT = (SELECT CommentStatusId FROM dbo.CommentStatuses WHERE StatusCode = 'Cancelled');

    DECLARE @VoteApprove INT = (SELECT VoteValueId FROM dbo.VoteValues WHERE VoteCode = 'Approve');
    DECLARE @VoteReject INT = (SELECT VoteValueId FROM dbo.VoteValues WHERE VoteCode = 'Reject');
    DECLARE @VoteNeedRevision INT = (SELECT VoteValueId FROM dbo.VoteValues WHERE VoteCode = 'NeedRevision');
    DECLARE @VoteAbstain INT = (SELECT VoteValueId FROM dbo.VoteValues WHERE VoteCode = 'Abstain');

    DECLARE @DecisionContinue INT = (SELECT DecisionTypeId FROM dbo.DecisionTypes WHERE DecisionCode = 'Continue');
    DECLARE @DecisionCancel INT = (SELECT DecisionTypeId FROM dbo.DecisionTypes WHERE DecisionCode = 'Cancel');
    DECLARE @DecisionChangeSchedule INT = (SELECT DecisionTypeId FROM dbo.DecisionTypes WHERE DecisionCode = 'ChangeSchedule');
    DECLARE @DecisionPause INT = (SELECT DecisionTypeId FROM dbo.DecisionTypes WHERE DecisionCode = 'Pause');
    DECLARE @DecisionPromote INT = (SELECT DecisionTypeId FROM dbo.DecisionTypes WHERE DecisionCode = 'Promote');

    DECLARE @NotifTaskAssigned INT = (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'TaskAssigned');
    DECLARE @NotifSubmissionUploaded INT = (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'SubmissionUploaded');
    DECLARE @NotifSubmissionReviewed INT = (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'SubmissionReviewed');
    DECLARE @NotifEditorCommentCreated INT = (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'EditorCommentCreated');
    DECLARE @NotifBoardVoteSubmitted INT = (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'BoardVoteSubmitted');
    DECLARE @NotifRankingUpdated INT = (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'RankingUpdated');
    DECLARE @NotifDeadlineWarning INT = (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'DeadlineWarning');
    DECLARE @NotifSystem INT = (SELECT NotificationTypeId FROM dbo.NotificationTypes WHERE TypeCode = 'System');

    DECLARE @EarningPending INT = (SELECT EarningStatusId FROM dbo.EarningStatuses WHERE StatusCode = 'Pending');
    DECLARE @EarningApproved INT = (SELECT EarningStatusId FROM dbo.EarningStatuses WHERE StatusCode = 'Approved');
    DECLARE @EarningPaid INT = (SELECT EarningStatusId FROM dbo.EarningStatuses WHERE StatusCode = 'Paid');
    DECLARE @EarningCancelled INT = (SELECT EarningStatusId FROM dbo.EarningStatuses WHERE StatusCode = 'Cancelled');

    /* ============================================================
       2. EXTRA USERS FOR CODING AND ROLE FILTER DEMO
       ============================================================ */

    DECLARE @AdminId UNIQUEIDENTIFIER = (SELECT UserId FROM dbo.Users WHERE Email = N'admin@manga.local');
    DECLARE @Mangaka1Id UNIQUEIDENTIFIER = (SELECT UserId FROM dbo.Users WHERE Email = N'mangaka@manga.local');
    DECLARE @Assistant1Id UNIQUEIDENTIFIER = (SELECT UserId FROM dbo.Users WHERE Email = N'assistant@manga.local');
    DECLARE @Editor1Id UNIQUEIDENTIFIER = (SELECT UserId FROM dbo.Users WHERE Email = N'editor@manga.local');
    DECLARE @Board1Id UNIQUEIDENTIFIER = (SELECT UserId FROM dbo.Users WHERE Email = N'board@manga.local');

    DECLARE @Mangaka2Id UNIQUEIDENTIFIER = NEWID();
    DECLARE @Assistant2Id UNIQUEIDENTIFIER = NEWID();
    DECLARE @Editor2Id UNIQUEIDENTIFIER = NEWID();
    DECLARE @Board2Id UNIQUEIDENTIFIER = NEWID();
    DECLARE @Board3Id UNIQUEIDENTIFIER = NEWID();
    DECLARE @Assistant3Id UNIQUEIDENTIFIER = NEWID();

    INSERT INTO dbo.Users (UserId, FullName, Email, PasswordHash, AvatarUrl, CreatedAt)
    VALUES
    (@Mangaka2Id, N'Riku Mori', N'mangaka2@manga.local', N'$2a$12$Rwo5eRb9x0E/6Q9MUw1OcuwhQwT4lNIprqDIZfSU6l/B3jEg7Ve2m', N'/demo/avatars/riku-mori.png', DATEADD(DAY, -40, @Now)),
    (@Assistant2Id, N'Yuna Ito', N'assistant2@manga.local', N'$2a$12$Rwo5eRb9x0E/6Q9MUw1OcuwhQwT4lNIprqDIZfSU6l/B3jEg7Ve2m', N'/demo/avatars/yuna-ito.png', DATEADD(DAY, -36, @Now)),
    (@Assistant3Id, N'Daichi Noda', N'assistant3@manga.local', N'$2a$12$Rwo5eRb9x0E/6Q9MUw1OcuwhQwT4lNIprqDIZfSU6l/B3jEg7Ve2m', N'/demo/avatars/daichi-noda.png', DATEADD(DAY, -18, @Now)),
    (@Editor2Id, N'Haruka Editor', N'editor2@manga.local', N'$2a$12$Rwo5eRb9x0E/6Q9MUw1OcuwhQwT4lNIprqDIZfSU6l/B3jEg7Ve2m', N'/demo/avatars/haruka-editor.png', DATEADD(DAY, -35, @Now)),
    (@Board2Id, N'Board Member 02', N'board2@manga.local', N'$2a$12$Rwo5eRb9x0E/6Q9MUw1OcuwhQwT4lNIprqDIZfSU6l/B3jEg7Ve2m', N'/demo/avatars/board-02.png', DATEADD(DAY, -34, @Now)),
    (@Board3Id, N'Board Member 03', N'board3@manga.local', N'$2a$12$Rwo5eRb9x0E/6Q9MUw1OcuwhQwT4lNIprqDIZfSU6l/B3jEg7Ve2m', N'/demo/avatars/board-03.png', DATEADD(DAY, -33, @Now));

    INSERT INTO dbo.UserRoles (UserId, RoleId)
    VALUES
    (@Mangaka2Id, @RoleMangaka),
    (@Assistant2Id, @RoleAssistant),
    (@Assistant3Id, @RoleAssistant),
    (@Editor2Id, @RoleEditor),
    (@Board2Id, @RoleBoard),
    (@Board3Id, @RoleBoard);

    /* ============================================================
       3. PUBLICATION ISSUES
       ============================================================ */

    DECLARE @IssueW27 UNIQUEIDENTIFIER = NEWID();
    DECLARE @IssueW28 UNIQUEIDENTIFIER = NEWID();
    DECLARE @IssueW29 UNIQUEIDENTIFIER = NEWID();
    DECLARE @IssueM07 UNIQUEIDENTIFIER = NEWID();

    INSERT INTO dbo.PublicationIssues (PublicationIssueId, IssueNumber, IssueTitle, PublishedDate, CreatedAt)
    VALUES
    (@IssueW27, N'2026-W27', N'Weekly Manga Issue 27/2026', DATEADD(DAY, 7, CAST(GETDATE() AS DATE)), DATEADD(DAY, -2, @Now)),
    (@IssueW28, N'2026-W28', N'Weekly Manga Issue 28/2026', DATEADD(DAY, 14, CAST(GETDATE() AS DATE)), DATEADD(DAY, -1, @Now)),
    (@IssueW29, N'2026-W29', N'Weekly Manga Issue 29/2026', DATEADD(DAY, 21, CAST(GETDATE() AS DATE)), @Now),
    (@IssueM07, N'2026-M07', N'Monthly Special Issue 07/2026', DATEADD(DAY, 28, CAST(GETDATE() AS DATE)), @Now);

    /* ============================================================
       4. SERIES IN EVERY IMPORTANT STATUS
       ============================================================ */

    DECLARE @SNeon UNIQUEIDENTIFIER = NEWID();
    DECLARE @SCrimson UNIQUEIDENTIFIER = NEWID();
    DECLARE @SMoon UNIQUEIDENTIFIER = NEWID();
    DECLARE @SHollow UNIQUEIDENTIFIER = NEWID();
    DECLARE @SStarfall UNIQUEIDENTIFIER = NEWID();
    DECLARE @SPaper UNIQUEIDENTIFIER = NEWID();

    INSERT INTO dbo.Series (
        SeriesId, Title, AlternativeTitle, Description, Genre, CoverImageUrl,
        MangakaId, TantouEditorId, SeriesStatusId, PublicationScheduleId,
        CreatedAt, SubmittedAt, ApprovedAt, RejectedAt, CancelledAt, CancellationRiskScore
    )
    VALUES
    (@SNeon, N'Neon Samurai', N'Kiếm Sĩ Neon',
     N'Series proposal waiting for editorial board decision. Useful for Board voting screens.',
     N'Cyberpunk / Action', N'/demo/series/neon-samurai-cover.png',
     @Mangaka2Id, @Editor2Id, @SeriesSubmitted, @ScheduleWeekly,
     DATEADD(DAY, -12, @Now), DATEADD(DAY, -10, @Now), NULL, NULL, NULL, 12.00),

    (@SCrimson, N'Crimson Café', N'Quán Cà Phê Đỏ',
     N'Draft series owned by Mangaka. Useful for Create/Edit/Delete Series CRUD screens.',
     N'Slice of Life / Mystery', N'/demo/series/crimson-cafe-cover.png',
     @Mangaka1Id, @Editor1Id, @SeriesDraft, @ScheduleMonthly,
     DATEADD(DAY, -5, @Now), NULL, NULL, NULL, NULL, 0.00),

    (@SMoon, N'Moon Rabbit Express', N'Chuyến Tàu Thỏ Trăng',
     N'Approved series ready to prepare new chapters. Useful for approved series filters.',
     N'Fantasy / Adventure', N'/demo/series/moon-rabbit-cover.png',
     @Mangaka2Id, @Editor2Id, @SeriesApproved, @ScheduleMonthly,
     DATEADD(DAY, -45, @Now), DATEADD(DAY, -42, @Now), DATEADD(DAY, -38, @Now), NULL, NULL, 8.00),

    (@SHollow, N'Hollow Brush', N'Cọ Rỗng',
     N'Series requiring revision after editor and board feedback. Useful for revision workflow.',
     N'Horror / Supernatural', N'/demo/series/hollow-brush-cover.png',
     @Mangaka1Id, @Editor1Id, @SeriesRevisionRequired, @ScheduleSpecial,
     DATEADD(DAY, -24, @Now), DATEADD(DAY, -20, @Now), NULL, NULL, NULL, 35.00),

    (@SStarfall, N'Starfall Idol Manga', N'Thần Tượng Sao Rơi',
     N'Publishing series with positive ranking trend. Useful for ranking and issue dashboard.',
     N'Music / Drama', N'/demo/series/starfall-idol-cover.png',
     @Mangaka2Id, @Editor2Id, @SeriesPublishing, @ScheduleWeekly,
     DATEADD(DAY, -90, @Now), DATEADD(DAY, -85, @Now), DATEADD(DAY, -80, @Now), NULL, NULL, 18.50),

    (@SPaper, N'Paper Dragon Legacy', N'Di Sản Rồng Giấy',
     N'Cancelled series with poor ranking trend. Useful for cancellation decision demo.',
     N'Historical / Fantasy', N'/demo/series/paper-dragon-cover.png',
     @Mangaka1Id, @Editor1Id, @SeriesCancelled, @SchedulePaused,
     DATEADD(DAY, -120, @Now), DATEADD(DAY, -115, @Now), DATEADD(DAY, -110, @Now), NULL, DATEADD(DAY, -3, @Now), 96.00);

    INSERT INTO dbo.SeriesTeamMembers (SeriesId, UserId, RoleInSeries, JoinedAt)
    VALUES
    (@SNeon, @Mangaka2Id, N'Main Mangaka', DATEADD(DAY, -12, @Now)),
    (@SNeon, @Assistant2Id, N'Clean Line Assistant', DATEADD(DAY, -11, @Now)),
    (@SNeon, @Editor2Id, N'Tantou Editor', DATEADD(DAY, -11, @Now)),

    (@SCrimson, @Mangaka1Id, N'Main Mangaka', DATEADD(DAY, -5, @Now)),
    (@SCrimson, @Assistant1Id, N'Tone Assistant', DATEADD(DAY, -4, @Now)),
    (@SCrimson, @Editor1Id, N'Tantou Editor', DATEADD(DAY, -4, @Now)),

    (@SMoon, @Mangaka2Id, N'Main Mangaka', DATEADD(DAY, -45, @Now)),
    (@SMoon, @Assistant2Id, N'Background Assistant', DATEADD(DAY, -44, @Now)),
    (@SMoon, @Assistant3Id, N'Effect Assistant', DATEADD(DAY, -30, @Now)),
    (@SMoon, @Editor2Id, N'Tantou Editor', DATEADD(DAY, -44, @Now)),

    (@SHollow, @Mangaka1Id, N'Main Mangaka', DATEADD(DAY, -24, @Now)),
    (@SHollow, @Assistant1Id, N'Shading Assistant', DATEADD(DAY, -23, @Now)),
    (@SHollow, @Editor1Id, N'Tantou Editor', DATEADD(DAY, -23, @Now)),

    (@SStarfall, @Mangaka2Id, N'Main Mangaka', DATEADD(DAY, -90, @Now)),
    (@SStarfall, @Assistant2Id, N'Background Assistant', DATEADD(DAY, -89, @Now)),
    (@SStarfall, @Assistant3Id, N'Effect Assistant', DATEADD(DAY, -60, @Now)),
    (@SStarfall, @Editor2Id, N'Tantou Editor', DATEADD(DAY, -89, @Now)),

    (@SPaper, @Mangaka1Id, N'Main Mangaka', DATEADD(DAY, -120, @Now)),
    (@SPaper, @Assistant1Id, N'Legacy Assistant', DATEADD(DAY, -119, @Now)),
    (@SPaper, @Editor1Id, N'Tantou Editor', DATEADD(DAY, -119, @Now));

    /* ============================================================
       5. CHAPTERS
       ============================================================ */

    DECLARE @ChNeon1 UNIQUEIDENTIFIER = NEWID();
    DECLARE @ChCrimson1 UNIQUEIDENTIFIER = NEWID();
    DECLARE @ChMoon1 UNIQUEIDENTIFIER = NEWID();
    DECLARE @ChMoon2 UNIQUEIDENTIFIER = NEWID();
    DECLARE @ChHollow1 UNIQUEIDENTIFIER = NEWID();
    DECLARE @ChStar1 UNIQUEIDENTIFIER = NEWID();
    DECLARE @ChStar2 UNIQUEIDENTIFIER = NEWID();
    DECLARE @ChPaper1 UNIQUEIDENTIFIER = NEWID();

    INSERT INTO dbo.Chapters (ChapterId, SeriesId, ChapterNumber, Title, Summary, ChapterStatusId, Deadline, CreatedAt, CompletedAt)
    VALUES
    (@ChNeon1, @SNeon, 1, N'Pilot: Rain Over Neo-Tokyo', N'Pilot chapter for board review.', @ChapterReadyForBoardDecision, DATEADD(DAY, 2, @Now), DATEADD(DAY, -9, @Now), NULL),
    (@ChCrimson1, @SCrimson, 1, N'Draft: The Red Cup', N'Draft chapter, useful for Mangaka CRUD.', @ChapterDraft, DATEADD(DAY, 14, @Now), DATEADD(DAY, -4, @Now), NULL),
    (@ChMoon1, @SMoon, 1, N'Ticket to the Moon', N'Approved chapter ready for publication.', @ChapterReadyForPublication, DATEADD(DAY, 3, @Now), DATEADD(DAY, -20, @Now), DATEADD(DAY, -2, @Now)),
    (@ChMoon2, @SMoon, 2, N'The Broken Station', N'Chapter in editor review.', @ChapterReadyForEditorReview, DATEADD(DAY, 9, @Now), DATEADD(DAY, -2, @Now), NULL),
    (@ChHollow1, @SHollow, 1, N'Brush Without Shadow', N'Chapter requiring editor revision.', @ChapterEditorRevisionRequired, DATEADD(DAY, 4, @Now), DATEADD(DAY, -18, @Now), NULL),
    (@ChStar1, @SStarfall, 1, N'First Stage', N'Published chapter.', @ChapterPublished, DATEADD(DAY, -10, @Now), DATEADD(DAY, -35, @Now), DATEADD(DAY, -12, @Now)),
    (@ChStar2, @SStarfall, 2, N'Encore Under Meteor Lights', N'In production chapter with live tasks.', @ChapterInProduction, DATEADD(DAY, 5, @Now), DATEADD(DAY, -5, @Now), NULL),
    (@ChPaper1, @SPaper, 1, N'The Last Fold', N'Cancelled legacy chapter.', @ChapterCancelled, DATEADD(DAY, -20, @Now), DATEADD(DAY, -60, @Now), DATEADD(DAY, -21, @Now));

    INSERT INTO dbo.ChapterPublications (ChapterId, PublicationIssueId, PublicationOrder)
    VALUES
    (@ChMoon1, @IssueW27, 2),
    (@ChStar1, @IssueW27, 1),
    (@ChPaper1, @IssueW27, 8);

    /* ============================================================
       6. MANUSCRIPTS
       ============================================================ */

    INSERT INTO dbo.Manuscripts (SeriesId, ChapterId, Title, Description, FileUrl, FileName, ContentType, FileSizeBytes, VersionNo, ManuscriptStatusId, UploadedByUserId, UploadedAt)
    VALUES
    (@SNeon, @ChNeon1, N'Neon Samurai pilot storyboard', N'Pilot manuscript awaiting board review.', N'/demo/manuscripts/neon-samurai-pilot.pdf', N'neon-samurai-pilot.pdf', N'application/pdf', 1550000, 1, @ManuscriptSubmitted, @Mangaka2Id, DATEADD(DAY, -9, @Now)),
    (@SCrimson, @ChCrimson1, N'Crimson Café rough plot', N'Draft plot document.', N'/demo/manuscripts/crimson-cafe-rough-plot.pdf', N'crimson-cafe-rough-plot.pdf', N'application/pdf', 800000, 1, @ManuscriptUploaded, @Mangaka1Id, DATEADD(DAY, -4, @Now)),
    (@SMoon, @ChMoon1, N'Moon Rabbit Express final script', N'Approved final manuscript.', N'/demo/manuscripts/moon-rabbit-final.pdf', N'moon-rabbit-final.pdf', N'application/pdf', 2100000, 2, @ManuscriptApproved, @Mangaka2Id, DATEADD(DAY, -6, @Now)),
    (@SHollow, @ChHollow1, N'Hollow Brush revised script', N'Needs revision after editor review.', N'/demo/manuscripts/hollow-brush-revision.pdf', N'hollow-brush-revision.pdf', N'application/pdf', 1800000, 2, @ManuscriptRevisionRequired, @Mangaka1Id, DATEADD(DAY, -3, @Now)),
    (@SStarfall, @ChStar2, N'Starfall Idol chapter 2 script', N'Under editor review.', N'/demo/manuscripts/starfall-c2-script.pdf', N'starfall-c2-script.pdf', N'application/pdf', 1900000, 1, @ManuscriptUnderReview, @Mangaka2Id, DATEADD(DAY, -2, @Now)),
    (@SPaper, @ChPaper1, N'Paper Dragon rejected addendum', N'Rejected addendum after cancellation.', N'/demo/manuscripts/paper-dragon-addendum.pdf', N'paper-dragon-addendum.pdf', N'application/pdf', 900000, 1, @ManuscriptRejected, @Mangaka1Id, DATEADD(DAY, -15, @Now));

    /* ============================================================
       7. PAGES
       ============================================================ */

    DECLARE @PNeon1 UNIQUEIDENTIFIER = NEWID();
    DECLARE @PNeon2 UNIQUEIDENTIFIER = NEWID();
    DECLARE @PCrimson1 UNIQUEIDENTIFIER = NEWID();
    DECLARE @PMoon1 UNIQUEIDENTIFIER = NEWID();
    DECLARE @PMoon2 UNIQUEIDENTIFIER = NEWID();
    DECLARE @PHollow1 UNIQUEIDENTIFIER = NEWID();
    DECLARE @PStar1 UNIQUEIDENTIFIER = NEWID();
    DECLARE @PStar2 UNIQUEIDENTIFIER = NEWID();
    DECLARE @PPaper1 UNIQUEIDENTIFIER = NEWID();

    INSERT INTO dbo.MangaPages (PageId, ChapterId, PageNumber, ImageUrl, ThumbnailUrl, FileName, ContentType, FileSizeBytes, VersionNo, PageStatusId, UploadedByUserId, UploadedAt)
    VALUES
    (@PNeon1, @ChNeon1, 1, N'/demo/pages/neon-c1-p1.png', N'/demo/pages/thumb-neon-c1-p1.png', N'neon-c1-p1.png', N'image/png', 910000, 1, @PagePendingReview, @Mangaka2Id, DATEADD(DAY, -8, @Now)),
    (@PNeon2, @ChNeon1, 2, N'/demo/pages/neon-c1-p2.png', N'/demo/pages/thumb-neon-c1-p2.png', N'neon-c1-p2.png', N'image/png', 940000, 1, @PagePendingReview, @Mangaka2Id, DATEADD(DAY, -8, @Now)),
    (@PCrimson1, @ChCrimson1, 1, N'/demo/pages/crimson-c1-p1.png', N'/demo/pages/thumb-crimson-c1-p1.png', N'crimson-c1-p1.png', N'image/png', 670000, 1, @PageUploaded, @Mangaka1Id, DATEADD(DAY, -3, @Now)),
    (@PMoon1, @ChMoon1, 1, N'/demo/pages/moon-c1-p1.png', N'/demo/pages/thumb-moon-c1-p1.png', N'moon-c1-p1.png', N'image/png', 890000, 1, @PageApproved, @Mangaka2Id, DATEADD(DAY, -7, @Now)),
    (@PMoon2, @ChMoon2, 1, N'/demo/pages/moon-c2-p1.png', N'/demo/pages/thumb-moon-c2-p1.png', N'moon-c2-p1.png', N'image/png', 840000, 1, @PagePendingReview, @Mangaka2Id, DATEADD(DAY, -1, @Now)),
    (@PHollow1, @ChHollow1, 1, N'/demo/pages/hollow-c1-p1.png', N'/demo/pages/thumb-hollow-c1-p1.png', N'hollow-c1-p1.png', N'image/png', 970000, 1, @PageRevisionRequired, @Mangaka1Id, DATEADD(DAY, -3, @Now)),
    (@PStar1, @ChStar1, 1, N'/demo/pages/starfall-c1-p1.png', N'/demo/pages/thumb-starfall-c1-p1.png', N'starfall-c1-p1.png', N'image/png', 920000, 1, @PageApproved, @Mangaka2Id, DATEADD(DAY, -20, @Now)),
    (@PStar2, @ChStar2, 1, N'/demo/pages/starfall-c2-p1.png', N'/demo/pages/thumb-starfall-c2-p1.png', N'starfall-c2-p1.png', N'image/png', 950000, 1, @PageInProduction, @Mangaka2Id, DATEADD(DAY, -4, @Now)),
    (@PPaper1, @ChPaper1, 1, N'/demo/pages/paper-c1-p1.png', N'/demo/pages/thumb-paper-c1-p1.png', N'paper-c1-p1.png', N'image/png', 790000, 1, @PageApproved, @Mangaka1Id, DATEADD(DAY, -50, @Now));

    /* ============================================================
       8. REGIONS
       ============================================================ */

    DECLARE @RNeonPanel UNIQUEIDENTIFIER = NEWID();
    DECLARE @RNeonChar UNIQUEIDENTIFIER = NEWID();
    DECLARE @RCrimsonBubble UNIQUEIDENTIFIER = NEWID();
    DECLARE @RMoonBackground UNIQUEIDENTIFIER = NEWID();
    DECLARE @RMoonEffect UNIQUEIDENTIFIER = NEWID();
    DECLARE @RHollowShading UNIQUEIDENTIFIER = NEWID();
    DECLARE @RStarPanel UNIQUEIDENTIFIER = NEWID();
    DECLARE @RStarEffect UNIQUEIDENTIFIER = NEWID();
    DECLARE @RPaperOther UNIQUEIDENTIFIER = NEWID();

    INSERT INTO dbo.PageRegions (RegionId, PageId, RegionTypeId, X, Y, Width, Height, Label, Confidence, SourceType, CreatedByUserId, CreatedAt)
    VALUES
    (@RNeonPanel, @PNeon1, @RegionPanel, 20, 30, 700, 420, N'Opening city panel', 0.9500, 'AI', @Mangaka2Id, DATEADD(DAY, -8, @Now)),
    (@RNeonChar, @PNeon1, @RegionCharacter, 260, 190, 220, 480, N'Samurai silhouette', 0.8800, 'AI', @Mangaka2Id, DATEADD(DAY, -8, @Now)),
    (@RCrimsonBubble, @PCrimson1, @RegionSpeechBubble, 430, 80, 200, 110, N'Waitress intro dialogue', NULL, 'Manual', @Mangaka1Id, DATEADD(DAY, -3, @Now)),
    (@RMoonBackground, @PMoon1, @RegionBackground, 0, 0, 760, 320, N'Moon station background', NULL, 'Manual', @Mangaka2Id, DATEADD(DAY, -7, @Now)),
    (@RMoonEffect, @PMoon2, @RegionEffect, 110, 360, 420, 160, N'Train departure light burst', 0.9100, 'AI', @Mangaka2Id, DATEADD(DAY, -1, @Now)),
    (@RHollowShading, @PHollow1, @RegionShading, 70, 120, 500, 380, N'Dark room shading correction', NULL, 'Manual', @Mangaka1Id, DATEADD(DAY, -3, @Now)),
    (@RStarPanel, @PStar1, @RegionPanel, 10, 20, 740, 500, N'Opening stage panel', NULL, 'Manual', @Mangaka2Id, DATEADD(DAY, -20, @Now)),
    (@RStarEffect, @PStar2, @RegionEffect, 120, 240, 520, 230, N'Meteor spotlight effect', 0.9200, 'AI', @Mangaka2Id, DATEADD(DAY, -4, @Now)),
    (@RPaperOther, @PPaper1, @RegionOther, 50, 50, 680, 620, N'Legacy full-page correction', NULL, 'Manual', @Mangaka1Id, DATEADD(DAY, -50, @Now));

    /* ============================================================
       9. AI SEGMENTATION JOBS: EVERY STATUS
       ============================================================ */

    INSERT INTO dbo.AiSegmentationJobs (PageId, RequestedByUserId, ModelName, Status, RawResultJson, ErrorMessage, StartedAt, CompletedAt, CreatedAt)
    VALUES
    (@PNeon1, @Mangaka2Id, N'Mock-YOLOv8-Manga-v2', 'Succeeded',
     N'[{"regionType":"Panel","x":20,"y":30,"width":700,"height":420,"confidence":0.95},{"regionType":"Character","x":260,"y":190,"width":220,"height":480,"confidence":0.88}]',
     NULL, DATEADD(DAY, -8, @Now), DATEADD(DAY, -8, DATEADD(MINUTE, 2, @Now)), DATEADD(DAY, -8, @Now)),
    (@PMoon2, @Mangaka2Id, N'Mock-SAM-Manga-v1', 'Queued', NULL, NULL, NULL, NULL, DATEADD(MINUTE, -12, @Now)),
    (@PStar2, @Mangaka2Id, N'Mock-U-Net-Manga-v1', 'Running', NULL, NULL, DATEADD(MINUTE, -3, @Now), NULL, DATEADD(MINUTE, -4, @Now)),
    (@PHollow1, @Mangaka1Id, N'Mock-SAM-Manga-v1', 'Failed', NULL,
     N'Low contrast page. Manual correction required.', DATEADD(HOUR, -3, @Now), DATEADD(HOUR, -3, DATEADD(MINUTE, 1, @Now)), DATEADD(HOUR, -3, @Now));

    /* ============================================================
       10. TASKS: EVERY STATUS
       ============================================================ */

    DECLARE @TAssigned UNIQUEIDENTIFIER = NEWID();
    DECLARE @TInProgress UNIQUEIDENTIFIER = NEWID();
    DECLARE @TSubmitted UNIQUEIDENTIFIER = NEWID();
    DECLARE @TRevision UNIQUEIDENTIFIER = NEWID();
    DECLARE @TApproved UNIQUEIDENTIFIER = NEWID();
    DECLARE @TRejected UNIQUEIDENTIFIER = NEWID();
    DECLARE @TOverdue UNIQUEIDENTIFIER = NEWID();
    DECLARE @TCancelled UNIQUEIDENTIFIER = NEWID();

    INSERT INTO dbo.ProductionTasks (
        TaskId, PageId, RegionId, AssignedToAssistantId, CreatedByMangakaId, TaskTypeId, TaskStatusId,
        Title, Description, Priority, Deadline, Price, CreatedAt, StartedAt, CompletedAt, UpdatedAt
    )
    VALUES
    (@TAssigned, @PCrimson1, @RCrimsonBubble, @Assistant1Id, @Mangaka1Id, @TaskSpeechBubbleCleanup, @TaskAssigned,
     N'Prepare clean dialogue bubble for Crimson Café', N'Clean the speech bubble and leave safe space for lettering.', 3, DATEADD(DAY, 6, @Now), 45000, DATEADD(HOUR, -4, @Now), NULL, NULL, DATEADD(HOUR, -4, @Now)),

    (@TInProgress, @PStar2, @RStarEffect, @Assistant3Id, @Mangaka2Id, @TaskEffect, @TaskInProgress,
     N'Create meteor spotlight effect', N'Add strong stage lighting and meteor visual effect.', 2, DATEADD(DAY, 2, @Now), 100000, DATEADD(DAY, -2, @Now), DATEADD(DAY, -1, @Now), NULL, DATEADD(DAY, -1, @Now)),

    (@TSubmitted, @PNeon1, @RNeonChar, @Assistant2Id, @Mangaka2Id, @TaskCleanLine, @TaskSubmitted,
     N'Clean samurai silhouette line art', N'Clean the silhouette lines for the main character reveal.', 1, DATEADD(DAY, 1, @Now), 85000, DATEADD(DAY, -6, @Now), DATEADD(DAY, -5, @Now), NULL, DATEADD(HOUR, -6, @Now)),

    (@TRevision, @PHollow1, @RHollowShading, @Assistant1Id, @Mangaka1Id, @TaskShading, @TaskRevisionRequired,
     N'Rework dark room shading', N'Previous shading made the panel too dark. Need revision.', 1, DATEADD(DAY, 1, @Now), 95000, DATEADD(DAY, -7, @Now), DATEADD(DAY, -6, @Now), NULL, DATEADD(HOUR, -2, @Now)),

    (@TApproved, @PMoon1, @RMoonBackground, @Assistant2Id, @Mangaka2Id, @TaskBackgroundDrawing, @TaskApproved,
     N'Finalize moon station background', N'Final approved background for Moon Rabbit Express.', 2, DATEADD(DAY, -3, @Now), 130000, DATEADD(DAY, -10, @Now), DATEADD(DAY, -9, @Now), DATEADD(DAY, -4, @Now), DATEADD(DAY, -4, @Now)),

    (@TRejected, @PMoon2, @RMoonEffect, @Assistant3Id, @Mangaka2Id, @TaskEffect, @TaskRejected,
     N'Old train light burst attempt', N'Rejected attempt because the light burst hides the character face.', 2, DATEADD(DAY, -1, @Now), 75000, DATEADD(DAY, -5, @Now), DATEADD(DAY, -4, @Now), DATEADD(DAY, -2, @Now), DATEADD(DAY, -2, @Now)),

    (@TOverdue, @PNeon2, @RNeonPanel, @Assistant2Id, @Mangaka2Id, @TaskTone, @TaskOverdue,
     N'Apply cyberpunk screen tone', N'Overdue tone task for Neon Samurai page 2.', 1, DATEADD(DAY, -1, @Now), 70000, DATEADD(DAY, -4, @Now), DATEADD(DAY, -3, @Now), NULL, DATEADD(HOUR, -1, @Now)),

    (@TCancelled, @PPaper1, @RPaperOther, @Assistant1Id, @Mangaka1Id, @TaskOther, @TaskCancelled,
     N'Cancelled correction after series cancellation', N'Task cancelled because the series was cancelled by board decision.', 5, DATEADD(DAY, -10, @Now), 30000, DATEADD(DAY, -20, @Now), NULL, NULL, DATEADD(DAY, -3, @Now));

    /* ============================================================
       11. SUBMISSIONS: EVERY REVIEW STATUS
       ============================================================ */

    INSERT INTO dbo.TaskSubmissions (
        TaskId, SubmittedByAssistantId, FileUrl, FileName, ContentType, FileSizeBytes, Comment,
        SubmissionStatusId, SubmittedAt, ReviewedAt, ReviewedByMangakaId, ReviewNote
    )
    VALUES
    (@TSubmitted, @Assistant2Id, N'/demo/submissions/neon-silhouette-line-v1.png', N'neon-silhouette-line-v1.png', N'image/png', 710000,
     N'Clean line art completed. Please review the sword outline.', @SubSubmitted, DATEADD(HOUR, -6, @Now), NULL, NULL, NULL),

    (@TRevision, @Assistant1Id, N'/demo/submissions/hollow-shading-v1.png', N'hollow-shading-v1.png', N'image/png', 760000,
     N'First shading pass completed.', @SubRevisionRequired, DATEADD(DAY, -2, @Now), DATEADD(HOUR, -2, @Now), @Mangaka1Id,
     N'Revision required: the corner area is too dark and hides the brush symbol.'),

    (@TApproved, @Assistant2Id, N'/demo/submissions/moon-station-bg-final.png', N'moon-station-bg-final.png', N'image/png', 850000,
     N'Final background exported.', @SubApproved, DATEADD(DAY, -5, @Now), DATEADD(DAY, -4, @Now), @Mangaka2Id,
     N'Approved. Strong perspective and atmosphere.'),

    (@TRejected, @Assistant3Id, N'/demo/submissions/moon-light-burst-v1.png', N'moon-light-burst-v1.png', N'image/png', 650000,
     N'First light burst attempt.', @SubRejected, DATEADD(DAY, -3, @Now), DATEADD(DAY, -2, @Now), @Mangaka2Id,
     N'Rejected: effect covers character expression. Please wait for reassignment.');

    /* ============================================================
       12. EDITOR COMMENTS
       ============================================================ */

    INSERT INTO dbo.EditorComments (PageId, EditorId, X, Y, Width, Height, CommentText, CommentStatusId, CreatedAt, ResolvedAt, ResolvedByUserId)
    VALUES
    (@PNeon1, @Editor2Id, 260, 190, 220, 480, N'The silhouette is strong, but the sword outline needs clearer contrast.', @CommentOpen, DATEADD(HOUR, -5, @Now), NULL, NULL),
    (@PCrimson1, @Editor1Id, 430, 80, 200, 110, N'This line should sound more natural and less expositional.', @CommentOpen, DATEADD(HOUR, -4, @Now), NULL, NULL),
    (@PMoon1, @Editor2Id, 0, 0, 760, 320, N'Background approved. Good sense of depth.', @CommentResolved, DATEADD(DAY, -6, @Now), DATEADD(DAY, -5, @Now), @Mangaka2Id),
    (@PHollow1, @Editor1Id, 70, 120, 500, 380, N'Reduce the black fill by around 20%. The focal object is currently hard to read.', @CommentOpen, DATEADD(DAY, -1, @Now), NULL, NULL),
    (@PPaper1, @Editor1Id, NULL, NULL, NULL, NULL, N'Comment closed after board cancellation decision.', @CommentCancelled, DATEADD(DAY, -8, @Now), NULL, NULL);

    /* ============================================================
       13. BOARD VOTES AND DECISIONS
       ============================================================ */

    INSERT INTO dbo.BoardVotes (SeriesId, BoardMemberId, VoteValueId, Comment, CreatedAt)
    VALUES
    (@SNeon, @Board1Id, @VoteApprove, N'Cyberpunk hook is strong and marketable.', DATEADD(DAY, -1, @Now)),
    (@SNeon, @Board2Id, @VoteNeedRevision, N'Need clearer first chapter pacing before approval.', DATEADD(HOUR, -20, @Now)),
    (@SNeon, @Board3Id, @VoteApprove, N'Approve with editor supervision.', DATEADD(HOUR, -18, @Now)),

    (@SHollow, @Board1Id, @VoteNeedRevision, N'Mood is good but panel readability is weak.', DATEADD(DAY, -3, @Now)),
    (@SHollow, @Board2Id, @VoteNeedRevision, N'Requires revision before second review.', DATEADD(DAY, -3, @Now)),
    (@SHollow, @Board3Id, @VoteAbstain, N'Waiting for corrected sample pages.', DATEADD(DAY, -3, @Now)),

    (@SPaper, @Board1Id, @VoteReject, N'Reader response has been weak for multiple issues.', DATEADD(DAY, -4, @Now)),
    (@SPaper, @Board2Id, @VoteReject, N'Production cost is not justified by ranking.', DATEADD(DAY, -4, @Now)),
    (@SPaper, @Board3Id, @VoteReject, N'Recommend cancellation.', DATEADD(DAY, -4, @Now));

    INSERT INTO dbo.PublishingDecisions (SeriesId, DecisionTypeId, DecidedByUserId, Reason, CreatedAt)
    VALUES
    (@SNeon, @DecisionContinue, @Board1Id, N'Pilot remains in review; allow one revision cycle.', DATEADD(HOUR, -12, @Now)),
    (@SHollow, @DecisionPause, @Board2Id, N'Pause approval until page readability improves.', DATEADD(DAY, -2, @Now)),
    (@SStarfall, @DecisionPromote, @Board3Id, N'Promote because ranking improved and reader votes increased.', DATEADD(DAY, -1, @Now)),
    (@SPaper, @DecisionCancel, @Board1Id, N'Cancelled after repeated low ranking and weak reader voting.', DATEADD(DAY, -3, @Now));

    /* ============================================================
       14. READER VOTES AND RANKINGS
       ============================================================ */

    INSERT INTO dbo.ReaderVoteData (SeriesId, IssueNumber, VoteCount, RankPosition, ImportedByUserId, ImportedAt, Notes)
    VALUES
    (@SStarfall, N'2026-W27', 2100, 7, @Board2Id, DATEADD(DAY, -7, @Now), N'Good response after idol concert chapter.'),
    (@SStarfall, N'2026-W28', 2680, 3, @Board2Id, DATEADD(DAY, -1, @Now), N'Strong improvement after social media buzz.'),
    (@SMoon, N'2026-W27', 1950, 8, @Board3Id, DATEADD(DAY, -7, @Now), N'Stable fantasy readership.'),
    (@SMoon, N'2026-W28', 1985, 8, @Board3Id, DATEADD(DAY, -1, @Now), N'Stable ranking.'),
    (@SPaper, N'2026-W27', 620, 18, @Board1Id, DATEADD(DAY, -7, @Now), N'Low reader vote.'),
    (@SPaper, N'2026-W28', 410, 21, @Board1Id, DATEADD(DAY, -1, @Now), N'Dropped further; cancellation confirmed.');

    INSERT INTO dbo.RankingRecords (SeriesId, IssueNumber, VoteCount, RankPosition, PreviousRankPosition, Trend, CalculatedAt)
    VALUES
    (@SStarfall, N'2026-W27', 2100, 7, NULL, 'New', DATEADD(DAY, -7, @Now)),
    (@SStarfall, N'2026-W28', 2680, 3, 7, 'Up', DATEADD(DAY, -1, @Now)),
    (@SMoon, N'2026-W27', 1950, 8, NULL, 'New', DATEADD(DAY, -7, @Now)),
    (@SMoon, N'2026-W28', 1985, 8, 8, 'Stable', DATEADD(DAY, -1, @Now)),
    (@SPaper, N'2026-W27', 620, 18, NULL, 'New', DATEADD(DAY, -7, @Now)),
    (@SPaper, N'2026-W28', 410, 21, 18, 'Down', DATEADD(DAY, -1, @Now));

    /* ============================================================
       15. NOTIFICATIONS FOR EVERY TYPE AND READ/UNREAD STATES
       ============================================================ */

    INSERT INTO dbo.Notifications (UserId, NotificationTypeId, Title, Message, ReferenceType, ReferenceId, IsRead, CreatedAt, ReadAt)
    VALUES
    (@Assistant1Id, @NotifTaskAssigned, N'New task assigned', N'Prepare clean dialogue bubble for Crimson Café.', N'ProductionTask', @TAssigned, 0, DATEADD(HOUR, -4, @Now), NULL),
    (@Assistant3Id, @NotifTaskAssigned, N'Task in progress reminder', N'Meteor spotlight effect is due soon.', N'ProductionTask', @TInProgress, 0, DATEADD(HOUR, -2, @Now), NULL),
    (@Mangaka2Id, @NotifSubmissionUploaded, N'Assistant submitted task', N'Clean samurai silhouette line art has been submitted.', N'ProductionTask', @TSubmitted, 0, DATEADD(HOUR, -6, @Now), NULL),
    (@Assistant1Id, @NotifSubmissionReviewed, N'Submission needs revision', N'Dark room shading needs revision.', N'ProductionTask', @TRevision, 0, DATEADD(HOUR, -2, @Now), NULL),
    (@Mangaka1Id, @NotifEditorCommentCreated, N'Editor added comment', N'Editor commented on Crimson Café dialogue bubble.', N'EditorComment', @PCrimson1, 0, DATEADD(HOUR, -4, @Now), NULL),
    (@Mangaka2Id, @NotifBoardVoteSubmitted, N'Board voted on Neon Samurai', N'New board votes have been submitted for Neon Samurai.', N'Series', @SNeon, 0, DATEADD(HOUR, -18, @Now), NULL),
    (@Mangaka2Id, @NotifRankingUpdated, N'Starfall ranking improved', N'Starfall Idol Manga moved from rank 7 to rank 3.', N'Series', @SStarfall, 0, DATEADD(DAY, -1, @Now), NULL),
    (@Editor1Id, @NotifDeadlineWarning, N'Hollow Brush revision risk', N'Hollow Brush has unresolved editor comments and a close deadline.', N'Chapter', @ChHollow1, 0, DATEADD(HOUR, -1, @Now), NULL),
    (@AdminId, @NotifSystem, N'Demo seed completed', N'Extra demo seed data has been inserted successfully.', N'System', NULL, 1, @Now, @Now),
    (@Assistant2Id, @NotifSubmissionReviewed, N'Background approved', N'Moon station background has been approved.', N'ProductionTask', @TApproved, 1, DATEADD(DAY, -4, @Now), DATEADD(DAY, -4, DATEADD(HOUR, 1, @Now)));

    /* ============================================================
       16. ASSISTANT EARNINGS
       ============================================================ */

    INSERT INTO dbo.AssistantEarnings (AssistantId, TaskId, Amount, EarningStatusId, CalculatedAt, PaidAt)
    VALUES
    (@Assistant2Id, @TApproved, 130000, @EarningPaid, DATEADD(DAY, -4, @Now), DATEADD(DAY, -2, @Now)),
    (@Assistant3Id, @TInProgress, 100000, @EarningPending, @Now, NULL),
    (@Assistant1Id, @TRevision, 95000, @EarningPending, @Now, NULL),
    (@Assistant3Id, @TRejected, 0, @EarningCancelled, DATEADD(DAY, -2, @Now), NULL);

    /* ============================================================
       17. BACKGROUND JOB LOGS
       ============================================================ */

    INSERT INTO dbo.BackgroundJobLogs (JobName, Status, Message, StartedAt, FinishedAt)
    VALUES
    (N'DeadlineReminderJob', 'Succeeded', N'Sent deadline reminders to assistants with tasks due in 48 hours.', DATEADD(HOUR, -6, @Now), DATEADD(HOUR, -6, DATEADD(MINUTE, 1, @Now))),
    (N'OverdueTaskScannerJob', 'Succeeded', N'Marked Neon Samurai tone task as overdue.', DATEADD(HOUR, -2, @Now), DATEADD(HOUR, -2, DATEADD(MINUTE, 1, @Now))),
    (N'RankingRiskScannerJob', 'Succeeded', N'Paper Dragon Legacy marked as cancellation risk and Starfall marked as promoted.', DATEADD(DAY, -1, @Now), DATEADD(DAY, -1, DATEADD(MINUTE, 2, @Now))),
    (N'MonthlyEarningCalculatorJob', 'Succeeded', N'Calculated assistant earnings for approved tasks.', DATEADD(HOUR, -1, @Now), DATEADD(HOUR, -1, DATEADD(MINUTE, 1, @Now))),
    (N'AiSegmentationSyncJob', 'Failed', N'One AI job failed due to low contrast page. Manual annotation required.', DATEADD(HOUR, -3, @Now), DATEADD(HOUR, -3, DATEADD(MINUTE, 1, @Now))),
    (N'NotificationCleanupJob', 'Skipped', N'No notifications older than retention threshold.', DATEADD(HOUR, -12, @Now), DATEADD(HOUR, -12, DATEADD(MINUTE, 1, @Now)));

    /* ============================================================
       18. WORKFLOW HISTORY
       ============================================================ */

    INSERT INTO dbo.WorkflowStatusHistories (EntityName, EntityId, FromStatusCode, ToStatusCode, ChangedByUserId, Note, ChangedAt)
    VALUES
    (N'Series', @SNeon, 'Draft', 'Submitted', @Mangaka2Id, N'Mangaka submitted Neon Samurai for board review.', DATEADD(DAY, -10, @Now)),
    (N'Series', @SHollow, 'Submitted', 'RevisionRequired', @Editor1Id, N'Editor requested clearer panel readability.', DATEADD(DAY, -2, @Now)),
    (N'Series', @SStarfall, 'Approved', 'Publishing', @Board3Id, N'Board promoted Starfall to active weekly publishing.', DATEADD(DAY, -70, @Now)),
    (N'Series', @SPaper, 'Publishing', 'Cancelled', @Board1Id, N'Board cancelled the series after ranking drop.', DATEADD(DAY, -3, @Now)),

    (N'Chapter', @ChMoon1, 'ReadyForEditorReview', 'ReadyForPublication', @Editor2Id, N'Editor approved chapter for issue scheduling.', DATEADD(DAY, -2, @Now)),
    (N'Chapter', @ChHollow1, 'ReadyForEditorReview', 'EditorRevisionRequired', @Editor1Id, N'Editor requested page contrast revision.', DATEADD(DAY, -1, @Now)),

    (N'ProductionTask', @TAssigned, NULL, 'Assigned', @Mangaka1Id, N'Mangaka assigned dialogue bubble cleanup.', DATEADD(HOUR, -4, @Now)),
    (N'ProductionTask', @TInProgress, 'Assigned', 'InProgress', @Assistant3Id, N'Assistant started meteor spotlight effect.', DATEADD(DAY, -1, @Now)),
    (N'ProductionTask', @TSubmitted, 'InProgress', 'Submitted', @Assistant2Id, N'Assistant submitted clean line art.', DATEADD(HOUR, -6, @Now)),
    (N'ProductionTask', @TRevision, 'Submitted', 'RevisionRequired', @Mangaka1Id, N'Mangaka requested shading revision.', DATEADD(HOUR, -2, @Now)),
    (N'ProductionTask', @TApproved, 'Submitted', 'Approved', @Mangaka2Id, N'Mangaka approved background task.', DATEADD(DAY, -4, @Now)),
    (N'ProductionTask', @TRejected, 'Submitted', 'Rejected', @Mangaka2Id, N'Mangaka rejected light burst attempt.', DATEADD(DAY, -2, @Now)),
    (N'ProductionTask', @TOverdue, 'InProgress', 'Overdue', @AdminId, N'Worker marked task as overdue.', DATEADD(HOUR, -2, @Now)),
    (N'ProductionTask', @TCancelled, 'Assigned', 'Cancelled', @Board1Id, N'Task cancelled after series cancellation.', DATEADD(DAY, -3, @Now));

    /* ============================================================
       19. AUDIT LOGS
       ============================================================ */

    INSERT INTO dbo.AuditLogs (ActorUserId, EntityName, EntityId, ActionName, Details, CreatedAt)
    VALUES
    (@Mangaka2Id, N'Series', @SNeon, N'CreateSeries', N'Created Neon Samurai proposal.', DATEADD(DAY, -12, @Now)),
    (@Mangaka1Id, N'Series', @SCrimson, N'CreateSeries', N'Created Crimson Café draft.', DATEADD(DAY, -5, @Now)),
    (@Mangaka2Id, N'ProductionTask', @TSubmitted, N'AssignTask', N'Assigned clean line art task to assistant.', DATEADD(DAY, -6, @Now)),
    (@Assistant2Id, N'TaskSubmission', @TSubmitted, N'UploadSubmission', N'Uploaded clean line art submission.', DATEADD(HOUR, -6, @Now)),
    (@Editor1Id, N'EditorComment', @PHollow1, N'CreateEditorComment', N'Commented on Hollow Brush page contrast.', DATEADD(DAY, -1, @Now)),
    (@Board3Id, N'RankingRecords', @SStarfall, N'CalculateRanking', N'Calculated Starfall ranking improvement.', DATEADD(DAY, -1, @Now)),
    (@AdminId, N'DatabaseSeed', NULL, N'InsertExtraSeed', N'Inserted v3 extra seed data for PRN222 coding demo.', @Now);

    COMMIT TRANSACTION;

    PRINT N'Extra seed data inserted successfully.';

    /* ============================================================
       20. VALIDATION QUERIES FOR CODING
       ============================================================ */

    SELECT 'Extra seed data inserted successfully.' AS Message;

    SELECT r.RoleCode, COUNT(ur.UserId) AS UserCount
    FROM dbo.Roles r
    LEFT JOIN dbo.UserRoles ur ON r.RoleId = ur.RoleId
    GROUP BY r.RoleCode
    ORDER BY r.RoleCode;

    SELECT ss.StatusCode, COUNT(s.SeriesId) AS SeriesCount
    FROM dbo.SeriesStatuses ss
    LEFT JOIN dbo.Series s ON ss.SeriesStatusId = s.SeriesStatusId
    GROUP BY ss.StatusCode
    ORDER BY ss.StatusCode;

    SELECT cs.StatusCode, COUNT(c.ChapterId) AS ChapterCount
    FROM dbo.ChapterStatuses cs
    LEFT JOIN dbo.Chapters c ON cs.ChapterStatusId = c.ChapterStatusId
    GROUP BY cs.StatusCode
    ORDER BY cs.StatusCode;

    SELECT ts.StatusCode, COUNT(t.TaskId) AS TaskCount
    FROM dbo.TaskStatuses ts
    LEFT JOIN dbo.ProductionTasks t ON ts.TaskStatusId = t.TaskStatusId
    GROUP BY ts.StatusCode
    ORDER BY ts.StatusCode;

    SELECT ss.StatusCode, COUNT(sub.SubmissionId) AS SubmissionCount
    FROM dbo.SubmissionStatuses ss
    LEFT JOIN dbo.TaskSubmissions sub ON ss.SubmissionStatusId = sub.SubmissionStatusId
    GROUP BY ss.StatusCode
    ORDER BY ss.StatusCode;

    SELECT Status AS AiJobStatus, COUNT(*) AS JobCount
    FROM dbo.AiSegmentationJobs
    GROUP BY Status
    ORDER BY Status;

    SELECT TOP 20 * FROM dbo.vw_ChapterProgress ORDER BY SeriesTitle, ChapterNumber;
    SELECT TOP 20 * FROM dbo.vw_SeriesLatestRanking ORDER BY CancellationRiskScore DESC;
    SELECT TOP 30 Title, Message, IsRead, CreatedAt FROM dbo.Notifications ORDER BY CreatedAt DESC;

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorLine INT = ERROR_LINE();

    PRINT N'Extra seed failed.';
    SELECT @ErrorMessage AS ErrorMessage, @ErrorLine AS ErrorLine;
    THROW;
END CATCH;
GO
