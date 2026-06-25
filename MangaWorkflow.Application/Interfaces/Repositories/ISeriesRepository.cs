using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface ISeriesRepository
    {
        Task<int> CountSeriesAsync(CancellationToken cancellationToken = default);
        Task<List<Series>> GetRecentSeriesAsync(int take = 10, CancellationToken cancellationToken = default);
    }
}
