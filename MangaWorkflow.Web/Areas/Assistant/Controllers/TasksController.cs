using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Web.Areas.Assistant.Controllers
{
    [Area("Assistant")]
    [Authorize(Roles = "Assistant")]
    public class TasksController : Controller
    {
        private readonly ITaskWorkflowService _taskService;

        public TasksController(ITaskWorkflowService taskService)
        {
            _taskService = taskService;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET /Assistant/Tasks
        [HttpGet]
        public async Task<IActionResult> Index(string? status)
        {
            var tasks = await _taskService.GetAssistantTasksAsync(CurrentUserId, status);
            ViewData["StatusFilter"] = status;
            ViewData["StatusOptions"] = await _taskService.GetTaskStatusOptionsAsync();
            return View(tasks);
        }

        // GET /Assistant/Tasks/Detail/{id}
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var task = await _taskService.GetTaskDetailAsync(id);
            if (task == null) return NotFound();
            return View(task);
        }

        // POST /Assistant/Tasks/Start
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(Guid taskId)
        {
            await _taskService.StartTaskAsync(taskId, CurrentUserId);
            TempData["Success"] = "Task started. Good luck!";
            return RedirectToAction(nameof(Detail), new { id = taskId });
        }
    }
}
