# PHASE 3 — Razor Pages Workflow Screens
# Detailed Implementation Guide

**PRN222 Chapter Proven**: Chapter 05 — ASP.NET Core Razor Pages
**Depends on**: Phase 2 Complete
**Status**: LOCKED until Phase 2 DONE

---

## 1. What This Phase Proves in PRN222

Phase 3 demonstrates Chapter 05 Razor Pages mastery:
- PageModel class with constructor DI
- OnGetAsync() handler for page load
- OnPostAsync() handler for form submit
- Named handler methods: OnPostStartAsync(), OnPostSubmitAsync(), OnPostApproveAsync()
- [BindProperty] attribute for form binding
- TempData for flash messages after redirect-after-post
- File upload via IFormFile in PageModel
- Validation with ModelState.IsValid
- Partial pages and page layout conventions

---

## 2. What Must NOT Be Built in Phase 3

- NO Blazor components (Phase 4)
- NO SignalR push events (Phase 4) — notification RECORDS may be inserted
- NO Worker jobs (Phase 5)
- NO canvas drawing UI

---

## 3. Required Files Overview

### Assistant Task Workflow (Pages/Assistant/)
- Pages/Assistant/TaskInbox.cshtml + TaskInbox.cshtml.cs
- Pages/Assistant/TaskDetail.cshtml + TaskDetail.cshtml.cs
- Pages/Assistant/SubmitTask.cshtml + SubmitTask.cshtml.cs

### Mangaka Submission Review (Pages/Mangaka/)
- Pages/Mangaka/ReviewSubmissions.cshtml + ReviewSubmissions.cshtml.cs
- Pages/Mangaka/ReviewSubmissionDetail.cshtml + ReviewSubmissionDetail.cshtml.cs
- Pages/Mangaka/PageRegions.cshtml + PageRegions.cshtml.cs

### Editor Page Comments (Pages/Editor/)
- Pages/Editor/PageComments.cshtml + PageComments.cshtml.cs
- Pages/Editor/CommentDetail.cshtml + CommentDetail.cshtml.cs

---

## 4. Required New Application Layer Files

### 4.1 Task Workflow Service

MangaWorkflow.Application/Interfaces/Services/ITaskWorkflowService.cs:
```csharp
Task<List<TaskListItemDto>> GetAssistantTasksAsync(Guid assistantId, string? statusFilter, CancellationToken ct = default);
Task<TaskDetailDto?> GetTaskDetailAsync(Guid taskId, CancellationToken ct = default);
Task StartTaskAsync(Guid taskId, Guid assistantId, CancellationToken ct = default);
Task<List<TaskStatusOption>> GetTaskStatusOptionsAsync(CancellationToken ct = default);
```

MangaWorkflow.Application/Services/TaskWorkflowService.cs — implements above.

DTOs needed:
- DTOs/Tasks/TaskListItemDto.cs
- DTOs/Tasks/TaskDetailDto.cs
- DTOs/Tasks/TaskStatusOption.cs

TaskListItemDto.cs:
```csharp
public class TaskListItemDto
{
    public Guid TaskId { get; set; }
    public string Title { get; set; } = "";
    public string TypeCode { get; set; } = "";
    public string TypeName { get; set; } = "";
    public string StatusCode { get; set; } = "";
    public string StatusName { get; set; } = "";
    public DateTime? Deadline { get; set; }
    public bool IsOverdue => Deadline.HasValue && Deadline.Value < DateTime.UtcNow && StatusCode is not ("Approved" or "Rejected" or "Cancelled");
    public string? SeriesTitle { get; set; }
    public string? ChapterTitle { get; set; }
    public int? PageNumber { get; set; }
}
```

### 4.2 Submission Service

MangaWorkflow.Application/Interfaces/Services/ISubmissionService.cs:
```csharp
Task SubmitTaskAsync(SubmitTaskDto dto, Guid assistantId, CancellationToken ct = default);
Task<List<PendingSubmissionDto>> GetPendingSubmissionsForMangakaAsync(Guid mangakaId, CancellationToken ct = default);
Task<SubmissionDetailDto?> GetSubmissionDetailAsync(Guid submissionId, CancellationToken ct = default);
Task ReviewSubmissionAsync(ReviewSubmissionDto dto, Guid mangakaId, CancellationToken ct = default);
```

MangaWorkflow.Application/Services/SubmissionService.cs — implements above.

SubmitTaskDto.cs:
```csharp
public class SubmitTaskDto
{
    public Guid TaskId { get; set; }
    [MaxLength(2000)] public string? Notes { get; set; }
    public string? FileUrl { get; set; }
    public IFormFile? UploadedFile { get; set; }
}
```

