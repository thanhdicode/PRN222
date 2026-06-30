using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories;

public interface ITaskTypeRepository
{
    Task<List<TaskType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TaskType?> GetByTypeCodeAsync(string typeCode, CancellationToken cancellationToken = default);
}
