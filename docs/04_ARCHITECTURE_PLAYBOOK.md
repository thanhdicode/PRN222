# 04 — Architecture Playbook

## Architecture principle

Use a student-friendly layered architecture aligned with PRN222 labs and assignments. Do not use enterprise patterns that make the project harder to complete.

## Preferred solution structure

```text
MangaWorkflowSystem/
├── MangaWorkflow.BusinessObjects/
│   ├── Models/                 # EF Core scaffolded entities
│   ├── DTOs/                   # data transfer objects
│   ├── ViewModels/             # UI-specific models
│   └── Constants/              # status codes, role names
│
├── MangaWorkflow.DataAccessObjects/
│   ├── Data/                   # MangaWorkflowDbContext
│   └── DAOs/                   # low-level data access classes if required
│
├── MangaWorkflow.Repositories/
│   ├── Interfaces/
│   └── Implementations/
│
├── MangaWorkflow.Services/
│   ├── Interfaces/
│   ├── Implementations/
│   └── Clients/                # Mock AI segmentation client
│
├── MangaWorkflow.Web/
│   ├── Controllers/            # MVC
│   ├── Pages/                  # Razor Pages
│   ├── Views/                  # MVC views
│   ├── Components/             # Blazor components
│   ├── Hubs/                   # SignalR hubs
│   ├── wwwroot/                # CSS, JS, uploads
│   └── Program.cs
│
└── MangaWorkflow.Worker/
    ├── Workers/
    └── Program.cs
```

## Dependency direction

Dependencies must point inward/downward:

```text
Web → Services → Repositories → DataAccessObjects → BusinessObjects
Worker → Services → Repositories → DataAccessObjects → BusinessObjects
```

BusinessObjects should not depend on Web, Services, Repositories, or DataAccessObjects.

## Minimum acceptable structure if time is limited

```text
MangaWorkflow.Web/
├── Models/
├── Data/
├── Repositories/
├── Services/
├── Controllers/
├── Pages/
├── Components/
└── Hubs/

MangaWorkflow.Worker/
```

This is acceptable if the team is short on time, but still keep clear folders.

## Layer responsibilities

### BusinessObjects

Contains:

- EF Core entities.
- DTOs.
- ViewModels.
- Role constants.
- Status constants.

Does not contain:

- EF queries.
- Controller logic.
- SignalR calls.
- Worker loops.

### DataAccessObjects

Contains:

- `MangaWorkflowDbContext`.
- DAO classes if following lab format.
- Simple data access helpers.

Does not contain:

- Business workflow decisions.
- Notification sending.
- UI formatting.

### Repositories

Contains data query/update methods.

Example:

```csharp
Task<List<Series>> GetByMangakaAsync(Guid mangakaId, CancellationToken cancellationToken);
Task<Series?> GetDetailsAsync(Guid seriesId, CancellationToken cancellationToken);
Task AddAsync(Series series, CancellationToken cancellationToken);
Task SaveChangesAsync(CancellationToken cancellationToken);
```

### Services

Contains business rules.

Example:

```csharp
Task SubmitSeriesAsync(Guid seriesId, Guid mangakaId, CancellationToken cancellationToken);
Task AssignTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken);
Task ReviewSubmissionAsync(ReviewSubmissionRequest request, CancellationToken cancellationToken);
```

Services may call:

- Repositories.
- Notification service.
- SignalR hub context.
- Mock AI client.

### Web

Contains presentation layer:

- MVC controllers.
- Razor Pages.
- Blazor dashboard.
- SignalR hub endpoints.

Controllers/PageModels should call Services, not DbContext directly except for quick prototypes.

### Worker

Contains background job execution.

A worker must:

- Create DI scope.
- Call services.
- Use `CancellationToken`.
- Log to `BackgroundJobLogs`.
- Not directly manipulate UI.

## Authentication approach

### Simple PRN222 demo approach

Use custom login:

- Read `Users` by email.
- For demo, password can be a known value until real hashing is added.
- Store user ID and role claims in cookie.

### Better approach if time allows

Use ASP.NET Core Identity and map it to existing domain users. This is optional and not necessary for PRN222 core demonstration.

## MVC modules

Use MVC for:

- Series management.
- Chapter management.
- Manga page management.
- Board voting.
- Ranking management.
- Admin user/log views.

## Razor Pages modules

Use Razor Pages for assistant-focused flows:

- Task inbox.
- Task detail.
- Submission upload.
- Earnings list.

## Blazor modules

Use Blazor for dashboard only, unless time remains.

Do not rewrite the whole app in Blazor. The course needs MVC and Razor Pages too.

## SignalR design

Hubs:

- `NotificationHub`
- optional `WorkflowHub`

Groups:

- `user-{UserId}`
- `role-Mangaka`
- `role-Assistant`
- `series-{SeriesId}`

Start simple: send to all connected clients or role group first; improve later if user groups are ready.

## Worker design

Worker project can run separately:

```bash
dotnet run --project MangaWorkflow.Worker
```

Worker can also be implemented as Hosted Services inside Web if deployment simplicity matters:

```csharp
builder.Services.AddHostedService<DeadlineReminderWorker>();
```

Separate Worker is more impressive, but Web-hosted workers are easier for demo.
