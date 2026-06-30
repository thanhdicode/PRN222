using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MangaWorkflow.Application.Interfaces.Repositories;

public interface ITaskStatusRepository
{
    Task<List<Domain.Entities.TaskStatus>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Domain.Entities.TaskStatus?> GetByStatusCodeAsync(string statusCode, CancellationToken cancellationToken = default);
}
