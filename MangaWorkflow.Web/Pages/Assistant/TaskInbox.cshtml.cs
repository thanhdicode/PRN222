using System;
using System.Collections.Generic;
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
    public class TaskInboxModel : PageModel
    {
        private readonly ITaskWorkflowService _taskService;

        public TaskInboxModel(ITaskWorkflowService taskService)
        {
            _taskService = taskService;
        }

        public List<TaskListItemDto> TaskList { get; set; } = new();

        [BindProperty(SupportsGet = true)] 
        public string? StatusFilter { get; set; }

        public async Task OnGetAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var userId))
            {
                TaskList = await _taskService.GetAssistantTasksAsync(userId, StatusFilter);
            }
        }
    }
}