ReviewSubmissionDto.cs:
```csharp
public class ReviewSubmissionDto
{
    public Guid SubmissionId { get; set; }
    [Required] public string Decision { get; set; } = ""; // Approved, Rejected, RevisionRequired
    [MaxLength(1000)] public string? Reason { get; set; }
}
```

Repositories needed:
- MangaWorkflow.Application/Interfaces/Repositories/ITaskSubmissionRepository.cs
- MangaWorkflow.Infrastructure/Repositories/TaskSubmissionRepository.cs

Note: IProductionTaskRepository already exists — expand it with:
```csharp
Task<List<ProductionTask>> GetByAssistantAsync(Guid assistantId, string? statusCode, CancellationToken ct = default);
Task<ProductionTask?> GetWithDetailsAsync(Guid taskId, CancellationToken ct = default);
Task UpdateStatusAsync(Guid taskId, string statusCode, CancellationToken ct = default);
```

### 4.3 Editor Comment Service

MangaWorkflow.Application/Interfaces/Services/IEditorCommentService.cs:
```csharp
Task<List<CommentListItemDto>> GetCommentsForPageAsync(Guid pageId, string? statusFilter, CancellationToken ct = default);
Task<CommentDetailDto?> GetCommentDetailAsync(Guid commentId, CancellationToken ct = default);
Task AddCommentAsync(AddCommentDto dto, Guid editorId, CancellationToken ct = default);
Task ResolveCommentAsync(Guid commentId, Guid editorId, CancellationToken ct = default);
```

AddCommentDto.cs:
```csharp
public class AddCommentDto
{
    public Guid PageId { get; set; }
    [Required] [MaxLength(2000)] public string CommentText { get; set; } = "";
    public decimal? RegionX { get; set; }
    public decimal? RegionY { get; set; }
    public decimal? RegionWidth { get; set; }
    public decimal? RegionHeight { get; set; }
}
```

Repository needed:
- MangaWorkflow.Application/Interfaces/Repositories/IEditorCommentRepository.cs
- MangaWorkflow.Infrastructure/Repositories/EditorCommentRepository.cs

### 4.4 Page Region Service

MangaWorkflow.Application/Interfaces/Services/IPageRegionService.cs:
```csharp
Task<List<RegionListItemDto>> GetRegionsForPageAsync(Guid pageId, CancellationToken ct = default);
Task<Guid> CreateRegionAsync(CreateRegionDto dto, CancellationToken ct = default);
Task<Guid> CreateTaskFromRegionAsync(CreateTaskFromRegionDto dto, Guid mangakaId, CancellationToken ct = default);
Task<List<RegionTypeOption>> GetRegionTypesAsync(CancellationToken ct = default);
Task<List<AssistantOption>> GetAvailableAssistantsAsync(CancellationToken ct = default);
```

CreateRegionDto.cs:
```csharp
public class CreateRegionDto
{
    public Guid PageId { get; set; }
    [Required] public string RegionTypeCode { get; set; } = "";
    [Required] public decimal X { get; set; }
    [Required] public decimal Y { get; set; }
    [Required] public decimal Width { get; set; }
    [Required] public decimal Height { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }
}
```

CreateTaskFromRegionDto.cs:
```csharp
public class CreateTaskFromRegionDto
{
    public Guid RegionId { get; set; }
    [Required] public string TaskTypeCode { get; set; } = "";
    [Required] [MaxLength(300)] public string Title { get; set; } = "";
    public Guid? AssignedToUserId { get; set; }
    public DateTime? Deadline { get; set; }
    [MaxLength(2000)] public string? Instructions { get; set; }
}
```

Repositories needed:
- MangaWorkflow.Application/Interfaces/Repositories/IPageRegionRepository.cs
- MangaWorkflow.Infrastructure/Repositories/PageRegionRepository.cs

### 4.5 File Storage Service

MangaWorkflow.Application/Interfaces/Services/IFileStorageService.cs:
```csharp
Task<string> SaveFileAsync(IFormFile file, string subfolder, CancellationToken ct = default);
bool IsValidImageFile(IFormFile file);
bool IsValidSubmissionFile(IFormFile file);
```

MangaWorkflow.Infrastructure/FileStorage/LocalFileStorageService.cs — saves files to wwwroot/uploads/

---

## 5. Razor Pages Detailed Specs

### 5.1 Pages/Assistant/TaskInbox.cshtml

Authorization: [Authorize(Roles = "Assistant")]
Route: /Assistant/TaskInbox

