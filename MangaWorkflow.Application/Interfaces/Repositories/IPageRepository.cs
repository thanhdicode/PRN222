using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface IPageRepository
    {
        Task<List<MangaPage>> GetByChapterAsync(Guid chapterId, CancellationToken ct = default);
        Task<MangaPage?> GetByIdWithDetailsAsync(Guid pageId, CancellationToken ct = default);
        Task AddAsync(MangaPage page, CancellationToken ct = default);
        Task<PageStatus?> GetStatusByCodeAsync(string statusCode, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
