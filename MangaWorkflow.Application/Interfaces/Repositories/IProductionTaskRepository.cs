namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface IProductionTaskRepository
    {
        Task<int> CountTasksAsync(CancellationToken cancellationToken = default);
    }
}
