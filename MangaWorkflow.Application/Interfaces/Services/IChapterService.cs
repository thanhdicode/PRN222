using MangaWorkflow.Application.DTOs.Chapters;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface IChapterService
    {
        Task<List<ChapterListItemDto>> GetChaptersBySeriesAsync(Guid seriesId, CancellationToken ct = default);
        Task<ChapterDetailDto?> GetChapterDetailAsync(Guid chapterId, CancellationToken ct = default);
        Task<Guid> CreateChapterAsync(CreateChapterDto dto, CancellationToken ct = default);
        Task UpdateChapterAsync(EditChapterDto dto, CancellationToken ct = default);
        Task DeleteChapterAsync(Guid chapterId, CancellationToken ct = default);
        Task StartProductionAsync(Guid chapterId, CancellationToken ct = default);
    }
}
