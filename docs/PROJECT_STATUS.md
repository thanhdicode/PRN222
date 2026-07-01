# PRN222 MangaWorkflowSystem — Project Status

## Current Phase: ALL PHASES COMPLETE
**Current Phase:** AI Studio V1 Stabilization — COMPLETE
**Next Phase:** None

**Last Updated**: 2026-06-30
**Build Status**: ✅ PASSING (0 errors)
**Test Status**: ✅ 16/16 PASSING

---

## Phase Overview

| Phase | Name | Status | PRN222 Chapter |
|---|---|---|---|
| Phase 0 | Database Ready | ✅ DONE | — |
| Phase 1 | Foundation, Scaffold, DI | ✅ DONE | Ch01-Ch03 |
| Phase 2 | ASP.NET Core MVC CRUD | ✅ DONE | Ch04 |
| Phase 3 | Razor Pages Workflow | ✅ DONE | Ch05 |
| Phase 4 | Blazor + SignalR | ✅ DONE | Ch06, Ch07 |
| Phase 5 | Worker + Final Demo | ✅ DONE | Ch08 |

---

## ✅ Phase 0 — Database Ready (DONE)

- [x] Run MangaWorkflowDB_v2_demo_ready.sql
- [x] Run MangaWorkflowDB_v3_extra_seed_demo_data.sql
- [x] Verify tables and seed data

---

## ✅ Phase 1 — Foundation, Scaffold, DI (DONE)

Completed: 2026-06-25

### What was built
- [x] 6-project solution created: Domain, Application, Infrastructure, Web, Worker, Tools.DbSmokeTest
- [x] EF Core database-first scaffold — 41 entity files in MangaWorkflow.Domain/Entities/
- [x] MangaWorkflowDbContext in MangaWorkflow.Infrastructure/Persistence/
- [x] Repository interfaces: ISeriesRepository, IChapterRepository, IUserRepository, IProductionTaskRepository, INotificationRepository
- [x] Repository implementations: SeriesRepository, ChapterRepository, UserRepository, ProductionTaskRepository, NotificationRepository
- [x] Service interfaces: ISeriesService, IDashboardService, INotificationService
- [x] Service implementations: SeriesService, DashboardService, NotificationService
- [x] Application/DependencyInjection.cs
- [x] Infrastructure/DependencyInjection.cs
- [x] Web/Program.cs calls AddApplicationServices() and AddInfrastructure()
- [x] DbSmokeTest project verifies SQL Server connectivity
- [x] Build passes with zero errors
- [x] Architectural refactor: BusinessObjects->Domain, Services->Application, DataAccess/Repos->Infrastructure

### Existing basic DTO
- [x] DTOs/DashboardSummaryDto.cs

### Commands confirmed working
- dotnet build — passes
- dotnet run --project MangaWorkflow.Tools.DbSmokeTest — passes

---

## ✅ Phase 2 — ASP.NET Core MVC CRUD (DONE)

Status: DONE
PRN222: Chapter 04 ASP.NET Core MVC

### Target deliverables
- [x] Cookie authentication (Login/Logout)
- [x] Admin Area: User management CRUD
- [x] Mangaka Area: Series CRUD + submit flow
- [x] Mangaka Area: Chapter CRUD
- [x] Mangaka Area: Manga Page management + file upload
- [x] Board Area: Series Review + voting
- [x] Board Area: Ranking management
- [x] Role-based navigation menu
- [x] All Area routes registered
- [x] All new services/repos in DependencyInjection.cs

### Completion gate
Must pass ALL before marking done:
- dotnet build passes
- dotnet run DbSmokeTest passes
- All MVC screens open
- Full flow: Mangaka create series -> submit -> Board vote works
- docs/PHASE_2_COMPLETION_REPORT.md created

### Implementation guide
See docs/PHASE_2_MVC_CRUD_PLAN.md

### Agent prompt
See prompts/PHASE_2_IMPLEMENTATION_PROMPT.md

---

## ✅ Phase 3 — Razor Pages Workflow (DONE)

Status: DONE
PRN222: Chapter 05 Razor Pages

