using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Web.Areas.Assistant.Controllers
{
    [Area("Assistant")]
    [Authorize(Roles = "Assistant")]
    public class DashboardController : Controller
    {
        private readonly ITaskWorkflowService _taskService;

        public DashboardController(ITaskWorkflowService taskService)
        {
            _taskService = taskService;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET /Assistant/Dashboard  (redirect target after Assistant login)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Show only active tasks (Assigned + InProgress) on dashboard
            var myTasks = await _taskService.GetAssistantTasksAsync(CurrentUserId, null);
            var activeTasks = myTasks
                .Where(t => t.StatusCode == "Assigned" || t.StatusCode == "InProgress")
                .ToList();
            return View(activeTasks);
        }
    }
}
