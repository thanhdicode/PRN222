# Phase 3 Implementation Prompt
# Copy this entire prompt and paste it to your coding AI agent

---

You are a senior .NET developer implementing Phase 3 of the PRN222 MangaWorkflow project.

## Prerequisite

Phase 2 must be COMPLETE before you start. Check docs/PROJECT_STATUS.md.
If Phase 2 is not marked [DONE], stop and do not proceed.

## Your Mission

Implement ONLY Phase 3: Razor Pages Workflow Screens.
Do NOT implement Blazor components, SignalR push events, or Worker jobs.
Those belong to Phases 4 and 5.

## Read These Files First

1. AGENTS.md
2. docs/AI_AGENT_EXECUTION_RULES.md
3. docs/PHASE_3_RAZOR_PAGES_WORKFLOW_PLAN.md
4. docs/03_DOMAIN_MODEL_DATABASE_GUIDE.md
5. docs/PROJECT_STATUS.md

## What You Must Build

### Razor Pages (all go in MangaWorkflow.Web/Pages/ folder)

#### Assistant Workflow
- Pages/Assistant/TaskInbox.cshtml + TaskInbox.cshtml.cs
  Authorization: [Authorize(Roles = "Assistant")]
  OnGetAsync: loads tasks for logged-in assistant, filtered by StatusFilter
  Shows: Table of tasks with status badge, Overdue highlight

- Pages/Assistant/TaskDetail.cshtml + TaskDetail.cshtml.cs
  OnGetAsync(Guid taskId): loads task detail
  OnPostStartAsync: changes status Assigned -> InProgress
  Links to SubmitTask page

- Pages/Assistant/SubmitTask.cshtml + SubmitTask.cshtml.cs
  OnGetAsync(Guid taskId): loads task
  OnPostAsync: validates file, saves to wwwroot/uploads/submissions/{guid}{ext}
  Creates TaskSubmission record, updates task to Submitted
  Creates Notification record for Mangaka
  Uses TempData["Success"] and redirects

#### Mangaka Submission Review
- Pages/Mangaka/ReviewSubmissions.cshtml + ReviewSubmissions.cshtml.cs
  Authorization: [Authorize(Roles = "Mangaka,Admin")]
  OnGetAsync: loads pending submissions for logged-in Mangaka

- Pages/Mangaka/ReviewSubmissionDetail.cshtml + ReviewSubmissionDetail.cshtml.cs
  OnGetAsync: loads submission detail with file link
  OnPostApproveAsync(Guid submissionId): Decision = Approved, notify assistant
  OnPostRejectAsync(Guid submissionId, string reason): Decision = Rejected (reason required)
  OnPostRevisionAsync(Guid submissionId, string reason): Decision = RevisionRequired, task -> RevisionRequired

#### Page Regions
- Pages/Mangaka/PageRegions.cshtml + PageRegions.cshtml.cs
  Authorization: [Authorize(Roles = "Mangaka,Admin")]
  OnGetAsync(Guid pageId): loads page + regions + region type options + assistant options
  OnPostAddRegionAsync(): [BindProperty] CreateRegionDto, validates X/Y/W/H
  OnPostCreateTaskAsync(): [BindProperty] CreateTaskFromRegionDto, assigns to assistant, creates notification

#### Editor Comments
- Pages/Editor/PageComments.cshtml + PageComments.cshtml.cs
  Authorization: [Authorize(Roles = "Editor,Admin")]
  OnGetAsync(Guid pageId, string? statusFilter): loads page + comments
  OnPostAddCommentAsync(): [BindProperty] AddCommentDto, creates comment, creates Notification for Mangaka
  OnPostResolveCommentAsync(Guid commentId): marks comment resolved

### Application Layer New Files

Services:
- MangaWorkflow.Application/Interfaces/Services/ITaskWorkflowService.cs
- MangaWorkflow.Application/Services/TaskWorkflowService.cs
- MangaWorkflow.Application/Interfaces/Services/ISubmissionService.cs
- MangaWorkflow.Application/Services/SubmissionService.cs
- MangaWorkflow.Application/Interfaces/Services/IEditorCommentService.cs
- MangaWorkflow.Application/Services/EditorCommentService.cs
- MangaWorkflow.Application/Interfaces/Services/IPageRegionService.cs
- MangaWorkflow.Application/Services/PageRegionService.cs

Repositories:
- MangaWorkflow.Application/Interfaces/Repositories/ITaskSubmissionRepository.cs
- MangaWorkflow.Infrastructure/Repositories/TaskSubmissionRepository.cs
- MangaWorkflow.Application/Interfaces/Repositories/IEditorCommentRepository.cs
- MangaWorkflow.Infrastructure/Repositories/EditorCommentRepository.cs
- MangaWorkflow.Application/Interfaces/Repositories/IPageRegionRepository.cs
- MangaWorkflow.Infrastructure/Repositories/PageRegionRepository.cs

File Storage:
- MangaWorkflow.Application/Interfaces/Services/IFileStorageService.cs
- MangaWorkflow.Infrastructure/FileStorage/LocalFileStorageService.cs

DTOs:
- DTOs/Tasks/TaskListItemDto.cs, TaskDetailDto.cs, TaskStatusOption.cs
- DTOs/Submissions/SubmitTaskDto.cs, PendingSubmissionDto.cs, SubmissionDetailDto.cs, ReviewSubmissionDto.cs
- DTOs/Comments/CommentListItemDto.cs, CommentDetailDto.cs, AddCommentDto.cs
- DTOs/Regions/RegionListItemDto.cs, CreateRegionDto.cs, CreateTaskFromRegionDto.cs, RegionTypeOption.cs, AssistantOption.cs

### Update DependencyInjection.cs Files

Application/DependencyInjection.cs — add:
- ITaskWorkflowService, ISubmissionService, IEditorCommentService, IPageRegionService

Infrastructure/DependencyInjection.cs — add:
- ITaskSubmissionRepository, IEditorCommentRepository, IPageRegionRepository, IFileStorageService

## Architecture Rules

Same as always:
- PageModels call Services only
- All DB calls async
- [Authorize] on all pages
- [BindProperty] for form fields
- TempData for flash messages
- Redirect after POST (PRG pattern)
- Insert notification DB records (no SignalR push yet)

## Validation Rules

- Task submission: file extension must be .jpg .jpeg .png .zip .psd, max 10MB
- Region coordinates: X, Y, Width, Height must all be > 0
- Task creation: Title required, AssignedTo required
- Comment: CommentText required, max 2000 chars
- Review rejection/revision: Reason required

## After Implementing

Run:
```
dotnet clean && dotnet restore && dotnet build
dotnet run --project MangaWorkflow.Tools.DbSmokeTest
dotnet run --project MangaWorkflow.Web
```

Test the full assistant workflow:
Mangaka adds region and task -> Assistant starts task -> Assistant submits -> Mangaka reviews and approves

Test the editor comment workflow:
Editor adds comment on page -> Mangaka sees notification in DB -> Editor resolves comment

## When Done

Create docs/PHASE_3_COMPLETION_REPORT.md
Update docs/PROJECT_STATUS.md with [DONE] Phase 3
