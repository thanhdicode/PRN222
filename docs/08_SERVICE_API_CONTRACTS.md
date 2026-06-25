# 08 — Service and Contract Guide

This file defines service boundaries so AI agents do not invent inconsistent methods.

## Status code constants

Create constants rather than hardcoding strings everywhere.

```csharp
public static class RoleCodes
{
    public const string Admin = "Admin";
    public const string Mangaka = "Mangaka";
    public const string Assistant = "Assistant";
    public const string TantouEditor = "TantouEditor";
    public const string EditorialBoard = "EditorialBoard";
}
```

## Service interfaces

### ISeriesService

```csharp
public interface ISeriesService
{
    Task<List<SeriesListItemVm>> GetSeriesAsync(SeriesFilter filter, CancellationToken cancellationToken = default);
    Task<SeriesDetailVm?> GetDetailsAsync(Guid seriesId, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateSeriesVm model, Guid mangakaId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid seriesId, EditSeriesVm model, Guid actorUserId, CancellationToken cancellationToken = default);
    Task SubmitAsync(Guid seriesId, Guid mangakaId, CancellationToken cancellationToken = default);
}
```

### IChapterService

```csharp
public interface IChapterService
{
    Task<List<ChapterListItemVm>> GetBySeriesAsync(Guid seriesId, CancellationToken cancellationToken = default);
    Task<ChapterDetailVm?> GetDetailsAsync(Guid chapterId, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateChapterVm model, Guid mangakaId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid chapterId, EditChapterVm model, Guid actorUserId, CancellationToken cancellationToken = default);
}
```

### IPageService

```csharp
public interface IPageService
{
    Task<List<MangaPageVm>> GetByChapterAsync(Guid chapterId, CancellationToken cancellationToken = default);
    Task<Guid> AddPageAsync(CreateMangaPageVm model, Guid uploadedByUserId, CancellationToken cancellationToken = default);
    Task<Guid> AddRegionAsync(CreatePageRegionVm model, Guid createdByUserId, CancellationToken cancellationToken = default);
}
```

### ITaskService

```csharp
public interface ITaskService
{
    Task<List<TaskListItemVm>> GetTasksAsync(TaskFilter filter, CancellationToken cancellationToken = default);
    Task<TaskDetailVm?> GetDetailsAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<Guid> AssignTaskAsync(CreateProductionTaskVm model, Guid mangakaId, CancellationToken cancellationToken = default);
    Task StartTaskAsync(Guid taskId, Guid assistantId, CancellationToken cancellationToken = default);
    Task MarkOverdueTasksAsync(CancellationToken cancellationToken = default);
}
```

### ISubmissionService

```csharp
public interface ISubmissionService
{
    Task<Guid> SubmitAsync(Guid taskId, SubmitTaskVm model, Guid assistantId, CancellationToken cancellationToken = default);
    Task ReviewAsync(Guid submissionId, ReviewSubmissionVm model, Guid mangakaId, CancellationToken cancellationToken = default);
    Task<List<SubmissionListItemVm>> GetPendingReviewsAsync(Guid mangakaId, CancellationToken cancellationToken = default);
}
```

### IEditorCommentService

```csharp
public interface IEditorCommentService
{
    Task<List<EditorCommentVm>> GetByPageAsync(Guid pageId, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreateEditorCommentVm model, Guid editorId, CancellationToken cancellationToken = default);
    Task ResolveAsync(Guid commentId, Guid actorUserId, CancellationToken cancellationToken = default);
}
```

### IBoardVotingService

```csharp
public interface IBoardVotingService
{
    Task<List<SeriesListItemVm>> GetPendingSeriesAsync(CancellationToken cancellationToken = default);
    Task VoteAsync(Guid seriesId, CreateBoardVoteVm model, Guid boardMemberId, CancellationToken cancellationToken = default);
    Task<VoteSummaryVm> GetVoteSummaryAsync(Guid seriesId, CancellationToken cancellationToken = default);
    Task CreateDecisionAsync(Guid seriesId, CreatePublishingDecisionVm model, Guid boardMemberId, CancellationToken cancellationToken = default);
}
```

### IRankingService

```csharp
public interface IRankingService
{
    Task ImportReaderVoteAsync(ImportReaderVoteVm model, Guid boardMemberId, CancellationToken cancellationToken = default);
    Task<List<LatestRankingVm>> GetLatestRankingsAsync(CancellationToken cancellationToken = default);
    Task ScanRankingRiskAsync(CancellationToken cancellationToken = default);
}
```

### INotificationService

```csharp
public interface INotificationService
{
    Task<Guid> CreateAsync(CreateNotificationRequest request, CancellationToken cancellationToken = default);
    Task NotifyUserAsync(Guid userId, CreateNotificationRequest request, CancellationToken cancellationToken = default);
    Task<List<NotificationVm>> GetUnreadAsync(Guid userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken = default);
}
```

### IDashboardService

```csharp
public interface IDashboardService
{
    Task<MangakaDashboardVm> GetMangakaDashboardAsync(Guid mangakaId, CancellationToken cancellationToken = default);
    Task<AssistantDashboardVm> GetAssistantDashboardAsync(Guid assistantId, CancellationToken cancellationToken = default);
    Task<EditorDashboardVm> GetEditorDashboardAsync(Guid editorId, CancellationToken cancellationToken = default);
    Task<BoardDashboardVm> GetBoardDashboardAsync(CancellationToken cancellationToken = default);
}
```

### IAiSegmentationService

```csharp
public interface IAiSegmentationService
{
    Task<Guid> QueueSegmentationJobAsync(Guid pageId, Guid requestedByUserId, CancellationToken cancellationToken = default);
    Task ProcessMockJobAsync(Guid jobId, CancellationToken cancellationToken = default);
}
```

## ViewModel naming rule

Use suffixes:

- `Vm` for UI view model.
- `Dto` for service/data transfer.
- `Request` for service command input.
- `Filter` for list filters.

## Controller rule

Controllers should look like:

```csharp
public class SeriesController : Controller
{
    private readonly ISeriesService _seriesService;

    public SeriesController(ISeriesService seriesService)
    {
        _seriesService = seriesService;
    }

    public async Task<IActionResult> Index(SeriesFilter filter, CancellationToken cancellationToken)
    {
        var model = await _seriesService.GetSeriesAsync(filter, cancellationToken);
        return View(model);
    }
}
```

No heavy EF query in controllers unless prototyping.
