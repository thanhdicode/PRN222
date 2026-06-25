# PHASE 4 — Blazor Dashboard + SignalR Realtime
# Detailed Implementation Guide

**PRN222 Chapters Proven**: Chapter 06 Blazor + Chapter 07 SignalR
**Depends on**: Phase 2 AND Phase 3 Complete
**Status**: LOCKED until Phase 3 DONE

---

## 1. What This Phase Proves in PRN222

### Chapter 06 — Blazor Server
- .razor component files with @page directive
- Component lifecycle: OnInitializedAsync, OnParametersSetAsync
- Data binding with @bind
- Event handling with @onclick, @onchange
- Inject Application services directly into Blazor components
- Component parameters and cascading parameters
- Conditional rendering with @if, @foreach

### Chapter 07 — SignalR
- Hub class inheriting from Hub
- MapHub<T> registration in Program.cs
- IHubContext<T> injection into services for server-push
- JavaScript SignalR client: HubConnectionBuilder
- SignalR events (method names) sent from server
- Client-side event handlers updating UI

---

## 2. What Must NOT Be Built in Phase 4

- NO Worker jobs (Phase 5)
- Do NOT rewrite MVC or Razor Pages in Blazor
- Do NOT use full SignalR user groups in initial implementation — start with Clients.All

---

## 3. Blazor Dashboard Components

### 3.1 Component Files

All dashboard components go in:
MangaWorkflow.Web/Components/Dashboard/

- AdminDashboard.razor
- MangakaDashboard.razor
- AssistantDashboard.razor
- EditorDashboard.razor
- BoardDashboard.razor

Each component is a routable Blazor page accessed at /Dashboard/Admin, /Dashboard/Mangaka, etc.

### 3.2 Dashboard Service

MangaWorkflow.Application/Interfaces/Services/IDashboardService.cs (expand existing):
```csharp
Task<AdminDashboardDto> GetAdminDashboardAsync(CancellationToken ct = default);
Task<MangakaDashboardDto> GetMangakaDashboardAsync(Guid mangakaId, CancellationToken ct = default);
Task<AssistantDashboardDto> GetAssistantDashboardAsync(Guid assistantId, CancellationToken ct = default);
Task<EditorDashboardDto> GetEditorDashboardAsync(Guid editorId, CancellationToken ct = default);
Task<BoardDashboardDto> GetBoardDashboardAsync(CancellationToken ct = default);
```

MangaWorkflow.Application/Services/DashboardService.cs (expand existing).

DTOs:

AdminDashboardDto.cs:
```csharp
public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalSeries { get; set; }
    public int TotalChapters { get; set; }
    public int TotalTasks { get; set; }
    public int OpenNotifications { get; set; }
    public List<BackgroundJobSummary> RecentJobs { get; set; } = new();
}
```

MangakaDashboardDto.cs:
```csharp
public class MangakaDashboardDto
{
    public int TotalSeries { get; set; }
    public int DraftSeries { get; set; }
    public int SubmittedSeries { get; set; }
    public int ApprovedSeries { get; set; }
    public List<ChapterProgressItemDto> ChapterProgress { get; set; } = new();
    public int UnreadNotifications { get; set; }
    public int PendingReviews { get; set; }
}
```

ChapterProgressItemDto.cs:
```csharp
public class ChapterProgressItemDto
{
    public string ChapterTitle { get; set; } = "";
    public string SeriesTitle { get; set; } = "";
    public int TotalTasks { get; set; }
    public int ApprovedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int ProgressPercent { get; set; }
    public DateTime? Deadline { get; set; }
}
```

AssistantDashboardDto.cs:
```csharp
public class AssistantDashboardDto
{
    public int AssignedTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int SubmittedTasks { get; set; }
    public int ApprovedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public int UnreadNotifications { get; set; }
    public List<TaskListItemDto> RecentTasks { get; set; } = new();
}
```

BoardDashboardDto.cs:
```csharp
public class BoardDashboardDto
{
    public int SubmittedForReview { get; set; }
    public int UnderReview { get; set; }
    public int ApprovedSeries { get; set; }
    public List<RankingListItemDto> LatestRankings { get; set; } = new();
    public int UnreadNotifications { get; set; }
}
```

### 3.3 Dashboard Component Template

Each dashboard component follows this pattern:

