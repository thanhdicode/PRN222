using System.Threading;
using System.Threading.Tasks;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    /// <summary>
    /// Lookup for SubmissionStatus IDs by StatusCode, avoiding hardcoded IDs.
    /// </summary>
    public interface ISubmissionStatusRepository
    {
        Task<int> GetIdByCodeAsync(string statusCode, CancellationToken ct = default);
    }
}
