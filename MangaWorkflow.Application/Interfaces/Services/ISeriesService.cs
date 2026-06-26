using MangaWorkflow.Application.DTOs.Series;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface ISeriesService
    {
        // Legacy methods kept for Phase 1 compatibility
        Task<int> CountSeriesAsync(CancellationToken cancellationToken = default);
        Task<List<Series>> GetRecentSeriesAsync(int take = 10, CancellationToken cancellationToken = default);

        // Phase 2 methods
        Task<List<SeriesListItemDto>> GetSeriesForMangakaAsync(Guid mangakaId, string? statusCode, string? keyword, CancellationToken ct = default);
        Task<List<SeriesListItemDto>> GetAllSeriesAsync(string? statusCode, string? keyword, CancellationToken ct = default);
        Task<SeriesDetailDto?> GetSeriesDetailAsync(Guid seriesId, CancellationToken ct = default);
        Task<Guid> CreateSeriesAsync(CreateSeriesDto dto, Guid mangakaId, CancellationToken ct = default);
        Task UpdateSeriesAsync(EditSeriesDto dto, CancellationToken ct = default);
        Task DeleteSeriesAsync(Guid seriesId, Guid requestingUserId, bool isAdmin, CancellationToken ct = default);
        Task SubmitSeriesAsync(Guid seriesId, Guid mangakaId, CancellationToken ct = default);
        Task<List<SeriesStatus>> GetStatusesAsync(CancellationToken ct = default);
    }
}

