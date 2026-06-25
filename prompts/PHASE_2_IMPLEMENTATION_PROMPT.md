# Phase 2 Implementation Prompt
# Copy this entire prompt and paste it to your coding AI agent

---

You are a senior .NET developer implementing Phase 2 of the PRN222 MangaWorkflow project.

## Your Mission

Implement ONLY Phase 2: ASP.NET Core MVC CRUD Core System.
Do NOT implement Razor Pages, Blazor, SignalR UI, or Worker jobs.
Those belong to later phases and must not be started yet.

## Read These Files First (in order)

1. AGENTS.md
2. docs/AI_AGENT_EXECUTION_RULES.md
3. docs/PHASE_2_MVC_CRUD_PLAN.md
4. docs/03_DOMAIN_MODEL_DATABASE_GUIDE.md
5. docs/PROJECT_STATUS.md

Do not skip these. Do not assume you know the architecture without reading them.

## Current State (Phase 1 Complete)

The following already exist and must NOT be changed without good reason:
- MangaWorkflow.Domain/Entities/ — all EF Core entities (41 files)
- MangaWorkflow.Infrastructure/Persistence/MangaWorkflowDbContext.cs
- MangaWorkflow.Infrastructure/Repositories/: SeriesRepository, ChapterRepository, UserRepository, ProductionTaskRepository, NotificationRepository
- MangaWorkflow.Application/Interfaces/Repositories/: ISeriesRepository, IChapterRepository, IUserRepository, IProductionTaskRepository, INotificationRepository
- MangaWorkflow.Application/Interfaces/Services/: ISeriesService, IDashboardService, INotificationService
- MangaWorkflow.Application/Services/: SeriesService, DashboardService, NotificationService
- MangaWorkflow.Application/DependencyInjection.cs
- MangaWorkflow.Infrastructure/DependencyInjection.cs
- MangaWorkflow.Web/Program.cs (with AddApplicationServices and AddInfrastructure calls)

## What You Must Build

### Step 1: Cookie Authentication

Implement cookie authentication before any CRUD screen.
Files to create:
- MangaWorkflow.Application/Interfaces/Services/IAuthService.cs
- MangaWorkflow.Application/Services/AuthService.cs
- MangaWorkflow.Application/DTOs/Auth/LoginDto.cs
- MangaWorkflow.Web/Controllers/AccountController.cs
- MangaWorkflow.Web/Views/Account/Login.cshtml
- MangaWorkflow.Web/Views/Account/AccessDenied.cshtml

Update Program.cs to add:
- builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(...)
- app.UseAuthentication() BEFORE app.UseAuthorization()

### Step 2: Admin Area — User Management

Create areas setup files first, then:
- MangaWorkflow.Web/Areas/Admin/Controllers/UsersController.cs
- MangaWorkflow.Web/Areas/Admin/Controllers/DashboardController.cs
- MangaWorkflow.Web/Areas/Admin/Views/Users/Index.cshtml (with search/filter)
- MangaWorkflow.Web/Areas/Admin/Views/Users/Details.cshtml
- MangaWorkflow.Web/Areas/Admin/Views/Users/Create.cshtml
- MangaWorkflow.Web/Areas/Admin/Views/Users/Edit.cshtml
- MangaWorkflow.Web/Areas/Admin/Views/Users/AssignRole.cshtml
- MangaWorkflow.Application/Interfaces/Services/IUserService.cs
- MangaWorkflow.Application/Services/UserService.cs
- MangaWorkflow.Application/DTOs/Users/UserListItemDto.cs
- MangaWorkflow.Application/DTOs/Users/UserDetailDto.cs
- MangaWorkflow.Application/DTOs/Users/CreateUserDto.cs
- MangaWorkflow.Application/DTOs/Users/EditUserDto.cs
- MangaWorkflow.Application/DTOs/Users/AssignRoleDto.cs

The IUserRepository already exists — EXPAND it with GetAllAsync, GetByIdWithRolesAsync, etc.

Authorization: [Authorize(Roles = "Admin")] on UsersController

### Step 3: Mangaka Area — Series CRUD

- MangaWorkflow.Web/Areas/Mangaka/Controllers/SeriesController.cs
- MangaWorkflow.Web/Areas/Mangaka/Views/Series/ (Index, Details, Create, Edit, Delete)
- MangaWorkflow.Application/DTOs/Series/ (SeriesListItemDto, SeriesDetailDto, CreateSeriesDto, EditSeriesDto)