OnGetAsync:
- Gets logged-in user's Guid from User.Claims
- Calls ITaskWorkflowService.GetAssistantTasksAsync(userId, statusFilter)
- Populates TaskList property

Page model properties:
```csharp
public List<TaskListItemDto> TaskList { get; set; } = new();
[BindProperty(SupportsGet = true)] public string? StatusFilter { get; set; }
```

View features:
- Table of tasks with columns: Title, Type, Status, Series/Chapter, Deadline, Overdue badge
- Filter dropdown: All, Assigned, InProgress, Submitted, Approved, Rejected, Overdue
- Link to TaskDetail page for each task
- Overdue tasks highlighted in red/amber

### 5.2 Pages/Assistant/TaskDetail.cshtml

Route: /Assistant/TaskDetail?taskId={id}

OnGetAsync(Guid taskId):
- Loads task detail
- Shows task information, page image if available

OnPostStartAsync(Guid taskId):
- Calls TaskWorkflowService.StartTaskAsync(taskId, userId)
- Updates status from Assigned to InProgress
- TempData["Success"] = "Task started."
- Redirects to TaskDetail

OnPostAsync redirect to SubmitTask:
- Links to SubmitTask page

### 5.3 Pages/Assistant/SubmitTask.cshtml

Authorization: [Authorize(Roles = "Assistant")]
Route: /Assistant/SubmitTask?taskId={id}

PageModel properties:
```csharp
public TaskDetailDto? Task { get; set; }
[BindProperty] public SubmitTaskDto Input { get; set; } = new();
```

OnGetAsync(Guid taskId):
- Loads task detail
- Only allows access if task is assigned to current user and status is InProgress or RevisionRequired

OnPostAsync():
- Validates ModelState
- If file uploaded: validates extension and size
- Calls IFileStorageService.SaveFileAsync(file, "submissions")
- Calls ISubmissionService.SubmitTaskAsync(Input, userId)
  - Creates TaskSubmission record
  - Updates ProductionTask status to Submitted
  - Creates Notification record for Mangaka (inserted into DB, no SignalR yet)
- TempData["Success"] = "Submission uploaded successfully."
- Redirects to TaskInbox

### 5.4 Pages/Mangaka/ReviewSubmissions.cshtml

Authorization: [Authorize(Roles = "Mangaka,Admin")]
Route: /Mangaka/ReviewSubmissions

OnGetAsync():
- Calls ISubmissionService.GetPendingSubmissionsForMangakaAsync(mangakaId)

View features:
- Table of pending submissions
- Columns: AssistantName, TaskTitle, SubmittedAt, FileUrl, Actions
- Link to ReviewSubmissionDetail

### 5.5 Pages/Mangaka/ReviewSubmissionDetail.cshtml

Route: /Mangaka/ReviewSubmissionDetail?submissionId={id}

OnGetAsync(Guid submissionId):
- Loads submission detail with file link

OnPostApproveAsync(Guid submissionId):
- Calls ReviewSubmissionAsync with Decision = "Approved"
- Creates Notification for Assistant
- TempData["Success"] = "Submission approved."
- Redirect

OnPostRejectAsync(Guid submissionId, string reason):
- Requires reason
- Decision = "Rejected"
- Creates Notification for Assistant

OnPostRevisionAsync(Guid submissionId, string reason):
- Requires reason
- Decision = "RevisionRequired"
- Updates ProductionTask back to RevisionRequired
- Creates Notification for Assistant

### 5.6 Pages/Mangaka/PageRegions.cshtml

Authorization: [Authorize(Roles = "Mangaka,Admin")]
Route: /Mangaka/PageRegions?pageId={id}

OnGetAsync(Guid pageId):
- Loads page detail with image URL
- Loads existing regions
- Loads region type dropdown options
- Loads assistant dropdown options

OnPostAddRegionAsync():
- [BindProperty] public CreateRegionDto RegionInput { get; set; }
- Validates X/Y/Width/Height > 0
- Calls IPageRegionService.CreateRegionAsync(RegionInput)
- Redirects back to same page

OnPostCreateTaskAsync():
- [BindProperty] public CreateTaskFromRegionDto TaskInput { get; set; }
- Calls IPageRegionService.CreateTaskFromRegionAsync(TaskInput, mangakaId)
- Creates Notification for assigned assistant
- Redirects

View features:
- Shows page image (img tag with FileUrl)
- Table of existing regions with X/Y/W/H and type
- Form: Add Region (numeric input fields for X, Y, Width, Height, RegionType dropdown, Notes)
- For each region: Create Task button opens form with TaskType, Title, AssignedTo dropdown, Deadline

### 5.7 Pages/Editor/PageComments.cshtml

