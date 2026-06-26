using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Web.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MangaWorkflow.Web.Realtime
{
    public class SignalRWorkflowHubNotifier : IWorkflowHubNotifier
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRWorkflowHubNotifier(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToRoleAsync(string roleCode, string eventName, object payload, CancellationToken ct = default)
        {
            await _hubContext.Clients.Group($"role-{roleCode}").SendAsync(eventName, payload, ct);
        }

        public async Task SendToUserAsync(Guid userId, string eventName, object payload, CancellationToken ct = default)
        {
            await _hubContext.Clients.Group($"user-{userId}").SendAsync(eventName, payload, ct);
        }
    }
}