### Target deliverables
- [x] Pages/Assistant/TaskInbox
- [x] Pages/Assistant/TaskDetail
- [x] Pages/Assistant/SubmitTask (file upload)
- [x] Pages/Mangaka/ReviewSubmissions
- [x] Pages/Mangaka/ReviewSubmissionDetail (approve/reject/revision)
- [x] Pages/Mangaka/PageRegions (add regions, create tasks)
- [x] Pages/Editor/PageComments (add/resolve)

### Stabilization Fixes Applied (2026-06-26)
- [x] Fix 1: SubmissionStatus filter changed from wrong "PendingReview" → correct "Submitted"
- [x] Fix 2: Hardcoded SubmissionStatusId=1 replaced with DB lookup via ISubmissionStatusRepository
- [x] Fix 3: ProductionTaskRepository.UpdateStatusAsync — query TaskStatuses by StatusCode, set TaskStatusId FK
- [x] Fix 4: ReviewSubmissionAsync — query SubmissionStatuses by StatusCode, set SubmissionStatusId FK (no nav property mutation)
- [x] Fix 5: NotificationTypeId hardcoding replaced with INotificationTypeRepository lookup by TypeCode
- [x] Fix 6: Mangaka lookup — prefer Series.MangakaId; fallback to SeriesTeamMembers RoleInSeries.Contains("Mangaka")
- [x] Fix 7: PageComments authorization — removed invalid "Editor" role, now uses TantouEditor,Admin,EditorialBoard
- [x] Fix 8: Removed unused IPageRepository injection from PageRegionsModel

### Completion gate (PASSES)
- ✅ dotnet build — 0 errors
- ✅ dotnet run DbSmokeTest — passes
- ✅ Manual workflow verification passed

---

## ✅ Phase 4 — Blazor + SignalR (DONE)

Status: DONE
PRN222: Chapter 06 Blazor + Chapter 07 SignalR

### Target deliverables
- [x] NotificationHub, WorkflowHub
- [x] Real-time notification dropdown + toast
- [x] Role-based Blazor dashboards with real data
- [x] At least 3 SignalR event types wired

---

## ✅ Phase 5 — Worker + Final Demo (DONE)

Status: DONE
PRN222: Chapter 08 Worker Service

### Target deliverables
- [x] DeadlineReminderWorker
- [x] OverdueTaskScannerWorker
- [x] RankingRiskWorker
- [x] NotificationCleanupWorker
- [x] MonthlyEarningCalculatorWorker
- [x] Admin manual trigger for worker demo
- [x] docs/FINAL_DEMO_SCRIPT.md
- [x] docs/FINAL_QA_CHECKLIST.md
- [x] docs/FINAL_REPORT_OUTLINE.md

---

## Known Issues / Demo Limitations

- **.NET Versioning**: The solution targets `.NET 10.0` (Preview) and requires the corresponding `.NET 10.0` SDK/Runtime to be installed on the demonstration machine.
- **Authentication**: Authentication uses demo-mode plaintext password comparison for simplicity during the demonstration and is NOT suitable for production security.

---

## Run Commands

```
dotnet clean
dotnet restore
dotnet build
dotnet run --project MangaWorkflow.Tools.DbSmokeTest
dotnet run --project MangaWorkflow.Web
dotnet run --project MangaWorkflow.Worker
```

## Next Recommended Action

Demonstrate the project to the instructor using the `FINAL_DEMO_SCRIPT.md` checklist.

---

## ✅ AI Studio V1 Stabilization (DONE)

Status: DONE
Branch: feature/ai-studio-fullstack
Completed: 2026-06-30

This stabilization work completed the human-in-the-loop AI workflow
(detect → review → accept → suggest → approve → execute) and fixed
architectural issues. See `docs/specs/AI_STUDIO_V1_STABILIZATION_SPEC.md`.

