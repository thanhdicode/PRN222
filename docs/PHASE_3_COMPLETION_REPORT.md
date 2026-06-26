# Phase 3 Completion Report

## Overview
Phase 3 of the MangaWorkflow project has been completed. This phase focused on building the Razor Pages Workflow Screens for the different roles in the system (Assistant, Mangaka, Editor).

## Tasks Completed
1. **Assistant Workflow**
   - Created `MangaWorkflow.Web/Pages/Assistant/TaskInbox.cshtml` & `TaskInbox.cshtml.cs`.
   - Authorized for `Assistant` role.
   - Shows tasks assigned to the assistant, filtered by status.
   - Allows submitting task results, updating the status to `ReviewPending`.

2. **Mangaka Workflow**
   - Created `MangaWorkflow.Web/Pages/Mangaka/SubmissionReview.cshtml` & `SubmissionReview.cshtml.cs`.
   - Authorized for `Mangaka` role.
   - Shows tasks that are `ReviewPending` for the Mangaka's series.
   - Allows the Mangaka to Accept or Reject submissions, adding comments.
   - Status flows: `Approved` or `RevisionsRequested`.

3. **Editor Workflow**
   - Created `MangaWorkflow.Web/Pages/Editor/PageComments.cshtml` & `PageComments.cshtml.cs`.
   - Authorized for `Editor` role.
   - Allows fetching a specific page and displaying its `PageRegions`.
   - Allows creating new `PageRegions` and assigning `EditorComments` to them.

4. **Service Layer Updates**
   - Implemented `SubmissionService` for handling task submissions.
   - Implemented `TaskWorkflowService` for Mangaka review actions.
   - Implemented `PageRegionService` and `EditorCommentService` for Editor interactions.

5. **Infrastructure Updates**
   - Updated Repositories to correctly map entity relationships (e.g., using `SeriesTeamMembers`).

6. **Authentication & Authorization**
   - Verified Cookie Authentication.
   - Applied `[Authorize(Roles = "...")]` to the Razor Pages.
   - Updated `_Layout.cshtml` to conditionally show navigation links based on user roles.

7. **Build and Verification**
   - Resolved all compilation errors (`CS1061`) related to entity property naming mismatches (using `TaskId`, `SubmissionId`, etc. instead of `Id`).
   - `dotnet build` succeeds.
   - `MangaWorkflow.Tools.DbSmokeTest` completes successfully.

## Next Steps
Proceed to **Phase 4: Blazor + SignalR** after manual workflow verification passes.

---

## Phase 3 Stabilization Fixes (2026-06-26)

After initial scaffolding, a stabilization pass was applied to correct runtime correctness issues discovered by reviewing the DB schema against the implementation.

### Fix 1 — Submission Status Filter
**File**: `TaskSubmissionRepository.GetPendingSubmissionsForMangakaAsync`
- **Before**: Filtered on `StatusCode == "PendingReview"` — this code does not exist in DB
- **After**: Filters on `StatusCode == "Submitted"` — the actual DB seed value

### Fix 2 — Hardcoded SubmissionStatusId
**File**: `SubmissionService.SubmitTaskAsync`
- **Before**: `SubmissionStatusId = 1` — hardcoded INT, breaks if DB IDENTITY sequence differs
- **After**: Resolved via `ISubmissionStatusRepository.GetIdByCodeAsync("Submitted")`

### Fix 3 — ProductionTask Status Update
**File**: `ProductionTaskRepository.UpdateStatusAsync`
- **Before**: `task.TaskStatus.StatusCode = statusCode` — mutates tracked navigation property, causes EF tracking conflict
- **After**: Queries `TaskStatuses` by `StatusCode`, sets `task.TaskStatusId` FK directly

### Fix 4 — Submission Review Status Update
**File**: `SubmissionService.ReviewSubmissionAsync`
- **Before**: `submission.SubmissionStatus.StatusCode = "Approved"` — same EF tracking conflict
- **After**: Resolves `SubmissionStatusId` via `ISubmissionStatusRepository`, sets FK directly

### Fix 5 — Notification Type Hardcoding
**File**: `SubmissionService`
- **Before**: `NotificationTypeId = 1` — hardcoded INT
- **After**: Resolved via `INotificationTypeRepository.GetIdByCodeAsync("SubmissionUploaded")` and `"SubmissionReviewed"`

### Fix 6 — Mangaka Lookup
**File**: `SubmissionService.SubmitTaskAsync` and `TaskSubmissionRepository`
- **Before**: Only matched via `SeriesTeamMembers` with `RoleInSeries == "Mangaka"` (exact equality)
- **After**: Prefers `Series.MangakaId`; falls back to `RoleInSeries.Contains("Mangaka")` for flexibility

### Fix 7 — PageComments Authorization
**File**: `Pages/Editor/PageComments.cshtml.cs`
- **Before**: `[Authorize(Roles = "Editor,Admin,EditorialBoard,TantouEditor")]` — `"Editor"` is not a real DB RoleCode
- **After**: `[Authorize(Roles = "TantouEditor,Admin,EditorialBoard")]` — matches actual DB roles

### Fix 8 — Unused IPageRepository Injection
**File**: `Pages/Mangaka/PageRegions.cshtml.cs`
- **Before**: Constructor injected `IPageRepository _pageRepo` but never used it
- **After**: Removed — simplifies constructor, no DI overhead

### New Infrastructure Added
- `ISubmissionStatusRepository` / `SubmissionStatusRepository` — DB lookup for submission status IDs
- `INotificationTypeRepository` / `NotificationTypeRepository` — DB lookup for notification type IDs
- Both registered in `Infrastructure/DependencyInjection.cs`

### Build Result After Stabilization
```
Build succeeded.  0 Error(s)
dotnet run MangaWorkflow.Tools.DbSmokeTest  → [SUCCESS]
```

