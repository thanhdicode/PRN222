using MangaWorkflow.Application.DTOs.Pages;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface IPageService
    {
        Task<List<PageListItemDto>> GetPagesByChapterAsync(Guid chapterId, CancellationToken ct = default);
        Task<PageDetailDto?> GetPageDetailAsync(Guid pageId, CancellationToken ct = default);
        Task<Guid> CreatePageAsync(CreatePageDto dto, Guid uploadedByUserId, string wwwRootPath, CancellationToken ct = default);
        Task DeletePageAsync(Guid pageId, CancellationToken ct = default);
    }
}
