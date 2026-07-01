# 03 — Domain Model and Database Guide

## Database scripts

Run in this order:

1. `db/MangaWorkflowDB_v2_demo_ready.sql`
2. `db/MangaWorkflowDB_v3_extra_seed_demo_data.sql`

The database name is `MangaWorkflowDB`.

## Database purpose

The database is designed to support every demo module without needing manual data entry during early development. It contains both schema and seed data for all statuses.

## Table groups

### Lookup/status tables

These tables support dropdowns, filters, and status transitions:

- `SeriesStatuses`
- `PublicationSchedules`
- `ChapterStatuses`
- `ManuscriptStatuses`
- `PageStatuses`
- `RegionTypes`
- `TaskTypes`
- `TaskStatuses`
- `SubmissionStatuses`
- `CommentStatuses`
- `VoteValues`
- `DecisionTypes`
- `NotificationTypes`
- `EarningStatuses`

Do not hardcode status IDs. Query by `StatusCode` or create constants.

Recommended constants:

```csharp
public static class TaskStatusCodes
{
    public const string Assigned = "Assigned";
    public const string InProgress = "InProgress";
    public const string Submitted = "Submitted";
    public const string RevisionRequired = "RevisionRequired";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string Overdue = "Overdue";
    public const string Cancelled = "Cancelled";
}
```

### Auth tables

- `Users`
- `Roles`
- `UserRoles`

These are custom auth tables for simple PRN222 demo. You may later replace them with ASP.NET Core Identity, but do not change domain tables.

Seed users:

```text
admin@manga.local
mangaka@manga.local
assistant@manga.local
editor@manga.local
board@manga.local
mangaka2@manga.local
assistant2@manga.local
assistant3@manga.local
editor2@manga.local
board2@manga.local
board3@manga.local
```

Password for every seeded demo account is `test123@`. The database stores BCrypt hashes, not plaintext passwords.

### Core manga tables

- `Series`
- `SeriesTeamMembers`
- `Chapters`
- `Manuscripts`
- `MangaPages`
- `PageRegions`

These support the studio production hierarchy:

```text
Series
└── Chapters
    ├── Manuscripts
    └── MangaPages
        └── PageRegions
```

### Task/submission tables

- `ProductionTasks`
- `TaskSubmissions`
- `AssistantEarnings`

Workflow:

```text
ProductionTask Assigned
→ InProgress
→ Submitted
→ Approved / Rejected / RevisionRequired
→ optional earning calculation
```

### Editor/board tables

- `EditorComments`
- `BoardVotes`
- `PublishingDecisions`

Editor comments are page-based. Board votes are series-based.

### Publication/ranking tables

- `PublicationIssues`
- `ChapterPublications`
- `ReaderVoteData`
- `RankingRecords`

Reader vote data represents raw input. Ranking records represent calculated/stored ranking result.

### Realtime/background/audit tables

- `Notifications`
- `BackgroundJobLogs`
- `WorkflowStatusHistories`
- `AuditLogs`

SignalR should create and push notifications. Worker services should write logs.

### Optional AI tables

- `AiSegmentationJobs`

This supports mocked AI segmentation. It is not real model training.

## Important views

### `vw_ChapterProgress`

Use for dashboard chapter progress:

- Total tasks.
- Approved tasks.
- Overdue tasks.
- Progress percent.
- Deadline.

### `vw_AssistantMonthlyEarnings`

Use for assistant earnings dashboard.

### `vw_SeriesLatestRanking`

Use for latest ranking and cancellation risk dashboard.

## EF Core scaffolding command

For SQL Server local instance:

```bash
dotnet ef dbcontext scaffold "Server=.;Database=MangaWorkflowDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models -c MangaWorkflowDbContext --context-dir Data --force
```

For LocalDB:

```bash
dotnet ef dbcontext scaffold "Server=(localdb)\MSSQLLocalDB;Database=MangaWorkflowDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models -c MangaWorkflowDbContext --context-dir Data --force
```

If using separate class libraries, run scaffolding into `MangaWorkflow.BusinessObjects` and place DbContext in `MangaWorkflow.DataAccessObjects`.

## Database coding rules

- Do not assume IDs. Query lookup IDs by code.
- Do not delete seed data unless testing delete screens.
- Prefer soft delete where available (`Series.IsDeleted`).
- Keep demo data intact for presentation.
- Use views for dashboard read-only data.
- Use service methods for workflow transitions.

## Typical queries for debugging

```sql
SELECT * FROM dbo.Users;
SELECT * FROM dbo.Series;
SELECT * FROM dbo.Chapters;
SELECT * FROM dbo.ProductionTasks;
SELECT * FROM dbo.TaskSubmissions;
SELECT * FROM dbo.Notifications ORDER BY CreatedAt DESC;
SELECT * FROM dbo.vw_ChapterProgress;
SELECT * FROM dbo.vw_SeriesLatestRanking;
SELECT * FROM dbo.BackgroundJobLogs ORDER BY StartedAt DESC;
```