Expand existing ISeriesService and SeriesService with:
- GetSeriesForMangakaAsync (filter by logged-in user's ID, Admin sees all)
- SubmitSeriesAsync (status Draft -> Submitted)
- DeleteSeriesAsync (only if Draft)

Authorization: [Authorize(Roles = "Mangaka,Admin")]

### Step 4: Mangaka Area — Chapter CRUD

- MangaWorkflow.Web/Areas/Mangaka/Controllers/ChaptersController.cs
- MangaWorkflow.Web/Areas/Mangaka/Views/Chapters/ (Index, Details, Create, Edit)
- MangaWorkflow.Application/Interfaces/Services/IChapterService.cs
- MangaWorkflow.Application/Services/ChapterService.cs
- MangaWorkflow.Application/DTOs/Chapters/ (ChapterListItemDto, ChapterDetailDto, CreateChapterDto, EditChapterDto)
- Expand IChapterRepository and ChapterRepository

Validation: ChapterNumber > 0, Title required, Deadline must be future if provided.

### Step 5: Mangaka Area — Manga Page Management

- MangaWorkflow.Web/Areas/Mangaka/Controllers/PagesController.cs
- MangaWorkflow.Web/Areas/Mangaka/Views/Pages/ (Index, Details, Create)
- MangaWorkflow.Application/Interfaces/Services/IPageService.cs
- MangaWorkflow.Application/Services/PageService.cs
- MangaWorkflow.Application/Interfaces/Repositories/IPageRepository.cs
- MangaWorkflow.Infrastructure/Repositories/PageRepository.cs
- MangaWorkflow.Application/DTOs/Pages/ (PageListItemDto, PageDetailDto, CreatePageDto)

File uploads: .jpg, .jpeg, .png, .webp only. Save to wwwroot/uploads/pages/{guid}{ext}.

### Step 6: Board Area — Series Review

- MangaWorkflow.Web/Areas/Board/Controllers/SeriesReviewController.cs
- MangaWorkflow.Web/Areas/Board/Views/SeriesReview/ (Index, Details, Vote)
- MangaWorkflow.Application/Interfaces/Services/IBoardReviewService.cs
- MangaWorkflow.Application/Services/BoardReviewService.cs
- MangaWorkflow.Application/Interfaces/Repositories/IBoardVoteRepository.cs
- MangaWorkflow.Infrastructure/Repositories/BoardVoteRepository.cs
- MangaWorkflow.Application/DTOs/Board/ (BoardSeriesListItemDto, BoardSeriesDetailDto, SubmitVoteDto, VoteSummaryDto)

Business rules:
- Prevent duplicate vote (one board member one vote per series)
- After vote: INSERT notification record for Mangaka
- Vote options from VoteValues table

Authorization: [Authorize(Roles = "EditorialBoard,Admin")]

### Step 7: Board Area — Rankings

- MangaWorkflow.Web/Areas/Board/Controllers/RankingsController.cs
- MangaWorkflow.Web/Areas/Board/Views/Rankings/ (Index, Create)
- MangaWorkflow.Application/Interfaces/Services/IRankingService.cs
- MangaWorkflow.Application/Services/RankingService.cs
- MangaWorkflow.Application/Interfaces/Repositories/IRankingRepository.cs
- MangaWorkflow.Infrastructure/Repositories/RankingRepository.cs
- MangaWorkflow.Application/DTOs/Rankings/ (RankingListItemDto, CreateRankingDto)

After saving ranking: INSERT notification record with type RankingUpdated.

### Step 8: Update DependencyInjection.cs

Application/DependencyInjection.cs — add all new services.
Infrastructure/DependencyInjection.cs — add all new repositories.

### Step 9: Update Navigation Menu

Update Views/Shared/_Layout.cshtml with role-based navigation.

### Step 10: Update Area Routes in Program.cs

Add MapAreaControllerRoute calls for Admin, Mangaka, and Board areas.

## Architecture Rules You Must Follow

1. Controllers call Services only — never DbContext directly
2. Services call Repositories
3. Repositories use MangaWorkflowDbContext
4. All DB calls must be async (ToListAsync, FindAsync, SaveChangesAsync, etc.)
5. No .Result or .Wait() in request handlers
6. Register all new services and repositories in DependencyInjection.cs files
7. Add [Authorize(Roles = "...")] to every controller
8. Use Data Annotations for validation (Required, MaxLength, Range, etc.)
9. Status codes: query from DB by StatusCode string, never hardcode IDs
10. Domain project must not be modified for architecture changes

## After Implementing

Run:
```
dotnet clean
dotnet restore
dotnet build
dotnet run --project MangaWorkflow.Tools.DbSmokeTest
dotnet run --project MangaWorkflow.Web
```

Fix any build errors before proceeding.

Test manually:
- Login as admin@manga.local — verify User management works
- Login as mangaka@manga.local — create series, submit series
- Login as board@manga.local — vote on submitted series
- Verify notification record inserted in DB after board vote

## When Done

Create docs/PHASE_2_COMPLETION_REPORT.md with:
- List of all files created
- Confirmation all completion criteria met
- Any known limitations or TODOs

Update docs/PROJECT_STATUS.md with:
- [DONE] Phase 2 — ASP.NET Core MVC CRUD Core System
- Date completed
- Next step: Phase 3 Razor Pages
