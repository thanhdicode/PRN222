# PHASE 5 — Worker Service + Final Integration + Demo
# Detailed Implementation Guide

**PRN222 Chapter Proven**: Chapter 08 — Worker Service / Background Tasks
**Depends on**: Phase 2, Phase 3, AND Phase 4 Complete
**Status**: LOCKED until Phase 4 DONE

---

## 1. What This Phase Proves in PRN222

Chapter 08 Worker Service mastery:
- BackgroundService base class
- IHostedService pattern
- IServiceScopeFactory to resolve scoped dependencies from singleton context
- CancellationToken for clean shutdown
- Timed background loops with Task.Delay
- ILogger for structured logging
- Writing results to database (BackgroundJobLogs table)

Final integration demonstrates:
- All PRN222 chapters working together
- Complete demo flow from start to finish
- Build quality and code organization

---

## 2. Worker Files to Create

All workers go in MangaWorkflow.Worker/ project.

### 2.1 DeadlineReminderWorker.cs

Purpose: Find tasks due within 48 hours. Create DeadlineWarning notifications.

```csharp
public class DeadlineReminderWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DeadlineReminderWorker> _logger;
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var taskService = scope.ServiceProvider.GetRequiredService<ITaskWorkflowService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var logRepo = scope.ServiceProvider.GetRequiredService<IBackgroundJobLogRepository>();

            var jobLog = await logRepo.StartJobAsync("DeadlineReminderWorker", stoppingToken);
            try
            {
                var tasks = await taskService.GetTasksDueWithinHoursAsync(48, stoppingToken);
                int count = 0;
                foreach (var task in tasks)
                {
                    await notificationService.CreateAndSendAsync(
                        task.AssignedToUserId,
                        "DeadlineWarning",
                        "Task Deadline Approaching",
                        $"Task '{task.Title}' is due within 48 hours.",
                        stoppingToken);
                    count++;
                }
                await logRepo.CompleteJobAsync(jobLog.LogId, $"Reminded {count} tasks.", stoppingToken);
                _logger.LogInformation("DeadlineReminderWorker: {count} reminders sent.", count);
            }
            catch (Exception ex)
            {
                await logRepo.FailJobAsync(jobLog.LogId, ex.Message, stoppingToken);
                _logger.LogError(ex, "DeadlineReminderWorker failed.");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}
```

### 2.2 OverdueTaskScannerWorker.cs

Purpose: Find tasks past deadline not yet Approved/Cancelled. Update to Overdue status. Notify.

Logic:
- Find ProductionTasks where Deadline < DateTime.UtcNow
  AND StatusCode NOT IN (Approved, Rejected, Cancelled, Overdue)
- Set StatusCode = "Overdue"
- Insert WorkflowStatusHistory record
- Create notification for assigned assistant AND owning Mangaka
- Write BackgroundJobLogs

Interval: every 5 minutes.

### 2.3 RankingRiskWorker.cs

Purpose: Find series with downward ranking trend. Update CancellationRiskScore. Notify.

Logic:
- Query latest RankingRecords per series
- If trend = "Down" or rank worsened by > 3 positions compared to previous:
  - Update Series.CancellationRiskScore (if column exists, otherwise update RankingRecord)
  - Create notification for series Mangaka
- Write BackgroundJobLogs

Interval: every 60 minutes.

### 2.4 NotificationCleanupWorker.cs

Purpose: Delete or archive old read notifications.

Logic:
- Find Notifications where IsRead = 1 AND CreatedAt < DateTime.UtcNow.AddDays(-30)
- Delete them (or set IsArchived = 1 if column exists)
- Write BackgroundJobLogs
- If nothing to clean, log "Skipped — no old notifications."

Interval: every 24 hours (use DateTime.UtcNow.Date comparison or Task.Delay 24h).

### 2.5 MonthlyEarningCalculatorWorker.cs

Purpose: Find approved tasks without corresponding AssistantEarning record. Create earning.

Logic:
- Find ProductionTasks where StatusCode = "Approved"
  AND no AssistantEarning row exists for this task
- Create AssistantEarning record:
  - AssistantId = task.AssignedToUserId
  - Amount = task calculated rate (use a fixed demo rate or task type rate)
  - EarningStatusCode = "Pending"
  - CalculatedAt = UtcNow
- Write BackgroundJobLogs

Interval: once per day.

---

## 3. Required Worker Application Layer Additions

ITaskWorkflowService additions:
```csharp
Task<List<TaskDeadlineReminderDto>> GetTasksDueWithinHoursAsync(int hours, CancellationToken ct = default);
Task<List<TaskOverdueDto>> GetOverdueTasksAsync(CancellationToken ct = default);
Task MarkTasksAsOverdueAsync(List<Guid> taskIds, CancellationToken ct = default);
```

IBackgroundJobLogRepository:
```csharp
Task<BackgroundJobLog> StartJobAsync(string workerName, CancellationToken ct = default);
Task CompleteJobAsync(Guid logId, string summary, CancellationToken ct = default);
Task FailJobAsync(Guid logId, string errorMessage, CancellationToken ct = default);
```

MangaWorkflow.Infrastructure/Repositories/BackgroundJobLogRepository.cs implements it.

---

## 4. Worker Program.cs Registration

