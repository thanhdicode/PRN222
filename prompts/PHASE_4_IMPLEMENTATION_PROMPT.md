# Phase 4 Implementation Prompt
# Copy this entire prompt and paste it to your coding AI agent

---

You are a senior .NET developer implementing Phase 4 of the PRN222 MangaWorkflow project.

## Prerequisite

Phase 2 AND Phase 3 must be COMPLETE before you start. Check docs/PROJECT_STATUS.md.
If either phase is not marked [DONE], stop and do not proceed.

## Your Mission

Implement ONLY Phase 4: Blazor Server Dashboards + SignalR Realtime Notifications.
Do NOT implement Worker jobs (Phase 5).
Do NOT rewrite any MVC or Razor Pages screens in Blazor.

## Read These Files First

1. AGENTS.md
2. docs/AI_AGENT_EXECUTION_RULES.md
3. docs/PHASE_4_BLAZOR_SIGNALR_PLAN.md
4. docs/09_SIGNALR_WORKER_PLAYBOOK.md
5. docs/PROJECT_STATUS.md

## What You Must Build

### A. SignalR Hubs

MangaWorkflow.Web/Hubs/NotificationHub.cs:
- Inherits Hub
- OnConnectedAsync: adds user to role group (role-Mangaka, role-Assistant, etc.)

MangaWorkflow.Web/Hubs/WorkflowHub.cs:
- Simple hub for workflow events

Program.cs additions:
- app.MapHub<NotificationHub>("/hubs/notifications")
- app.MapHub<WorkflowHub>("/hubs/workflow")

### B. NotificationService SignalR Integration

Expand existing NotificationService to inject IHubContext<NotificationHub>.
In CreateAndSendAsync: after inserting notification to DB, push:
```csharp
await _hubContext.Clients.All.SendAsync("ReceiveNotification", payload, ct);
```
Start with Clients.All. Improve to user groups after basic flow works.

New INotificationService methods:
- GetUnreadAsync(Guid userId, int count = 5)
- MarkReadAsync(Guid notificationId)
- MarkAllReadAsync(Guid userId)

### C. Notification DTOs

MangaWorkflow.Application/DTOs/Notifications/NotificationDto.cs

### D. Notifications Controller

MangaWorkflow.Web/Controllers/NotificationsController.cs:
- GET /Notifications/GetUnread — returns JSON List<NotificationDto>
- POST /Notifications/MarkRead/{id}
- POST /Notifications/MarkAllRead

### E. JavaScript Client

MangaWorkflow.Web/wwwroot/js/notifications.js:
- Connects to /hubs/notifications
- On ReceiveNotification: update badge count, prepend to dropdown, show toast
- Fetch initial unread count on page load via /Notifications/GetUnread

Update Views/Shared/_Layout.cshtml:
- Include signalr.min.js (CDN or local lib)
- Include notifications.js
- Add notification bell with badge in navbar
- Add toast container div
- Add notification dropdown ul with id="notification-list"

### F. Dashboard Service

Expand MangaWorkflow.Application/Interfaces/Services/IDashboardService.cs with:
- GetAdminDashboardAsync()
- GetMangakaDashboardAsync(Guid mangakaId)
- GetAssistantDashboardAsync(Guid assistantId)
- GetEditorDashboardAsync(Guid editorId)
- GetBoardDashboardAsync()

Expand MangaWorkflow.Application/Services/DashboardService.cs to implement all methods.
Query real data: user counts, series counts, chapter progress from vw_ChapterProgress, latest rankings from vw_SeriesLatestRanking.

Add new DTOs:
- MangaWorkflow.Application/DTOs/Dashboard/AdminDashboardDto.cs
- MangaWorkflow.Application/DTOs/Dashboard/MangakaDashboardDto.cs
- MangaWorkflow.Application/DTOs/Dashboard/AssistantDashboardDto.cs
- MangaWorkflow.Application/DTOs/Dashboard/EditorDashboardDto.cs
- MangaWorkflow.Application/DTOs/Dashboard/BoardDashboardDto.cs
- MangaWorkflow.Application/DTOs/Dashboard/ChapterProgressItemDto.cs
- MangaWorkflow.Application/DTOs/Dashboard/BackgroundJobSummary.cs

### G. Blazor Dashboard Components

Create in MangaWorkflow.Web/Components/Dashboard/:
- AdminDashboard.razor
- MangakaDashboard.razor
- AssistantDashboard.razor
- EditorDashboard.razor
- BoardDashboard.razor

Each component:
- Has @page "/Dashboard/{Role}" directive
- Injects IDashboardService
- Injects AuthenticationStateProvider to get current user ID
- Implements OnInitializedAsync to load data
- Shows stat cards (Bootstrap cards with counts)
- Shows relevant table (chapter progress, task list, ranking list, etc.)
- Has [Authorize(Roles = "...")] attribute

Embed Blazor components in an MVC Dashboard action or use Blazor routing directly.
Simplest approach: HomeController.Dashboard action returns a view that uses <component> tag helper:
```cshtml
@if (User.IsInRole("Mangaka"))
{
    <component type="typeof(MangakaDashboard)" render-mode="Server" />
}
```

### H. SignalR Events Wiring

These events MUST fire when the corresponding action happens:

| Action | Event Name | Call in |
|---|---|---|
| Task assigned to assistant | TaskAssigned | PageRegionService.CreateTaskFromRegionAsync |
| Assistant submits task | SubmissionUploaded | SubmissionService.SubmitTaskAsync |
| Mangaka reviews submission | SubmissionReviewed | SubmissionService.ReviewSubmissionAsync |
| Editor adds page comment | EditorCommentCreated | EditorCommentService.AddCommentAsync |
| Board member votes | BoardVoteSubmitted | BoardReviewService.SubmitVoteAsync |
| Board enters ranking | RankingUpdated | RankingService.CreateOrUpdateRankingAsync |

Each of these calls NotificationService.CreateAndSendAsync which pushes the event.

## Architecture Rules

- Blazor components inject Application services (not Infrastructure, not DbContext)
- Components must be async: OnInitializedAsync not OnInitialized
- SignalR hub context is injected via IHubContext<NotificationHub>
- NotificationService lives in Application layer but depends on IHubContext (this is OK for Web project DI)
  Alternative: Create an ISignalRNotifier interface in Application, implement in Web/Infrastructure

## After Implementing

Run:
```
dotnet clean && dotnet restore && dotnet build
dotnet run --project MangaWorkflow.Tools.DbSmokeTest
dotnet run --project MangaWorkflow.Web
```

Test real-time:
- Open two browser windows with different logins
- Submit a task in one window
- Confirm notification appears in the other window

Test Blazor dashboard:
- Navigate to /Dashboard
- Confirm counts show real database data

## When Done

Create docs/PHASE_4_COMPLETION_REPORT.md
Update docs/PROJECT_STATUS.md with [DONE] Phase 4
