using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace MangaWorkflow.Web.Hubs
{
    [Authorize]
    public class WorkflowHub : Hub
    {
        // Simple Hub for generic workflow events.
    }
}