```razor
@page "/Dashboard/Mangaka"
@using MangaWorkflow.Application.Interfaces.Services
@using MangaWorkflow.Application.DTOs.Dashboard
@inject IDashboardService DashboardService
@inject AuthenticationStateProvider AuthStateProvider
@attribute [Authorize(Roles = "Mangaka,Admin")]

<h2>Mangaka Dashboard</h2>

@if (_loading)
{
    <p>Loading...</p>
}
else if (_data is not null)
{
    <div class="row">
        <div class="col-md-3">
            <div class="card text-center">
                <div class="card-body">
                    <h3>@_data.TotalSeries</h3>
                    <p>Total Series</p>
                </div>
            </div>
        </div>
        <!-- More stat cards -->
        <div class="col-12 mt-4">
            <h4>Chapter Progress</h4>
            <table class="table">
                <thead><tr><th>Chapter</th><th>Progress</th><th>Deadline</th></tr></thead>
                <tbody>
                @foreach (var ch in _data.ChapterProgress)
                {
                    <tr>
                        <td>@ch.SeriesTitle — @ch.ChapterTitle</td>
                        <td>
                            <div class="progress">
                                <div class="progress-bar" style="width: @ch.ProgressPercent%">@ch.ProgressPercent%</div>
                            </div>
                        </td>
                        <td>@ch.Deadline?.ToString("yyyy-MM-dd")</td>
                    </tr>
                }
                </tbody>
            </table>
        </div>
    </div>
}

@code {
    private MangakaDashboardDto? _data;
    private bool _loading = true;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var userId = Guid.Parse(authState.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        _data = await DashboardService.GetMangakaDashboardAsync(userId);
        _loading = false;
    }
}
```

### 3.4 Blazor App Setup in Program.cs

Program.cs must have (already added in Phase 1, verify):
```csharp
builder.Services.AddServerSideBlazor();
// ...
app.MapBlazorHub();
app.MapFallbackToPage("/_Host"); // or using Blazor component routing
```

For Blazor Server with MVC co-hosting, add _Host.cshtml:
- MangaWorkflow.Web/Pages/_Host.cshtml (Razor Page)

Or configure using Blazor component routing in Program.cs:
```csharp
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
```

For simplicity with MVC hosting, use individual @page Blazor components embedded in an MVC view:
- Create a HomeController Dashboard action that returns a View containing a Blazor component tag

Simplest approach:
```cshtml
<!-- In MVC View: Views/Home/Dashboard.cshtml -->
<component type="typeof(MangakaDashboard)" render-mode="Server" />
```

This renders the Blazor component inside an MVC view.

---

## 4. SignalR Implementation

### 4.1 Hub Files

MangaWorkflow.Web/Hubs/NotificationHub.cs:
```csharp
using Microsoft.AspNetCore.SignalR;

namespace MangaWorkflow.Web.Hubs
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // Phase 4: Add user to role group
            var roleCode = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (roleCode is not null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"role-{roleCode}");
            }
            await base.OnConnectedAsync();
        }
    }
}
```

MangaWorkflow.Web/Hubs/WorkflowHub.cs:
```csharp
namespace MangaWorkflow.Web.Hubs
{
    public class WorkflowHub : Hub
    {
        // Used for workflow event pushes (SubmissionUploaded, etc.)
    }
}
```

### 4.2 Program.cs Hub Registration

```csharp
// Already added in Phase 1:
builder.Services.AddSignalR();

// Add hub routes:
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapHub<WorkflowHub>("/hubs/workflow");
```

### 4.3 NotificationService SignalR Integration

Expand INotificationService.cs:
```csharp
Task CreateAndSendAsync(Guid recipientUserId, string typeCode, string title, string message, CancellationToken ct = default);
Task<List<NotificationDto>> GetUnreadAsync(Guid userId, int count = 5, CancellationToken ct = default);
Task MarkReadAsync(Guid notificationId, CancellationToken ct = default);
Task MarkAllReadAsync(Guid userId, CancellationToken ct = default);
```

Expand NotificationService.cs:
```csharp
// Inject IHubContext<NotificationHub>
public NotificationService(
    INotificationRepository repo,
    IHubContext<NotificationHub> hubContext)

// In CreateAndSendAsync:
// 1. Insert notification into DB
// 2. Push SignalR event:
await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
{
    title = title,
    message = message,
    typeCode = typeCode,
    timestamp = DateTime.UtcNow
}, ct);
```

When Phase 4 is stable, improve to user-specific groups:
```csharp
await _hubContext.Clients.Group($"user-{recipientUserId}").SendAsync("ReceiveNotification", payload, ct);
```

### 4.4 JavaScript Client

MangaWorkflow.Web/wwwroot/js/notifications.js:
```javascript
const notificationConnection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/notifications')
    .withAutomaticReconnect()
    .build();

notificationConnection.on('ReceiveNotification', function (payload) {
    // Update unread count badge
    const badge = document.getElementById('notification-badge');
    if (badge) {
        const current = parseInt(badge.textContent) || 0;
        badge.textContent = current + 1;
    }

    // Add to notification dropdown
    const list = document.getElementById('notification-list');
    if (list) {
        const item = document.createElement('li');
        item.className = 'dropdown-item';
        item.innerHTML = '<strong>' + payload.title + '</strong><br><small>' + payload.message + '</small>';
        list.prepend(item);
    }

    // Toast notification
    showToast(payload.title, payload.message);
});

notificationConnection.start().catch(function (err) {
    console.error('SignalR connection error:', err);
});

function showToast(title, message) {
    const toastContainer = document.getElementById('toast-container');
    if (!toastContainer) return;
    const toast = document.createElement('div');
    toast.className = 'toast show';
    toast.innerHTML = '<div class="toast-header"><strong>' + title + '</strong><button class="btn-close ms-auto" data-bs-dismiss="toast"></button></div><div class="toast-body">' + message + '</div>';
    toastContainer.appendChild(toast);
    setTimeout(() => toast.remove(), 5000);
}
```

