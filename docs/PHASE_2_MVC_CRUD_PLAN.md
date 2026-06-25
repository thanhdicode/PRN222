# PHASE 2 — ASP.NET Core MVC CRUD Core System
# Extremely Detailed Implementation Guide

**PRN222 Chapter Proven**: Chapter 04 — ASP.NET Core MVC
**Depends on**: Phase 1 Complete (Build passes, DbSmokeTest passes)
**Status**: NEXT — not started

---

## 1. What This Phase Proves in PRN222

Phase 2 demonstrates Chapter 04 ASP.NET Core MVC mastery:
- MVC architectural pattern (Model-View-Controller)
- Areas for role-based screen grouping
- Controller actions with HTTP GET and POST
- Model binding from forms to DTOs
- Data annotations for server-side validation
- Role-based authorization with [Authorize] attribute
- Tag helpers in Razor views (asp-for, asp-action, asp-controller, asp-area, asp-validation-for)
- Partial views and layout inheritance
- Search and filter patterns
- Service/Repository pattern called from controllers

This is equivalent to PRN222 Assignment 01 style CRUD.

---

## 2. What Must NOT Be Built in Phase 2

- NO Razor Pages (Pages/ folder screens) — that is Phase 3
- NO Blazor components — that is Phase 4
- NO SignalR JavaScript push — that is Phase 4
- NO Worker jobs — that is Phase 5
- NO complex canvas drawing UI
- NO external API calls
- NO real file AI processing

Notification records MAY be inserted into the database (e.g., after a board vote),
but SignalR push events are NOT wired yet.

---

## 3. Authentication Must Be Implemented First

Before any CRUD screen works, implement cookie authentication.

### 3.1 Login System

Files to create:
- MangaWorkflow.Web/Controllers/AccountController.cs
- MangaWorkflow.Web/Views/Account/Login.cshtml
- MangaWorkflow.Application/Interfaces/Services/IAuthService.cs
- MangaWorkflow.Application/Services/AuthService.cs
- MangaWorkflow.Application/DTOs/Auth/LoginDto.cs

### AccountController Actions
- GET /Account/Login — show login form
- POST /Account/Login — validate credentials, set cookie, redirect
- GET /Account/Logout — clear cookie, redirect to Login

### AuthService.cs
```csharp
public interface IAuthService
{
    Task<User?> ValidateCredentialsAsync(string email, string password);
}
```

Password handling for PRN222 demo:
Option A: store plain demo passwords in database and compare directly.
Option B: use BCrypt.Net-Next package for password hashing.
Option C: accept any password for seeded accounts (pure demo mode with TODO comment).
The agent may choose Option A or B. Document the choice in PROJECT_STATUS.md.

### ClaimsPrincipal construction
After validation, build claims:
```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
    new Claim(ClaimTypes.Name, user.Email),
    new Claim(ClaimTypes.GivenName, user.FullName ?? user.Email),
};
// Add role claims from user.UserRoles
foreach (var ur in user.UserRoles)
{
    claims.Add(new Claim(ClaimTypes.Role, ur.Role.RoleCode));
}
var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
```

### Program.cs additions
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

// Must be before UseAuthorization
app.UseAuthentication();
app.UseAuthorization();
```

### NuGet packages (if not already present)
No additional packages needed — cookie auth is built into ASP.NET Core.
If choosing BCrypt: dotnet add MangaWorkflow.Web package BCrypt.Net-Next

---

## 4. Area Setup

### 4.1 Area Route Registration in Program.cs

```csharp
app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "mangaka",
    areaName: "Mangaka",
    pattern: "Mangaka/{controller=Series}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "board",
    areaName: "Board",
    pattern: "Board/{controller=SeriesReview}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

### 4.2 Each Area Needs These Files

For each area (Admin, Mangaka, Board), create:
- Areas/{AreaName}/Controllers/ — controllers go here
- Areas/{AreaName}/Views/ — views go here
- Areas/{AreaName}/Views/Shared/_Layout.cshtml — area layout (can inherit main layout)
- Areas/{AreaName}/Views/_ViewImports.cshtml
- Areas/{AreaName}/Views/_ViewStart.cshtml

