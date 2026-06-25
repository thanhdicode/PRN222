namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<int> CountUsersAsync(CancellationToken cancellationToken = default);
    }
}
