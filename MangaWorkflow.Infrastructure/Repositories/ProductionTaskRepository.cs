using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class ProductionTaskRepository : IProductionTaskRepository
    {
        private readonly MangaWorkflowDbContext _context;
        public ProductionTaskRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountTasksAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ProductionTasks.CountAsync(cancellationToken);
        }
    }
}
