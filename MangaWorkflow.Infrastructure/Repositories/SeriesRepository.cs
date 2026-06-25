using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Domain.Entities;
using MangaWorkflow.Application.Interfaces.Repositories;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class SeriesRepository : ISeriesRepository
    {
        private readonly MangaWorkflowDbContext _context;
        public SeriesRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountSeriesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Series.CountAsync(cancellationToken);
        }

        public async Task<List<Series>> GetRecentSeriesAsync(int take = 10, CancellationToken cancellationToken = default)
        {
            return await _context.Series.OrderByDescending(s => s.CreatedAt).Take(take).ToListAsync(cancellationToken);
        }
    }
}
