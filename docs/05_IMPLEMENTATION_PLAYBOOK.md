# 05 — Implementation Playbook

This is the step-by-step coding plan. Follow it strictly to avoid wasting time.

## Phase 0 — Database ready

1. Run `MangaWorkflowDB_v2_demo_ready.sql`.
2. Run `MangaWorkflowDB_v3_extra_seed_demo_data.sql`.
3. Verify tables and seed data:

```sql
SELECT COUNT(*) FROM dbo.Series;
SELECT COUNT(*) FROM dbo.ProductionTasks;
SELECT COUNT(*) FROM dbo.Notifications;
SELECT * FROM dbo.vw_ChapterProgress;
```

## Phase 1 — Solution setup

Create solution:

```bash
dotnet new sln -n MangaWorkflowSystem
```

Create projects:

```bash
dotnet new classlib -n MangaWorkflow.BusinessObjects
dotnet new classlib -n MangaWorkflow.DataAccessObjects
dotnet new classlib -n MangaWorkflow.Repositories
dotnet new classlib -n MangaWorkflow.Services
dotnet new mvc -n MangaWorkflow.Web
dotnet new worker -n MangaWorkflow.Worker
```

Add to solution:

```bash
dotnet sln add MangaWorkflow.BusinessObjects
dotnet sln add MangaWorkflow.DataAccessObjects
dotnet sln add MangaWorkflow.Repositories
dotnet sln add MangaWorkflow.Services
dotnet sln add MangaWorkflow.Web
dotnet sln add MangaWorkflow.Worker
```

Project references:

```bash
dotnet add MangaWorkflow.DataAccessObjects reference MangaWorkflow.BusinessObjects
dotnet add MangaWorkflow.Repositories reference MangaWorkflow.BusinessObjects MangaWorkflow.DataAccessObjects
dotnet add MangaWorkflow.Services reference MangaWorkflow.BusinessObjects MangaWorkflow.Repositories
dotnet add MangaWorkflow.Web reference MangaWorkflow.BusinessObjects MangaWorkflow.Services
dotnet add MangaWorkflow.Worker reference MangaWorkflow.BusinessObjects MangaWorkflow.Services
```

Packages:

```bash
dotnet add MangaWorkflow.DataAccessObjects package Microsoft.EntityFrameworkCore.SqlServer
dotnet add MangaWorkflow.DataAccessObjects package Microsoft.EntityFrameworkCore.Design
dotnet add MangaWorkflow.Web package Microsoft.EntityFrameworkCore.SqlServer
dotnet add MangaWorkflow.Web package Microsoft.AspNetCore.SignalR.Client
dotnet add MangaWorkflow.Worker package Microsoft.EntityFrameworkCore.SqlServer
```

## Phase 2 — EF Core scaffold

Run scaffold into the appropriate projects. If this is too difficult across class libraries, scaffold into Web first and move files later.

Simple scaffold:

```bash
dotnet ef dbcontext scaffold "Server=.;Database=MangaWorkflowDB;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models -c MangaWorkflowDbContext --context-dir Data --force --project MangaWorkflow.Web
```

Recommended after scaffold:

- Move entities to BusinessObjects.
- Move DbContext to DataAccessObjects.
- Fix namespaces.
- Add project references.

If moving causes too many errors, keep scaffold in Web and continue. Functioning demo beats perfect architecture.

## Phase 3 — Basic DI and app startup

In `MangaWorkflow.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MangaWorkflowDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

In `Program.cs`:

```csharp
builder.Services.AddDbContext<MangaWorkflowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISeriesService, SeriesService>();
builder.Services.AddScoped<IChapterService, ChapterService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddSignalR();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
```

Map endpoints:

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapHub<NotificationHub>("/hubs/notifications");
```

## Phase 4 — Build minimal read screens first

Order:

1. Home dashboard showing seed counts.
2. Series index.
3. Series details.
4. Chapter list by series.
5. Task list.
6. Notifications list.

Goal: prove database connection and scaffold models work.

## Phase 5 — MVC CRUD

Implement MVC modules:

### SeriesController

Actions:

- `Index(status, keyword)`
- `Details(id)`
- `Create()` GET/POST
- `Edit(id)` GET/POST
- `Submit(id)` POST

### ChaptersController

Actions:

- `Index(seriesId)`
- `Details(id)`
- `Create(seriesId)` GET/POST
- `Edit(id)` GET/POST

### BoardController

Actions:

- `PendingSeries()`
- `Vote(seriesId)` GET/POST
- `Decision(seriesId)` POST

### RankingController

Actions:

- `Index()`
- `Import()` GET/POST
- `Latest()`

## Phase 6 — Razor Pages assistant flow

Create pages:

```text
Pages/Assistant/Tasks/Index.cshtml
Pages/Assistant/Tasks/Details.cshtml
Pages/Assistant/Tasks/Submit.cshtml
Pages/Assistant/Earnings/Index.cshtml
```

Start with seeded assistant email or temporary dropdown. Later connect to auth.

## Phase 7 — Submission review

Implement Mangaka review:

- List submitted tasks for series owned by mangaka.
- Review submission.
- Approve/reject/revision.
- Create notification.
- Push SignalR event.

## Phase 8 — Editor comments

Implement:

- List assigned series/pages.
- Create editor comment.
- Resolve comment.
- Notify mangaka.

Bounding-box UI can be simple numeric fields first. Canvas selection can come later.

## Phase 9 — SignalR

Implement `NotificationHub`.

Server-side in notification service:

```csharp
await _hub.Clients.All.SendAsync("ReceiveNotification", notificationDto, cancellationToken);
```

Client-side:

```javascript
const connection = new signalR.HubConnectionBuilder()
  .withUrl("/hubs/notifications")
  .build();

connection.on("ReceiveNotification", function (message) {
  console.log(message);
});

connection.start();
```

Start with `Clients.All`, then improve to user groups.

## Phase 10 — Blazor dashboard

Create dashboard using existing views:

- `vw_ChapterProgress`
- `vw_SeriesLatestRanking`
- `Notifications`
- `BackgroundJobLogs`

The dashboard does not need complex charts. Cards and tables are enough.

## Phase 11 — Worker Service

Implement one worker first:

- `OverdueTaskScannerWorker`

Then add:

- `DeadlineReminderWorker`
- `RankingRiskScannerWorker`
- `NotificationCleanupWorker`

Each worker logs result to `BackgroundJobLogs`.

## Phase 12 — Optional AI segmentation

Implement only after core demo works.

Flow:

1. User clicks Auto Detect Regions.
2. Create `AiSegmentationJobs` row with Queued.
3. Mock service returns JSON boxes.
4. Update job to Succeeded.
5. Insert `PageRegions` with `SourceType = AI`.

## Phase 13 — Polish and demo

- Add validation messages.
- Add role-based menu.
- Add README.
- Add screenshots if possible.
- Prepare demo script.
- Ensure database seed can be reset.

## Stop conditions

Stop adding features when these work:

- Series CRUD.
- Task assignment.
- Assistant submission.
- Mangaka review.
- Editor comment.
- Board vote.
- Ranking view.
- SignalR notification.
- Worker log.
- Blazor dashboard.

Anything else is optional.