_ViewImports.cshtml content:
```cshtml
@using MangaWorkflow.Web
@using MangaWorkflow.Application.DTOs
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

_ViewStart.cshtml content:
```cshtml
@{
    Layout = "_Layout";
}
```

---

## 5. Module A — Admin Area: User Management

### Files to Create

#### Application Layer
- MangaWorkflow.Application/Interfaces/Services/IUserService.cs
- MangaWorkflow.Application/Services/UserService.cs
- MangaWorkflow.Application/DTOs/Users/UserListItemDto.cs
- MangaWorkflow.Application/DTOs/Users/UserDetailDto.cs
- MangaWorkflow.Application/DTOs/Users/CreateUserDto.cs
- MangaWorkflow.Application/DTOs/Users/EditUserDto.cs
- MangaWorkflow.Application/DTOs/Users/AssignRoleDto.cs

Note: IUserRepository already exists in Interfaces/Repositories/
Note: UserRepository already exists in Infrastructure/Repositories/

Expand IUserRepository to add:
```csharp
Task<List<User>> GetAllAsync(string? keyword, string? roleCode, CancellationToken ct = default);
Task<User?> GetByIdWithRolesAsync(Guid userId, CancellationToken ct = default);
Task AddAsync(User user, CancellationToken ct = default);
Task<List<Role>> GetAllRolesAsync(CancellationToken ct = default);
Task SaveChangesAsync(CancellationToken ct = default);
```

IUserService.cs:
```csharp
Task<List<UserListItemDto>> GetUsersAsync(string? keyword, string? roleCode, CancellationToken ct = default);
Task<UserDetailDto?> GetUserDetailAsync(Guid userId, CancellationToken ct = default);
Task CreateUserAsync(CreateUserDto dto, CancellationToken ct = default);
Task UpdateUserAsync(EditUserDto dto, CancellationToken ct = default);
Task AssignRoleAsync(AssignRoleDto dto, CancellationToken ct = default);
Task SetUserStatusAsync(Guid userId, bool isActive, CancellationToken ct = default);
Task<List<Role>> GetRolesAsync(CancellationToken ct = default);
```

UserListItemDto.cs:
```csharp
public class UserListItemDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = "";
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime? CreatedAt { get; set; }
}
```

UserDetailDto.cs:
```csharp
public class UserDetailDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = "";
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
```

CreateUserDto.cs:
```csharp
public class CreateUserDto
{
    [Required] [EmailAddress] public string Email { get; set; } = "";
    [Required] [MinLength(6)] public string Password { get; set; } = "";
    [MaxLength(200)] public string? FullName { get; set; }
    public string? RoleCode { get; set; }
}
```

EditUserDto.cs:
```csharp
public class EditUserDto
{
    public Guid UserId { get; set; }
    [MaxLength(200)] public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsActive { get; set; }
}
```

#### Web Layer
- MangaWorkflow.Web/Areas/Admin/Controllers/UsersController.cs
- MangaWorkflow.Web/Areas/Admin/Views/Users/Index.cshtml
- MangaWorkflow.Web/Areas/Admin/Views/Users/Details.cshtml
- MangaWorkflow.Web/Areas/Admin/Views/Users/Create.cshtml
- MangaWorkflow.Web/Areas/Admin/Views/Users/Edit.cshtml
- MangaWorkflow.Web/Areas/Admin/Views/Users/AssignRole.cshtml

Also create a simple Admin Dashboard:
- MangaWorkflow.Web/Areas/Admin/Controllers/DashboardController.cs
- MangaWorkflow.Web/Areas/Admin/Views/Dashboard/Index.cshtml

### UsersController.cs Actions

```csharp
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    // GET /Admin/Users?keyword=&roleCode=
    public async Task<IActionResult> Index(string? keyword, string? roleCode, CancellationToken ct)

    // GET /Admin/Users/Details/{id}
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)

    // GET /Admin/Users/Create
    public async Task<IActionResult> Create(CancellationToken ct)

    // POST /Admin/Users/Create
    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserDto dto, CancellationToken ct)

    // GET /Admin/Users/Edit/{id}
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)

    // POST /Admin/Users/Edit/{id}
    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditUserDto dto, CancellationToken ct)

    // GET /Admin/Users/AssignRole/{id}
    public async Task<IActionResult> AssignRole(Guid id, CancellationToken ct)

    // POST /Admin/Users/AssignRole/{id}
    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRole(Guid id, AssignRoleDto dto, CancellationToken ct)

    // POST /Admin/Users/ToggleStatus/{id}
    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(Guid id, CancellationToken ct)
}
```

### Index.cshtml features
- Table listing all users with columns: FullName, Email, Roles, Status, CreatedAt, Actions
- Search form: text input for keyword, dropdown for role filter
- Actions column: Details | Edit | Toggle Status | Assign Role
- Pagination is optional for PRN222 demo

### Create.cshtml features
- Form fields: Email, Password, FullName, Role (dropdown)
- Validation summary and field-level validation messages

### Edit.cshtml features
- Form fields: FullName, AvatarUrl, IsActive checkbox
- Show current email (read-only)

---

## 6. Module B — Mangaka Area: Series CRUD

### Files to Create

#### Application Layer
- MangaWorkflow.Application/Interfaces/Services/ISeriesService.cs (EXPAND existing)
- MangaWorkflow.Application/DTOs/Series/SeriesListItemDto.cs
- MangaWorkflow.Application/DTOs/Series/SeriesDetailDto.cs
- MangaWorkflow.Application/DTOs/Series/CreateSeriesDto.cs
- MangaWorkflow.Application/DTOs/Series/EditSeriesDto.cs

Note: ISeriesService, SeriesService, ISeriesRepository, SeriesRepository already exist.
Expand them with the following methods if missing:

ISeriesService additions:
```csharp
Task<List<SeriesListItemDto>> GetSeriesForMangakaAsync(Guid mangakaId, string? status, string? keyword, CancellationToken ct = default);
Task<List<SeriesListItemDto>> GetAllSeriesAsync(string? status, string? keyword, CancellationToken ct = default);
Task<SeriesDetailDto?> GetSeriesDetailAsync(Guid seriesId, CancellationToken ct = default);
Task<Guid> CreateSeriesAsync(CreateSeriesDto dto, Guid mangakaId, CancellationToken ct = default);
Task UpdateSeriesAsync(EditSeriesDto dto, CancellationToken ct = default);
Task DeleteSeriesAsync(Guid seriesId, CancellationToken ct = default);
Task SubmitSeriesAsync(Guid seriesId, Guid mangakaId, CancellationToken ct = default);
Task<List<SeriesStatus>> GetStatusesAsync(CancellationToken ct = default);
```

SeriesListItemDto.cs:
```csharp
public class SeriesListItemDto
{
    public Guid SeriesId { get; set; }
    public string Title { get; set; } = "";
    public string? Genre { get; set; }
    public string StatusCode { get; set; } = "";
    public string StatusName { get; set; } = "";
    public string? MangakaName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int ChapterCount { get; set; }
}
```

CreateSeriesDto.cs:
```csharp
public class CreateSeriesDto
{
    [Required] [MaxLength(300)] public string Title { get; set; } = "";
    [MaxLength(100)] public string? Genre { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
}
```

EditSeriesDto.cs:
```csharp
public class EditSeriesDto
{
    public Guid SeriesId { get; set; }
    [Required] [MaxLength(300)] public string Title { get; set; } = "";
    [MaxLength(100)] public string? Genre { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
}
```

#### Web Layer
- MangaWorkflow.Web/Areas/Mangaka/Controllers/SeriesController.cs
- MangaWorkflow.Web/Areas/Mangaka/Views/Series/Index.cshtml
- MangaWorkflow.Web/Areas/Mangaka/Views/Series/Details.cshtml
- MangaWorkflow.Web/Areas/Mangaka/Views/Series/Create.cshtml
- MangaWorkflow.Web/Areas/Mangaka/Views/Series/Edit.cshtml
- MangaWorkflow.Web/Areas/Mangaka/Views/Series/Delete.cshtml

### SeriesController.cs

```csharp
[Area("Mangaka")]
[Authorize(Roles = "Mangaka,Admin")]
public class SeriesController : Controller
{
    // GET /Mangaka/Series?status=&keyword=
    public async Task<IActionResult> Index(string? status, string? keyword, CancellationToken ct)
    // Shows only the logged-in Mangaka's series (Admin sees all)

