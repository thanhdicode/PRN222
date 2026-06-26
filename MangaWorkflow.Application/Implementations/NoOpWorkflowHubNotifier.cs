using System;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Application.Implementations
{
    public class NoOpWorkflowHubNotifier : IWorkflowHubNotifier
    {
        public Task SendToUserAsync(Guid userId, string eventName, object payload, CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        public Task SendToRoleAsync(string roleCode, string eventName, object payload, CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }
    }
}
