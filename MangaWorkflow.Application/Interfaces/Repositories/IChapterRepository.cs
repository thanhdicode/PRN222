using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface IChapterRepository
    {
        Task<int> CountChaptersAsync(CancellationToken cancellationToken = default);
        Task<List<Chapter>> GetBySeriesAsync(Guid seriesId, CancellationToken ct = default);
        Task<Chapter?> GetByIdWithDetailsAsync(Guid chapterId, CancellationToken ct = default);
        Task AddAsync(Chapter chapter, CancellationToken ct = default);
        Task<List<ChapterStatus>> GetAllStatusesAsync(CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