    // GET /Mangaka/Series/Details/{id}
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)

    // GET /Mangaka/Series/Create
    public IActionResult Create()

    // POST /Mangaka/Series/Create
    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSeriesDto dto, CancellationToken ct)

    // GET /Mangaka/Series/Edit/{id}
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)

    // POST /Mangaka/Series/Edit/{id}
    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditSeriesDto dto, CancellationToken ct)

    // GET /Mangaka/Series/Delete/{id}
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)

    // POST /Mangaka/Series/Delete/{id}
    [HttpPost, ActionName("Delete")] [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken ct)

    // POST /Mangaka/Series/Submit/{id}
    [HttpPost] [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(Guid id, CancellationToken ct)
    // Changes status Draft -> Submitted
}
```

### Business rules for Series
- Only the owning Mangaka may edit/delete their own series (unless Admin)
- Series may only be deleted when status is Draft
- Submit action: status must be Draft; changes to Submitted
- SubmitSeries in service must verify ownership

---

## 7. Module C — Mangaka Area: Chapter CRUD

### Files to Create

#### Application Layer
- MangaWorkflow.Application/Interfaces/Services/IChapterService.cs
- MangaWorkflow.Application/Services/ChapterService.cs
- MangaWorkflow.Application/Interfaces/Repositories/IChapterRepository.cs (EXPAND existing)
- MangaWorkflow.Application/DTOs/Chapters/ChapterListItemDto.cs
- MangaWorkflow.Application/DTOs/Chapters/ChapterDetailDto.cs
- MangaWorkflow.Application/DTOs/Chapters/CreateChapterDto.cs
- MangaWorkflow.Application/DTOs/Chapters/EditChapterDto.cs

Note: IChapterRepository already exists — expand it.

IChapterService.cs:
```csharp
Task<List<ChapterListItemDto>> GetChaptersBySeriesAsync(Guid seriesId, CancellationToken ct = default);
Task<ChapterDetailDto?> GetChapterDetailAsync(Guid chapterId, CancellationToken ct = default);
Task<Guid> CreateChapterAsync(CreateChapterDto dto, CancellationToken ct = default);
Task UpdateChapterAsync(EditChapterDto dto, CancellationToken ct = default);
Task DeleteChapterAsync(Guid chapterId, CancellationToken ct = default);
Task StartProductionAsync(Guid chapterId, CancellationToken ct = default);
```

CreateChapterDto.cs:
```csharp
public class CreateChapterDto
{
    public Guid SeriesId { get; set; }
    [Required] [Range(1, 9999)] public int ChapterNumber { get; set; }
    [Required] [MaxLength(300)] public string Title { get; set; } = "";
    [MaxLength(2000)] public string? Synopsis { get; set; }
    public DateTime? Deadline { get; set; }
    [CustomValidation...] // Deadline must be future if provided
}
```

EditChapterDto.cs:
```csharp
public class EditChapterDto
{
    public Guid ChapterId { get; set; }
    [Required] [Range(1, 9999)] public int ChapterNumber { get; set; }
    [Required] [MaxLength(300)] public string Title { get; set; } = "";
    [MaxLength(2000)] public string? Synopsis { get; set; }
    public DateTime? Deadline { get; set; }
}
```

ChapterDetailDto.cs:
```csharp
public class ChapterDetailDto
{
    public Guid ChapterId { get; set; }
    public Guid SeriesId { get; set; }
    public string SeriesTitle { get; set; } = "";
    public int ChapterNumber { get; set; }
    public string Title { get; set; } = "";
    public string? Synopsis { get; set; }
    public string StatusCode { get; set; } = "";
    public string StatusName { get; set; } = "";
    public DateTime? Deadline { get; set; }
    public int TotalPages { get; set; }
    public int TotalTasks { get; set; }
    public int ApprovedTasks { get; set; }
    public int OverdueTasks { get; set; }
}
```

#### Web Layer
- MangaWorkflow.Web/Areas/Mangaka/Controllers/ChaptersController.cs
- MangaWorkflow.Web/Areas/Mangaka/Views/Chapters/Index.cshtml
- MangaWorkflow.Web/Areas/Mangaka/Views/Chapters/Details.cshtml
- MangaWorkflow.Web/Areas/Mangaka/Views/Chapters/Create.cshtml
- MangaWorkflow.Web/Areas/Mangaka/Views/Chapters/Edit.cshtml

### ChaptersController Actions
- GET /Mangaka/Chapters/Index/{seriesId} — list chapters for series
- GET /Mangaka/Chapters/Details/{id}
- GET /Mangaka/Chapters/Create/{seriesId}
- POST /Mangaka/Chapters/Create
- GET /Mangaka/Chapters/Edit/{id}
- POST /Mangaka/Chapters/Edit/{id}
- POST /Mangaka/Chapters/StartProduction/{id} — Draft -> InProduction

### Validation rules for Chapters
- ChapterNumber must be > 0
- Title is required
- Deadline must be in the future if provided (use custom validation or check in service)

---

## 8. Module D — Mangaka Area: Manga Page Management

### Files to Create

#### Application Layer
- MangaWorkflow.Application/Interfaces/Services/IPageService.cs
- MangaWorkflow.Application/Services/PageService.cs
- MangaWorkflow.Application/Interfaces/Repositories/IPageRepository.cs
- MangaWorkflow.Infrastructure/Repositories/PageRepository.cs
- MangaWorkflow.Application/DTOs/Pages/PageListItemDto.cs
- MangaWorkflow.Application/DTOs/Pages/PageDetailDto.cs
- MangaWorkflow.Application/DTOs/Pages/CreatePageDto.cs

IPageService.cs:
```csharp
Task<List<PageListItemDto>> GetPagesByChapterAsync(Guid chapterId, CancellationToken ct = default);
Task<PageDetailDto?> GetPageDetailAsync(Guid pageId, CancellationToken ct = default);
Task<Guid> CreatePageAsync(CreatePageDto dto, CancellationToken ct = default);
Task DeletePageAsync(Guid pageId, CancellationToken ct = default);
```

CreatePageDto.cs:
```csharp
public class CreatePageDto
{
    public Guid ChapterId { get; set; }
    [Required] [Range(1, 9999)] public int PageNumber { get; set; }
    public string? FileUrl { get; set; } // demo URL or uploaded path
    public IFormFile? UploadedFile { get; set; }
}
```

PageListItemDto.cs:
```csharp
public class PageListItemDto
{
    public Guid PageId { get; set; }
    public int PageNumber { get; set; }
    public string? FileUrl { get; set; }
    public string StatusCode { get; set; } = "";
    public int VersionNumber { get; set; }
    public int RegionCount { get; set; }
}
```

#### Web Layer
- MangaWorkflow.Web/Areas/Mangaka/Controllers/PagesController.cs
- MangaWorkflow.Web/Areas/Mangaka/Views/Pages/Index.cshtml
- MangaWorkflow.Web/Areas/Mangaka/Views/Pages/Details.cshtml
- MangaWorkflow.Web/Areas/Mangaka/Views/Pages/Create.cshtml

### PagesController Actions
- GET /Mangaka/Pages/Index/{chapterId} — list pages for chapter
- GET /Mangaka/Pages/Details/{id} — view page with image
- GET /Mangaka/Pages/Create/{chapterId}
- POST /Mangaka/Pages/Create — handle optional file upload
- POST /Mangaka/Pages/Delete/{id}

### File Upload Rules
- Accept .jpg, .jpeg, .png, .webp only
- Max size: 10 MB
- Save to wwwroot/uploads/pages/{guid}{ext}
- If no file uploaded, accept a URL in the FileUrl field

---

## 9. Module E — Board Area: Series Review

### Files to Create

#### Application Layer
- MangaWorkflow.Application/Interfaces/Services/IBoardReviewService.cs
- MangaWorkflow.Application/Services/BoardReviewService.cs
- MangaWorkflow.Application/Interfaces/Repositories/IBoardVoteRepository.cs
- MangaWorkflow.Infrastructure/Repositories/BoardVoteRepository.cs
- MangaWorkflow.Application/DTOs/Board/BoardSeriesListItemDto.cs
- MangaWorkflow.Application/DTOs/Board/BoardSeriesDetailDto.cs
- MangaWorkflow.Application/DTOs/Board/SubmitVoteDto.cs
- MangaWorkflow.Application/DTOs/Board/VoteSummaryDto.cs

IBoardReviewService.cs:
```csharp
Task<List<BoardSeriesListItemDto>> GetSubmittedSeriesAsync(CancellationToken ct = default);
Task<BoardSeriesDetailDto?> GetSeriesForReviewAsync(Guid seriesId, CancellationToken ct = default);
Task SubmitVoteAsync(SubmitVoteDto dto, Guid boardMemberId, CancellationToken ct = default);
Task<VoteSummaryDto> GetVoteSummaryAsync(Guid seriesId, CancellationToken ct = default);
```

SubmitVoteDto.cs:
```csharp
public class SubmitVoteDto
{
    public Guid SeriesId { get; set; }
    [Required] public string VoteValueCode { get; set; } = ""; // Approve, Reject, NeedRevision, Abstain
    [MaxLength(1000)] public string? Comment { get; set; }
}
```

BoardSeriesDetailDto.cs:
```csharp
public class BoardSeriesDetailDto
{
    public Guid SeriesId { get; set; }
    public string Title { get; set; } = "";
    public string? Genre { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string MangakaName { get; set; } = "";
    public string StatusCode { get; set; } = "";
    public List<VoteSummaryItemDto> Votes { get; set; } = new();
    public bool CurrentMemberHasVoted { get; set; }
    public string? CurrentMemberVote { get; set; }
}
```

VoteSummaryDto.cs:
```csharp
public class VoteSummaryDto
{
    public int ApproveCount { get; set; }
    public int RejectCount { get; set; }
    public int NeedRevisionCount { get; set; }
    public int AbstainCount { get; set; }
    public int TotalVotes { get; set; }
}
```

#### Web Layer
- MangaWorkflow.Web/Areas/Board/Controllers/SeriesReviewController.cs
- MangaWorkflow.Web/Areas/Board/Views/SeriesReview/Index.cshtml
- MangaWorkflow.Web/Areas/Board/Views/SeriesReview/Details.cshtml
- MangaWorkflow.Web/Areas/Board/Views/SeriesReview/Vote.cshtml

### SeriesReviewController Actions
- GET /Board/SeriesReview — list submitted/underreview series
- GET /Board/SeriesReview/Details/{id} — view series + vote summary
- GET /Board/SeriesReview/Vote/{id} — show vote form
- POST /Board/SeriesReview/Vote/{id} — submit vote

### Business Rules for Board Review
- Prevent duplicate vote: one board member may vote once per series
- After vote: create a Notification record for the series Mangaka
- Vote options must come from VoteValues table (not hardcoded)
- After voting, redirect to Details page showing updated vote summary

---

## 10. Module F — Board Area: Ranking Management

### Files to Create

#### Application Layer
- MangaWorkflow.Application/Interfaces/Services/IRankingService.cs
- MangaWorkflow.Application/Services/RankingService.cs
- MangaWorkflow.Application/Interfaces/Repositories/IRankingRepository.cs
- MangaWorkflow.Infrastructure/Repositories/RankingRepository.cs
- MangaWorkflow.Application/DTOs/Rankings/RankingListItemDto.cs
- MangaWorkflow.Application/DTOs/Rankings/CreateRankingDto.cs

IRankingService.cs:
```csharp
Task<List<RankingListItemDto>> GetRankingsByIssueAsync(int issueNumber, CancellationToken ct = default);
Task<List<int>> GetAvailableIssueNumbersAsync(CancellationToken ct = default);
Task CreateOrUpdateRankingAsync(CreateRankingDto dto, CancellationToken ct = default);
```

CreateRankingDto.cs:
```csharp
public class CreateRankingDto
{
    public Guid SeriesId { get; set; }
    [Required] [Range(1, int.MaxValue)] public int IssueNumber { get; set; }
    [Required] [Range(1, int.MaxValue)] public int RankPosition { get; set; }
    [Required] [Range(0, int.MaxValue)] public int VoteCount { get; set; }
    public string? TrendCode { get; set; } // Up, Down, Stable, New
}
```

RankingListItemDto.cs:
```csharp
public class RankingListItemDto
{
    public Guid RankingId { get; set; }
    public string SeriesTitle { get; set; } = "";
    public int IssueNumber { get; set; }
    public int RankPosition { get; set; }
    public int VoteCount { get; set; }
    public string? TrendCode { get; set; }
    public string TrendDisplay { get; set; } = "";
    public decimal? CancellationRiskScore { get; set; }
}
```

#### Web Layer
- MangaWorkflow.Web/Areas/Board/Controllers/RankingsController.cs
- MangaWorkflow.Web/Areas/Board/Views/Rankings/Index.cshtml
- MangaWorkflow.Web/Areas/Board/Views/Rankings/Create.cshtml

### RankingsController Actions
- GET /Board/Rankings?issueNumber= — list rankings filtered by issue
- GET /Board/Rankings/Create — show form to enter new ranking
- POST /Board/Rankings/Create — save ranking, create RankingUpdated notification

### Validation rules for Rankings
- VoteCount >= 0
- RankPosition > 0
- IssueNumber required and > 0
- After saving: insert a Notification record with type RankingUpdated

---

## 11. Navigation Menu (Shared)

Update Views/Shared/_Layout.cshtml to show a role-based navigation menu:

```cshtml
@if (User.IsInRole("Admin"))
{
    <li><a asp-area="Admin" asp-controller="Users" asp-action="Index">Admin: Users</a></li>
    <li><a asp-area="Admin" asp-controller="Dashboard" asp-action="Index">Admin: Dashboard</a></li>
}
@if (User.IsInRole("Mangaka") || User.IsInRole("Admin"))
{
    <li><a asp-area="Mangaka" asp-controller="Series" asp-action="Index">My Series</a></li>
}
@if (User.IsInRole("EditorialBoard") || User.IsInRole("Admin"))
{
    <li><a asp-area="Board" asp-controller="SeriesReview" asp-action="Index">Board: Series Review</a></li>
    <li><a asp-area="Board" asp-controller="Rankings" asp-action="Index">Board: Rankings</a></li>
}
```

---

## 12. DependencyInjection.cs Updates

Update Application/DependencyInjection.cs to add all new services:
```csharp
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<IChapterService, ChapterService>();
services.AddScoped<IPageService, PageService>();
services.AddScoped<IBoardReviewService, BoardReviewService>();
services.AddScoped<IRankingService, RankingService>();
```

Update Infrastructure/DependencyInjection.cs to add new repositories:
```csharp
services.AddScoped<IPageRepository, PageRepository>();
services.AddScoped<IBoardVoteRepository, BoardVoteRepository>();
services.AddScoped<IRankingRepository, RankingRepository>();
```

---

## 13. Commands to Run After Phase 2

```
dotnet clean
dotnet restore
dotnet build
dotnet run --project MangaWorkflow.Tools.DbSmokeTest
dotnet run --project MangaWorkflow.Web
```

Then manually test in browser (see section 14).

---

## 14. Manual Test Checklist

A. Admin user test:
1. Navigate to https://localhost:5001/Account/Login
2. Login as admin@manga.local
3. Navigate to /Admin/Users
4. Verify user list loads
5. Search by name or email
6. Filter by role
7. Click Edit on a user
8. Change FullName, save
9. Click Assign Role on a user
10. Change role, save

B. Mangaka user test:
1. Login as mangaka@manga.local
2. Navigate to /Mangaka/Series
3. Verify only this Mangaka's series appears
4. Click Create Series
5. Fill in Title, Genre, Description
6. Submit form — verify series appears in list
7. Click Edit — modify title
8. Click Details — verify data shows
9. Click Submit on a Draft series — verify status changes to Submitted
10. Navigate to /Mangaka/Chapters/Index/{seriesId}
11. Create a chapter with valid ChapterNumber and Title and future Deadline
12. Navigate to /Mangaka/Pages/Index/{chapterId}
13. Create a page with a file upload or URL

C. Board user test:
1. Login as board@manga.local
2. Navigate to /Board/SeriesReview
3. Verify submitted series appear
4. Click Details on a series
5. Click Vote
6. Select vote value (Approve, Reject, NeedRevision, Abstain)
7. Submit vote
8. Verify vote summary shows
9. Verify notification was inserted in DB (check via SQL)
10. Navigate to /Board/Rankings
11. Create a new ranking entry
12. Verify it appears in list

---

## 15. Phase 2 Completion Criteria

Phase 2 is COMPLETE only when ALL of these are true:
[ ] dotnet build passes with zero errors
[ ] dotnet run DbSmokeTest passes
[ ] Login and cookie auth work for all demo accounts
[ ] Admin Users CRUD screens open and work
[ ] Mangaka Series CRUD screens open and work
[ ] Mangaka Chapter CRUD screens open and work
[ ] Mangaka Pages list/create/details screens open and work
[ ] Board Series Review list, detail, and vote screens open and work
[ ] Board Rankings list and create screens open and work
[ ] Full end-to-end flow: Mangaka creates series -> submits -> Board votes
[ ] After Board vote: notification record inserted in DB
[ ] docs/PROJECT_STATUS.md updated with [DONE] Phase 2
[ ] docs/PHASE_2_COMPLETION_REPORT.md created

---

## 16. What Phase 2 Completion Report Must Contain

docs/PHASE_2_COMPLETION_REPORT.md must include:
- Date completed
- List of files created (with file paths)
- Screenshot or description of each working screen
- Known limitations or TODOs
- Exact commands run and their output
- Confirmation that all completion criteria are met
