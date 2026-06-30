using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;
using MangaWorkflow.Infrastructure.Persistence;

namespace MangaWorkflow.Infrastructure.Repositories;

public class TaskStatusRepository : ITaskStatusRepository
{
    private readonly MangaWorkflowDbContext _context;

    public TaskStatusRepository(MangaWorkflowDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<List<Domain.Entities.TaskStatus>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TaskStatuses.ToListAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task<Domain.Entities.TaskStatus?> GetByStatusCodeAsync(string statusCode, CancellationToken cancellationToken = default)
    {
        return await _context.TaskStatuses
            .FirstOrDefaultAsync(s => s.StatusCode == statusCode, cancellationToken);
    }
}