### 4.5 Layout Integration

Add to Views/Shared/_Layout.cshtml:
1. Include SignalR client JS:
```html
<script src="~/lib/microsoft/signalr/dist/browser/signalr.min.js"></script>
<script src="~/js/notifications.js"></script>
```

Note: Install signalr client via libman or use CDN:
CDN: https://cdn.jsdelivr.net/npm/@microsoft/signalr@latest/dist/browser/signalr.min.js

2. Add notification dropdown in navbar:
```html
<!-- In navbar -->
<li class="nav-item dropdown">
    <a class="nav-link dropdown-toggle" href="#" id="notifDropdown" data-bs-toggle="dropdown">
        <i class="bi bi-bell"></i>
        <span id="notification-badge" class="badge bg-danger">0</span>
    </a>
    <ul class="dropdown-menu dropdown-menu-end" id="notification-list">
        <li class="dropdown-item text-muted">No notifications</li>
    </ul>
</li>
```

3. Add toast container:
```html
<!-- Near bottom of body -->
<div id="toast-container" class="toast-container position-fixed bottom-0 end-0 p-3" style="z-index: 1100;"></div>
```

### 4.6 Required SignalR Events

Events that MUST work by end of Phase 4:

| Event | Triggered by | Received by |
|---|---|---|
| TaskAssigned | Mangaka creates task from region | Assistant |
| SubmissionUploaded | Assistant submits task | Mangaka |
| SubmissionReviewed | Mangaka approves/rejects submission | Assistant |
| EditorCommentCreated | Editor adds page comment | Mangaka |
| BoardVoteSubmitted | Board member votes on series | All board members |
| RankingUpdated | Board enters new ranking | Mangaka, Editor |
| DeadlineWarning | Worker sends warning (Phase 5) | Assistant, Mangaka |

For Phase 4: these events must be pushed by the NotificationService when the relevant action occurs.

---

## 5. Notification Dropdown Persistence

Create a NotificationsController for AJAX/API calls:

MangaWorkflow.Web/Controllers/NotificationsController.cs:
```csharp
[Authorize]
public class NotificationsController : Controller
{
    // GET /Notifications/GetUnread
    public async Task<IActionResult> GetUnread(CancellationToken ct)
    // Returns JSON: List<NotificationDto>

    // POST /Notifications/MarkRead/{id}
    [HttpPost]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)

    // POST /Notifications/MarkAllRead
    [HttpPost]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
}
```

DTOs:
- DTOs/Notifications/NotificationDto.cs

NotificationDto.cs:
```csharp
public class NotificationDto
{
    public Guid NotificationId { get; set; }
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public string TypeCode { get; set; } = "";
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

## 6. Phase 4 Manual Test Flow

Test 1 — Real-time notification on task submission:
1. Open two browser windows/tabs
2. Window 1: Login as mangaka@manga.local
3. Window 2: Login as assistant@manga.local
4. Window 2: Navigate to /Assistant/TaskInbox
5. Window 2: Start a task and submit it
6. Window 1: Observe notification bell badge increment
7. Window 1: Open notification dropdown — see "Submission uploaded" notification

Test 2 — Blazor dashboard with real data:
1. Login as mangaka@manga.local
2. Navigate to /Dashboard/Mangaka (or /Home/Dashboard)
3. Verify stat cards show actual counts from DB
4. Verify chapter progress table shows real chapters and task counts

Test 3 — Editor comment notification:
1. Login as editor@manga.local
2. Add a comment on a manga page
3. In a second window logged in as mangaka, observe notification

Test 4 — Board vote notification:
1. Login as board@manga.local
2. Submit a vote on a series
3. Observe notification appears (may be to all for initial implementation)

---

## 7. Phase 4 Completion Criteria

[ ] dotnet build passes with zero errors
[ ] dotnet run DbSmokeTest passes
[ ] At least one Blazor dashboard renders with real database data
[ ] MangakaDashboard shows chapter progress from real DB
[ ] At least three SignalR events work (SubmissionUploaded, EditorCommentCreated, BoardVoteSubmitted)
[ ] Notification badge increments in real time
[ ] Notification dropdown shows recent notifications
[ ] Toast appears when notification received
[ ] Mark-read functionality works
[ ] docs/PROJECT_STATUS.md updated with [DONE] Phase 4
[ ] docs/PHASE_4_COMPLETION_REPORT.md created
