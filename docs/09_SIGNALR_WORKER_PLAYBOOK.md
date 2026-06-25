# 09 — SignalR and Worker Service Playbook

## SignalR purpose

SignalR proves PRN222 real-time communication. Use it to notify users when workflow events happen.

## Hubs

### NotificationHub

Location:

```text
MangaWorkflow.Web/Hubs/NotificationHub.cs
```

Basic hub:

```csharp
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }
}
```

Later add groups:

```csharp
await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
await Groups.AddToGroupAsync(Context.ConnectionId, $"role-{roleCode}");
```

## SignalR event names

Use exact event names:

```text
ReceiveNotification
TaskAssigned
SubmissionUploaded
SubmissionReviewed
EditorCommentCreated
BoardVoteSubmitted
RankingUpdated
DeadlineWarning
DashboardRefreshRequested
```

## Minimal client script

Place in `_Layout.cshtml` or a partial view:

```html
<script src="/lib/microsoft/signalr/dist/browser/signalr.js"></script>
<script>
const connection = new signalR.HubConnectionBuilder()
    .withUrl('/hubs/notifications')
    .withAutomaticReconnect()
    .build();

connection.on('ReceiveNotification', function (payload) {
    console.log('Notification:', payload);
    const list = document.getElementById('notification-list');
    if (list) {
        const item = document.createElement('li');
        item.textContent = payload.title + ': ' + payload.message;
        list.prepend(item);
    }
});

connection.start().catch(err => console.error(err));
</script>
```

## Server send rule

Notification service should both:

1. Insert into `Notifications` table.
2. Push SignalR event.

Prototype:

```csharp
await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
{
    title = request.Title,
    message = request.Message,
    type = request.TypeCode
}, cancellationToken);
```

Use `Clients.All` first. User-specific groups can be added after login is stable.

## Worker Service purpose

Worker Service proves PRN222 background tasks. It must not be fake. It should query real database data and write results to `BackgroundJobLogs`.

## Required jobs

### DeadlineReminderWorker

Logic:

- Find tasks with status Assigned/InProgress/RevisionRequired.
- Deadline within 48 hours.
- Create DeadlineWarning notification for assigned assistant and/or mangaka.
- Log result.

### OverdueTaskScannerWorker

Logic:

- Find tasks where Deadline < now and status is not Approved/Rejected/Cancelled/Overdue.
- Set status to Overdue.
- Create notification.
- Add workflow history.
- Log result.

### RankingRiskScannerWorker

Logic:

- Find latest ranking records.
- If trend Down or rank worse than threshold, update series risk score.
- Create warning notification.
- Log result.

### NotificationCleanupWorker

Logic:

- Mark or delete old read notifications.
- If none, log Skipped.

## Worker implementation skeleton

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
            var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();
            await taskService.MarkOverdueTasksAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

## Worker demo shortcut

For presentation, expose an Admin button:

```text
/Admin/BackgroundJobs/RunOverdueScanner
```

This button manually calls the same service used by the worker. That makes demo deterministic.
