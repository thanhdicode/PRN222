# SET 1 Runnable and Testable Design

Date: 2026-07-01

## Goal

Make the latest SET 1 code compile and make the Assistant area and audit/notification checklist runnable from a local pull. The work is a stabilization pass, not a redesign.

## Scope

1. Fix all current compile errors.
2. Add only missing Assistant MVC views and view imports/start files.
3. Verify DTOs and service methods used by Assistant controllers.
4. Verify area authorization attributes and add missing attributes only where needed.
5. Add an idempotent SQL seed script for the new notification type codes.
6. Run build/tests and smoke checks where the local environment allows.

## Design

### Build Fix

The current build failure is caused by `AiStudioController` calling a repository method that does not exist on `ISeriesRepository`. The fix should preserve the existing repository pattern and use an existing method where possible before adding API surface.

### Assistant Views

The new Assistant controllers already exist, so each action that returns a view must have a matching `.cshtml`. Views will be minimal Bootstrap/Razor screens using the DTO properties already exposed by `TaskWorkflowService`. They should not duplicate business logic.

Expected views:

- `Areas/Assistant/Views/Shared/_Layout.cshtml`
- `Areas/Assistant/Views/_ViewStart.cshtml`
- `Areas/Assistant/Views/_ViewImports.cshtml`
- `Areas/Assistant/Views/Dashboard/Index.cshtml`
- `Areas/Assistant/Views/Tasks/Index.cshtml`
- `Areas/Assistant/Views/Tasks/Detail.cshtml`
- `Areas/Assistant/Views/Submissions/Submit.cshtml`

### DTO and Service Compatibility

`SubmitTaskDto` must expose `TaskId`, optional `Notes`, and an uploaded file property compatible with MVC model binding. `ITaskWorkflowService` must expose the methods currently consumed by Assistant controllers. If a method is missing, add the narrowest interface and implementation change needed.

### Authorization

Area controllers should use role-based authorization consistent with the project rules:

- Admin: `Admin`
- Mangaka: `Mangaka,Admin` where admin access is already established by local pattern
- Board: `EditorialBoard,Admin` where admin access is already established by local pattern
- Assistant: `Assistant`

No advanced policy system will be introduced.

### Seed Script

Add an idempotent script at `Database/seed_notification_types_v2.sql` for:

- `UserLogin`
- `UserLogout`
- `SeriesSubmitted`
- `SubmissionUploaded`
- `SubmissionReviewed`
- `System`

The script will use `INSERT ... SELECT ... WHERE NOT EXISTS` and will not hardcode entity GUIDs.

## Testing Plan

Follow TDD where behavior changes require production code:

1. Add or adjust a focused test that fails for the current bug.
2. Run the targeted test and confirm the expected failure.
3. Implement the minimal fix.
4. Re-run targeted tests, then `dotnet build`, then broader tests.

For view-only additions, compile-time Razor validation and route smoke checks are the main verification. Final verification will include:

- `dotnet build`
- `dotnet test`
- `dotnet run --project MangaWorkflow.Tools.DbSmokeTest` if SQL Server is reachable
- HTTP smoke checks for `/Auth/Login` and Assistant routes if the app can start locally

## Assumptions

- Existing pushed audit log services and repositories are considered source of truth and will not be rewritten.
- Existing views are left intact unless a compile/runtime issue requires a targeted edit.
- Local database availability may limit DB smoke and audit-row verification; if so, the limitation will be reported clearly.