MangaWorkflow.Worker/Program.cs:
```csharp
using MangaWorkflow.Application;
using MangaWorkflow.Infrastructure;
using MangaWorkflow.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddHostedService<DeadlineReminderWorker>();
builder.Services.AddHostedService<OverdueTaskScannerWorker>();
builder.Services.AddHostedService<RankingRiskWorker>();
builder.Services.AddHostedService<NotificationCleanupWorker>();
builder.Services.AddHostedService<MonthlyEarningCalculatorWorker>();

var host = builder.Build();
host.Run();
```

Worker appsettings.json must have same connection string as Web project.

---

## 5. Admin Manual Trigger (Demo Shortcut)

For demo presentation, add an Admin MVC screen to manually trigger workers:

MangaWorkflow.Web/Areas/Admin/Controllers/BackgroundJobsController.cs:
```csharp
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BackgroundJobsController : Controller
{
    // GET /Admin/BackgroundJobs
    public async Task<IActionResult> Index(CancellationToken ct)
    // Show list of recent BackgroundJobLogs

    // POST /Admin/BackgroundJobs/RunOverdueScanner
    [HttpPost]
    public async Task<IActionResult> RunOverdueScanner(CancellationToken ct)
    // Manually calls same service method as OverdueTaskScannerWorker

    // POST /Admin/BackgroundJobs/RunDeadlineReminder
    [HttpPost]
    public async Task<IActionResult> RunDeadlineReminder(CancellationToken ct)
}
```

This makes demo deterministic — no need to wait for background intervals.

---

## 6. Final Integration Verification

Before marking Phase 5 complete, verify EVERY feature works end-to-end:

### Authentication
[ ] Login with admin@manga.local works
[ ] Login with mangaka@manga.local works
[ ] Login with assistant@manga.local works
[ ] Login with editor@manga.local works
[ ] Login with board@manga.local works
[ ] Logout works
[ ] Unauthorized access redirects to login

### MVC (Phase 2)
[ ] Admin Users CRUD works
[ ] Mangaka Series CRUD works
[ ] Mangaka Chapter CRUD works
[ ] Mangaka Pages list/create works
[ ] Board Series Review vote works
[ ] Board Rankings create works

### Razor Pages (Phase 3)
[ ] Assistant TaskInbox loads
[ ] Assistant can start a task
[ ] Assistant can submit a task with file upload
[ ] Mangaka ReviewSubmissions loads
[ ] Mangaka can approve/reject a submission
[ ] Mangaka PageRegions page loads and regions can be added
[ ] Editor PageComments page loads and comments can be added and resolved

### Blazor (Phase 4)
[ ] At least one Blazor dashboard renders with real data
[ ] Dashboards show correct counts

### SignalR (Phase 4)
[ ] Notification badge increments in real time
[ ] At least 3 event types work

### Worker (Phase 5)
[ ] OverdueTaskScannerWorker runs and writes BackgroundJobLogs
[ ] DeadlineReminderWorker runs and writes BackgroundJobLogs
[ ] Admin manual trigger works for at least one worker
[ ] BackgroundJobLogs screen shows recent job logs

---

## 7. Final Deliverables

### 7.1 docs/FINAL_DEMO_SCRIPT.md

Content:
- Step-by-step demo flow (15-20 minutes)
- Which URL to navigate to at each step
- What to say at each step
- Which PRN222 chapter each step demonstrates

Step outline:
1. Show running app (dotnet run --project MangaWorkflow.Web)
2. Login as Admin — show user management
3. Login as Mangaka — create series, submit series
4. Open Board account — vote on series, enter ranking
5. Mangaka creates chapter and page
6. Mangaka adds region and task (assigns to assistant)
7. Login as Assistant — start task, upload submission (show SignalR notification fires)
8. Login as Mangaka — review and approve submission
9. Login as Editor — add page comment, resolve it
10. Show Blazor dashboard with real data
11. Run Worker (or use Admin manual trigger) — show BackgroundJobLogs written
12. Summarize PRN222 chapter coverage

### 7.2 docs/FINAL_QA_CHECKLIST.md

Content: Complete checklist of every screen and feature, with Pass/Fail columns.

### 7.3 docs/FINAL_REPORT_OUTLINE.md

Content:
- Project introduction
- Architecture diagram (text-based is OK)
- PRN222 chapter coverage table
- Key features implemented
- Demo accounts and run instructions
- Known limitations

### 7.4 docs/PHASE_5_COMPLETION_REPORT.md

Content: What was done in Phase 5, files created, commands run, test results.

---

## 8. Phase 5 Commands

```
dotnet clean
dotnet restore
dotnet build
dotnet run --project MangaWorkflow.Tools.DbSmokeTest
dotnet run --project MangaWorkflow.Web
dotnet run --project MangaWorkflow.Worker
```

---

## 9. Phase 5 Completion Criteria

[ ] dotnet build passes with zero errors
[ ] dotnet run DbSmokeTest passes
[ ] All 5 workers exist and compile
[ ] At least 2 workers run without crashing
[ ] At least 1 worker writes to BackgroundJobLogs
[ ] Admin manual trigger works for worker demo
[ ] Full end-to-end demo script works without errors
[ ] All Phase 2, 3, 4 features still work
[ ] docs/FINAL_DEMO_SCRIPT.md created
[ ] docs/FINAL_QA_CHECKLIST.md created
[ ] docs/FINAL_REPORT_OUTLINE.md created
[ ] docs/PROJECT_STATUS.md updated with [DONE] Phase 5
[ ] docs/PHASE_5_COMPLETION_REPORT.md created
[ ] README.md contains final run instructions
