using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MangaWorkflow.Application.DTOs.Tasks;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Web.Pages.Assistant
{
    [Authorize(Roles = "Assistant")]
    public class TaskDetailModel : PageModel
    {
        private readonly ITaskWorkflowService _taskService;

        public TaskDetailModel(ITaskWorkflowService taskService)
        {
            _taskService = taskService;
        }

        public TaskDetailDto? Task { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid taskId)
        {
            Task = await _taskService.GetTaskDetailAsync(taskId);
            
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Task != null && Guid.TryParse(userIdString, out var userId) && Task.AssignedToUserId != userId)
            {
                Task = null; // Deny access
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostStartAsync(Guid taskId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var userId))
            {
                await _taskService.StartTaskAsync(taskId, userId);
                TempData["Success"] = "Task started.";
            }
            return RedirectToPage(new { taskId = taskId });
        }
    }
}