Authorization: [Authorize(Roles = "Editor,Admin")]
Route: /Editor/PageComments?pageId={id}

OnGetAsync(Guid pageId, string? statusFilter):
- Loads page detail
- Loads comments filtered by status (Open, Resolved, All)

OnPostAddCommentAsync():
- [BindProperty] public AddCommentDto CommentInput { get; set; }
- Validates CommentText not empty
- Calls IEditorCommentService.AddCommentAsync(CommentInput, editorId)
- Creates Notification for Mangaka
- TempData["Success"] = "Comment added."
- Redirect

OnPostResolveCommentAsync(Guid commentId):
- Calls IEditorCommentService.ResolveCommentAsync(commentId, editorId)
- TempData["Success"] = "Comment resolved."
- Redirect

View features:
- Shows page image
- Filter dropdown: All, Open, Resolved
- Table of comments: CommentText, X/Y coordinates if set, Status, CreatedAt, Editor name
- For open comments: Resolve button
- Add comment form: CommentText (required), X/Y/Width/Height (optional numeric fields)

---

## 6. DependencyInjection.cs Updates

Application/DependencyInjection.cs — add:
```csharp
services.AddScoped<ITaskWorkflowService, TaskWorkflowService>();
services.AddScoped<ISubmissionService, SubmissionService>();
services.AddScoped<IEditorCommentService, EditorCommentService>();
services.AddScoped<IPageRegionService, PageRegionService>();
```

Infrastructure/DependencyInjection.cs — add:
```csharp
services.AddScoped<ITaskSubmissionRepository, TaskSubmissionRepository>();
services.AddScoped<IEditorCommentRepository, EditorCommentRepository>();
services.AddScoped<IPageRegionRepository, PageRegionRepository>();
services.AddScoped<IFileStorageService, LocalFileStorageService>();
```

---

## 7. Navigation Menu Update for Phase 3

Add to _Layout.cshtml:
```cshtml
@if (User.IsInRole("Assistant"))
{
    <li><a asp-page="/Assistant/TaskInbox">My Tasks</a></li>
}
@if (User.IsInRole("Mangaka") || User.IsInRole("Admin"))
{
    <li><a asp-page="/Mangaka/ReviewSubmissions">Review Submissions</a></li>
}
@if (User.IsInRole("Editor") || User.IsInRole("Admin"))
{
    <!-- Editor picks a page from MVC Series/Chapter/Page screens -->
    <li><a asp-area="Mangaka" asp-controller="Series" asp-action="Index">Series (view pages)</a></li>
}
```

---

## 8. Phase 3 Manual Test Flow

Full assistant submission workflow:
1. Login as mangaka@manga.local
2. Navigate to Mangaka/Pages/Details/{pageId}
3. Navigate to /Mangaka/PageRegions?pageId={id}
4. Add a region: X=100, Y=100, W=200, H=150, Type=Screentone
5. Create Task from region: Title="Apply screentone", AssignedTo=assistant@manga.local, Deadline=future date
6. Login as assistant@manga.local
7. Navigate to /Assistant/TaskInbox
8. Confirm task appears with status Assigned
9. Click Task Detail -> Start Task
10. Confirm status changes to InProgress
11. Click Submit Task
12. Upload a PNG file or enter a URL, add notes
13. Submit form
14. Confirm task status changes to Submitted
15. Login as mangaka@manga.local
16. Navigate to /Mangaka/ReviewSubmissions
17. Confirm submission appears
18. Click Review -> Approve
19. Confirm task status changes to Approved

Full editor comment workflow:
1. Login as editor@manga.local
2. Navigate to a series/chapter/page via MVC screens
3. Navigate to /Editor/PageComments?pageId={id}
4. Confirm page image visible
5. Add comment: "Fix the nose shading here", X=200, Y=300
6. Confirm comment appears in list
7. Click Resolve
8. Confirm comment status changes to Resolved
9. Check DB: notification for Mangaka should be inserted

---

## 9. Phase 3 Completion Criteria

[ ] dotnet build passes with zero errors
[ ] dotnet run DbSmokeTest passes
[ ] All Razor Pages routes open without 404 or 500 errors
[ ] Full assistant submission workflow completes end-to-end
[ ] Mangaka can review and approve/reject submissions
[ ] Mangaka can add page regions and create tasks
[ ] Editor can add and resolve page comments
[ ] After task submission: notification record inserted in DB
[ ] After editor comment: notification record inserted in DB
[ ] docs/PROJECT_STATUS.md updated with [DONE] Phase 3
[ ] docs/PHASE_3_COMPLETION_REPORT.md created
