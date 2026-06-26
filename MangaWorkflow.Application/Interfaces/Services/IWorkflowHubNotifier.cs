using System;
using System.Threading;
using System.Threading.Tasks;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface IWorkflowHubNotifier
    {
        Task SendToUserAsync(Guid userId, string eventName, object payload, CancellationToken ct = default);
        Task SendToRoleAsync(string roleCode, string eventName, object payload, CancellationToken ct = default);
    }
}
