using MangaWorkflow.Domain.Entities;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Application.Services
{
    public class SeriesService : ISeriesService
    {
        private readonly ISeriesRepository _seriesRepository;
        public SeriesService(ISeriesRepository seriesRepository)
        {
            _seriesRepository = seriesRepository;
        }

        public async Task<int> CountSeriesAsync(CancellationToken cancellationToken = default)
        {
            return await _seriesRepository.CountSeriesAsync(cancellationToken);
        }

        public async Task<List<Series>> GetRecentSeriesAsync(int take = 10, CancellationToken cancellationToken = default)
        {
            return await _seriesRepository.GetRecentSeriesAsync(take, cancellationToken);
        }
    }
}
