# Final QA Checklist

## Phase 1 & 2: Architecture & CRUD
- [x] Solution uses Clean Architecture (Domain, Application, Infrastructure, Web).
- [x] Entity Framework Core uses SQL Server with `TrustServerCertificate=True`.
- [x] Identity Framework properly seeds roles (Admin, Mangaka, Editor, Assistant, EditorialBoard).
- [x] CRUD operations for Series are functioning via ASP.NET Core MVC.
- [x] Admin User Management can view and edit roles.

## Phase 3: Workflow & Razor Pages
- [x] Razor Pages implemented for Editor/Comments, Mangaka/PageRegions, Assistant/Inbox.
- [x] Assistants can submit region tasks.
- [x] Mangaka can approve or reject submissions.
- [x] Status history is tracked accurately using `WorkflowStatusHistory`.
- [x] Validation attributes and Anti-Forgery tokens protect endpoints.

## Phase 4: Blazor & SignalR
- [x] Dashboard components use Blazor Server (`_Host.cshtml` + `.razor` components).
- [x] SignalR `NotificationHub` correctly routes messages to specific users or roles.
- [x] Real-time Toast UI properly displays messages without page reloads.
- [x] `INotificationService` abstractions keep Application layer clean from `IHubContext` dependencies.

## Phase 5: Worker Service
- [x] 5 Workers implemented via `PeriodicTimer`.
- [x] HostedService logic delegates actual DB operations to `IBackgroundJobService`.
- [x] Job Runs are logged correctly into the `BackgroundJobLogs` table.
- [x] Admin UI exists to manually trigger workers and responds securely (POST + Authorize + ValidateAntiForgeryToken).
- [x] Multi-project execution handles both `Web` and `Worker` running concurrently.
