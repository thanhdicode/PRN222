# Phase 5 Implementation Prompt
# Copy this entire prompt and paste it to your coding AI agent

---

You are a senior .NET developer implementing Phase 5 of the PRN222 MangaWorkflow project.

## Prerequisite

Phases 2, 3, AND 4 must ALL be COMPLETE before you start. Check docs/PROJECT_STATUS.md.
If any earlier phase is not marked [DONE], stop and do not proceed.

## Your Mission

Implement Phase 5: Worker Service background jobs + Final Integration + Demo preparation.
This is the FINAL phase.

## Read These Files First

1. AGENTS.md
2. docs/AI_AGENT_EXECUTION_RULES.md
3. docs/PHASE_5_WORKER_FINAL_DEMO_PLAN.md
4. docs/09_SIGNALR_WORKER_PLAYBOOK.md
5. docs/PROJECT_STATUS.md

## What You Must Build

### A. Worker Service Files

All workers go in MangaWorkflow.Worker/ project.
All inherit from BackgroundService.
All use IServiceScopeFactory to resolve scoped services.
All use CancellationToken for clean shutdown.
All write to BackgroundJobLogs table.

Create:
- MangaWorkflow.Worker/DeadlineReminderWorker.cs
  Logic: every 30 minutes, find tasks due within 48 hours, create DeadlineWarning notifications
  Write BackgroundJobLog with result count

- MangaWorkflow.Worker/OverdueTaskScannerWorker.cs
  Logic: every 5 minutes, find tasks past deadline not yet Overdue/Approved/Rejected/Cancelled
  Update task status to Overdue
  Create notification for assistant and Mangaka
  Write BackgroundJobLog

- MangaWorkflow.Worker/RankingRiskWorker.cs
  Logic: every 60 minutes, find series with downward ranking trend
  Update cancellation risk score
  Create notification for Mangaka
  Write BackgroundJobLog

- MangaWorkflow.Worker/NotificationCleanupWorker.cs
  Logic: every 24 hours, find read notifications older than 30 days
  Delete or archive them
  Write BackgroundJobLog (or log "Skipped" if nothing to clean)

- MangaWorkflow.Worker/MonthlyEarningCalculatorWorker.cs
  Logic: daily, find Approved tasks with no AssistantEarning record
  Create AssistantEarning records
  Write BackgroundJobLog

Worker skeleton (follow this pattern):
```csharp
public class OverdueTaskScannerWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OverdueTaskScannerWorker> _logger;

    public OverdueTaskScannerWorker(IServiceScopeFactory scopeFactory, ILogger<OverdueTaskScannerWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            // resolve services from scope
            // do work
            // write BackgroundJobLog
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

### B. Worker Program.cs

Update MangaWorkflow.Worker/Program.cs:
```csharp
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddHostedService<DeadlineReminderWorker>();
builder.Services.AddHostedService<OverdueTaskScannerWorker>();
builder.Services.AddHostedService<RankingRiskWorker>();
builder.Services.AddHostedService<NotificationCleanupWorker>();
builder.Services.AddHostedService<MonthlyEarningCalculatorWorker>();
```

### C. Application Layer Additions for Workers

Add to ITaskWorkflowService:
- GetTasksDueWithinHoursAsync(int hours)
- GetOverdueTasksAsync()
- MarkTasksAsOverdueAsync(List<Guid> taskIds)

Create:
- MangaWorkflow.Application/Interfaces/Repositories/IBackgroundJobLogRepository.cs
- MangaWorkflow.Infrastructure/Repositories/BackgroundJobLogRepository.cs

IBackgroundJobLogRepository methods:
- StartJobAsync(string workerName, CancellationToken ct)
- CompleteJobAsync(Guid logId, string summary, CancellationToken ct)
- FailJobAsync(Guid logId, string errorMessage, CancellationToken ct)

### D. Admin Manual Trigger (Demo Shortcut)

MangaWorkflow.Web/Areas/Admin/Controllers/BackgroundJobsController.cs:
- GET /Admin/BackgroundJobs — show BackgroundJobLogs list (latest 50)
- POST /Admin/BackgroundJobs/RunOverdueScanner — manually call OverdueTask service method
- POST /Admin/BackgroundJobs/RunDeadlineReminder — manually call DeadlineReminder service method

This makes the demo deterministic without waiting for background intervals.

### E. Final Integration Verification

Before creating deliverables, test EVERY screen:
- Authentication: all 5 demo accounts login/logout
- Admin Users CRUD
- Mangaka Series/Chapter/Page CRUD
- Series submit -> Board vote
- Mangaka PageRegions + Task creation
- Assistant TaskInbox, TaskDetail, SubmitTask
- Mangaka ReviewSubmissions approval/rejection
- Editor PageComments add and resolve
- Blazor dashboard shows real data
- SignalR notification fires on task submission, editor comment, board vote
- Worker writes BackgroundJobLogs
- Admin manual worker trigger works

Fix any remaining issues before creating final deliverables.

### F. Create Final Deliverable Documents

docs/FINAL_DEMO_SCRIPT.md:
- 15-20 minute demo flow
- Step-by-step with exact URLs
- Which PRN222 chapter each step demonstrates
- What to say at each step

docs/FINAL_QA_CHECKLIST.md:
- Every screen and feature listed with Pass/Fail columns
- Space to fill in test results during QA

docs/FINAL_REPORT_OUTLINE.md:
- Project introduction section
- Architecture overview section
- PRN222 chapter coverage table
- Key features section
- Demo accounts and run instructions section
- Known limitations section

## Commands to Run

```
dotnet clean
dotnet restore
dotnet build
dotnet run --project MangaWorkflow.Tools.DbSmokeTest
dotnet run --project MangaWorkflow.Web
dotnet run --project MangaWorkflow.Worker
```

## When Done

Create docs/PHASE_5_COMPLETION_REPORT.md
Update docs/PROJECT_STATUS.md with [DONE] Phase 5 — COMPLETE
Update README.md with final run instructions including Worker
