namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface IChapterRepository
    {
        Task<int> CountChaptersAsync(CancellationToken cancellationToken = default);
    }
}
