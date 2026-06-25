# 02 — PRN222 Alignment Matrix

This file explains how the project demonstrates every important PRN222 learning outcome. Do not implement features that cannot be mapped here unless they are tiny UI polish items.

## Course outcome mapping

| PRN222 topic / CLO | Required implementation in MangaWorkflow | Minimum demo evidence |
|---|---|---|
| C# and .NET fundamentals | Entity models, services, LINQ queries, DTO/ViewModels | Code compiles, service methods work |
| EF Core | Database-first scaffold from SQL Server, DbContext, async queries | CRUD against MangaWorkflowDB |
| Networking Programming | Optional mock API call for AI segmentation or HTTP-style service abstraction | AI segmentation job can be queued/mock-returned |
| Async/Parallel | Async EF calls; optional `Task.WhenAll` for batch notifications | No blocking `.Result`/`.Wait()` in request code |
| Dependency Injection | Register repositories/services/hubs/workers | Constructor injection in controllers/pages/services |
| ASP.NET Core MVC | Series, Chapters, Board, Ranking, Admin screens | Controller + views + model binding + validation |
| Razor Pages | Assistant task inbox and submission workflow | `Pages/Assistant/Tasks` with `OnGetAsync`, `OnPostAsync` |
| Blazor | Dashboard with progress/ranking/task counts | `.razor` dashboard component/page |
| SignalR | Realtime notification for task/submission/review/comment/ranking | Hub + JS/Blazor client receives message |
| Worker Service | Deadline/overdue/ranking jobs | Worker writes `BackgroundJobLogs`, creates notifications |
| Team project | Clear module ownership and demo flow | README + commit history + presentation |

## Chapter-by-chapter implementation plan

### Chapter 01 — Networking Programming

Use only light networking to avoid overengineering. Recommended implementation:

- `IAiSegmentationClient`
- `MockAiSegmentationClient`
- method: `Task<AiSegmentationResultDto> DetectRegionsAsync(Guid pageId)`

This simulates an external AI service and supports the optional AI requirement from the topic list.

### Chapter 02 — Asynchronous and Parallel Programming

Required coding evidence:

- All repository methods use `Async` suffix.
- Use EF Core async methods.
- Notification broadcast can use `Task.WhenAll` if multiple users are notified.
- Worker service methods accept `CancellationToken`.

Example:

```csharp
var tasks = users.Select(u => _notificationService.CreateAsync(u.UserId, dto, cancellationToken));
await Task.WhenAll(tasks);
```

### Chapter 03 — Dependency Injection

Required services:

```text
ISeriesService
IChapterService
IPageService
ITaskService
ISubmissionService
IEditorCommentService
IBoardVotingService
IRankingService
INotificationService
IDashboardService
IAiSegmentationService
```

Register in `Program.cs`:

```csharp
builder.Services.AddScoped<ISeriesService, SeriesService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<DeadlineReminderWorker>();
```

### Chapter 04 — ASP.NET Core MVC

MVC modules:

- `SeriesController`
- `ChaptersController`
- `MangaPagesController`
- `TasksController`
- `BoardController`
- `RankingController`
- `AdminController`

MVC must show classic CRUD/list/detail/edit/search/filter/report patterns.

### Chapter 05 — Razor Pages

Razor Pages modules:

```text
/Pages/Assistant/Tasks/Index.cshtml
/Pages/Assistant/Tasks/Details.cshtml
/Pages/Assistant/Tasks/Submit.cshtml
/Pages/Assistant/Earnings/Index.cshtml
```

Use PageModel handlers:

```csharp
OnGetAsync()
OnPostSubmitAsync()
OnPostStartAsync()
```

### Chapter 06 — Blazor

Blazor module:

```text
/Components/Pages/Dashboard/MangakaDashboard.razor
/Components/Pages/Dashboard/EditorDashboard.razor
/Components/Pages/Dashboard/BoardDashboard.razor
```

Minimum widgets:

- Chapter progress.
- Task status counts.
- Latest ranking.
- Unread notifications.
- Background job status.

### Chapter 07 — SignalR

Hubs:

```text
NotificationHub
WorkflowHub
```

Minimum events:

- `TaskAssigned`
- `SubmissionUploaded`
- `SubmissionReviewed`
- `EditorCommentCreated`
- `BoardVoteSubmitted`
- `RankingUpdated`
- `DeadlineWarning`

### Chapter 08 — Worker Service

Workers/jobs:

- `DeadlineReminderWorker`
- `OverdueTaskScannerWorker`
- `RankingRiskScannerWorker`
- `NotificationCleanupWorker`
- optional `MonthlyEarningCalculatorWorker`

Minimum worker acceptance:

- Runs without crashing.
- Logs to `BackgroundJobLogs`.
- Creates at least one notification when a condition is met.
- Uses DI scopes correctly.

## Recommended grading explanation

When presenting, explicitly say:

> The project was intentionally designed to cover PRN222. MVC is used for management modules, Razor Pages for assistant task workflow, Blazor for dashboard, SignalR for realtime notifications, Worker Service for deadline/ranking automation, EF Core/SQL Server for persistence, DI for architecture, and async/await for scalable request handling.
