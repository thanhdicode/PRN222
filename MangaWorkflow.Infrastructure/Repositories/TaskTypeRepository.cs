using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;
using MangaWorkflow.Infrastructure.Persistence;

namespace MangaWorkflow.Infrastructure.Repositories;

public class TaskTypeRepository : ITaskTypeRepository
{
    private readonly MangaWorkflowDbContext _context;

    public TaskTypeRepository(MangaWorkflowDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TaskTypes.ToListAsync(cancellationToken);
    }

    public async Task<TaskType?> GetByTypeCodeAsync(string typeCode, CancellationToken cancellationToken = default)
    {
        return await _context.TaskTypes
            .FirstOrDefaultAsync(t => t.TypeCode == typeCode, cancellationToken);
    }
}
