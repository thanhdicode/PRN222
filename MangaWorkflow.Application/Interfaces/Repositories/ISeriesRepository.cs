using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface ISeriesRepository
    {
        Task<int> CountSeriesAsync(CancellationToken cancellationToken = default);
        Task<List<Series>> GetRecentSeriesAsync(int take = 10, CancellationToken cancellationToken = default);
        Task<List<Series>> GetByMangakaAsync(Guid mangakaId, string? statusCode, string? keyword, CancellationToken ct = default);
        Task<List<Series>> GetAllFilteredAsync(string? statusCode, string? keyword, CancellationToken ct = default);
        Task<Series?> GetByIdWithDetailsAsync(Guid seriesId, CancellationToken ct = default);
        Task<List<Series>> GetSubmittedSeriesAsync(CancellationToken ct = default);
        Task AddAsync(Series series, CancellationToken ct = default);
        Task<List<SeriesStatus>> GetAllStatusesAsync(CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
