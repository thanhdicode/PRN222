using System.Threading;
using System.Threading.Tasks;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    /// <summary>
    /// Lookup for NotificationType IDs by TypeCode, avoiding hardcoded IDs.
    /// </summary>
    public interface INotificationTypeRepository
    {
        Task<int?> GetIdByCodeAsync(string typeCode, CancellationToken ct = default);
    }
}
