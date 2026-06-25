# Task Prompts by Module

## Scaffold EF Core

```text
Implement EF Core database-first scaffolding for MangaWorkflowDB. Use the existing SQL Server database. Keep entities and DbContext organized according to the current solution structure. Do not modify SQL schema. After scaffolding, add connection string and register DbContext in Program.cs. Build the solution.
```

## Series MVC

```text
Implement Series MVC module for MangaWorkflow. Use Series, SeriesStatuses, PublicationSchedules, Users tables. Add SeriesController with Index, Details, Create, Edit, Submit. Use ISeriesService and async EF Core. Do not implement Board voting yet. Keep UI Bootstrap-simple. Update PROJECT_STATUS.md.
```

## Assistant Razor Pages

```text
Implement Assistant task inbox using Razor Pages. Use ProductionTasks, TaskStatuses, MangaPages, PageRegions, TaskSubmissions. Create /Pages/Assistant/Tasks/Index, Details, Submit. Use ITaskService and ISubmissionService. This must demonstrate PRN222 Razor Pages. Do not rewrite MVC modules.
```

## SignalR notifications

```text
Implement NotificationHub and notification dropdown. When a task is assigned or submission is reviewed, create a Notifications row and send SignalR ReceiveNotification event. Start with Clients.All for demo, then leave TODO for user groups. Do not overbuild authentication-specific groups unless auth is stable.
```

## Worker Service

```text
Implement OverdueTaskScannerWorker. It should scan ProductionTasks with past deadlines and active statuses, mark them Overdue, create notifications, write BackgroundJobLogs, and use DI scope correctly. Use async/await and CancellationToken. Provide a manual Admin trigger if useful for demo.
```

## Blazor dashboard

```text
Implement a Blazor dashboard page/component using vw_ChapterProgress, vw_SeriesLatestRanking, Notifications, and BackgroundJobLogs. Show cards and tables. This is for PRN222 Blazor evidence only; do not convert the whole app to Blazor.
```