### Tasks completed
- [x] Task 1: Pass Real Page Image Into AI
- [x] Task 2: Move AiStudioService to Application Layer (removed direct DbContext)
- [x] Task 3: Add Accept/Reject Detected Region Flow (creates PageRegion, SourceType="AI")
- [x] Task 4: Fix Task Suggestion Logic (region-type → TaskType mapping with "Other" fallback)
- [x] Task 5: Add Approve/Reject Task Suggestion Flow (creates ProductionTask)
- [x] Task 6: Fix AI DB Script Integrity (11 FK + 4 check constraints, IF NOT EXISTS guards)
- [x] Task 7: Fix AI Configuration (AiVisionOptions, FallbackToMock)
- [x] Task 8: Improve AI Studio UI Flow (region cards, color badges, confidence bars, approve modals)
- [x] Task 9: Update Training Docs for Honesty (mock-mode disclaimers, placeholder scripts)
- [x] Task 10: Add Verification and Tests (AiStudioServiceTests — 15 tests passing)

### Bug fixes applied (2026-06-30)
- [x] Build error: Razor `@page` directive collision — renamed loop variable `page`→`item`
  and MVC action `PageAnalysis`→`Analyze` in `Index.cshtml` / `AiStudioController.cs`
- [x] Ambiguous `TaskStatus` reference resolved in `AiStudioServiceTests.cs`

### Verification (2026-06-30)
- ✅ `dotnet build` — 0 errors, 4 warnings (NuGet vulnerability advisories only)
- ✅ `dotnet test` — 16/16 passed (1 MockAiVisionClient + 15 AiStudioService)
- ✅ All 10 AI Studio tasks complete and merge-ready

---

## SET 1 Local Stabilization (DONE)

Status: DONE
Completed: 2026-07-01

### Tasks completed
- [x] Added and committed `docs/specs/2026-07-01-set1-runnable-testable-design.md` before implementation.
- [x] Fixed build error in `AiStudioController` by using existing `ISeriesRepository.GetByIdWithDetailsAsync`.
- [x] Added Assistant MVC views for Dashboard, Tasks Index/Detail, and Submission upload.
- [x] Added Assistant `_ViewImports.cshtml`, functional `_ViewStart.cshtml`, and checklist `_ViewStart.cshtml` under Shared.
- [x] Verified `SubmitTaskDto` and `ITaskWorkflowService` methods used by Assistant controllers already exist.
- [x] Added file-storage handling to Assistant MVC submission upload before calling `ISubmissionService`.
- [x] Added idempotent `Database/seed_notification_types_v2.sql`.
- [x] Verified Area controller authorization attributes are present.

### Tests added
- [x] `AiStudioControllerTests.Analyze_AllowsMangakaOwnedPageUsingExistingSeriesLookup`
- [x] `AssistantSubmissionsControllerTests.Submit_SavesUploadedFileBeforeCallingSubmissionService`

### Commands run
- `git pull`
- `dotnet test MangaWorkflow.Tests\MangaWorkflow.Tests.csproj --filter Analyze_AllowsMangakaOwnedPageUsingExistingSeriesLookup`
- `dotnet test MangaWorkflow.Tests\MangaWorkflow.Tests.csproj --filter Submit_SavesUploadedFileBeforeCallingSubmissionService`
- `dotnet test`
- `dotnet build`
- `dotnet run --project MangaWorkflow.Tools.DbSmokeTest`
- `dotnet run --project MangaWorkflow.Web --urls http://localhost:5128`
- `curl.exe -i http://localhost:5128/Auth/Login`
- `curl.exe -i --max-redirs 0 http://localhost:5128/Assistant/Tasks`
- `curl.exe -i --max-redirs 0 http://localhost:5128/Assistant/Dashboard`
- `sqlcmd -S . -d MangaWorkflowDB -E -Q "<AuditLogs LoginFailed verification query>"`

### Verification
- [x] `dotnet build` passes with 0 errors.
- [x] `dotnet test` passes: 18/18.
- [x] `DbSmokeTest` passes against local SQL Server.
- [x] `/Auth/Login` returns 200 OK.
- [x] `/Assistant/Tasks` and `/Assistant/Dashboard` redirect unauthenticated users to `/Auth/Login`, confirming routes exist and authorization is active.
- [x] Wrong login POST creates a `LoginFailed` row in `AuditLogs`.

### Known issues
- NuGet vulnerability warnings remain for `NuGet.Packaging` and `NuGet.Protocol`; they existed before this stabilization and do not block compilation.
