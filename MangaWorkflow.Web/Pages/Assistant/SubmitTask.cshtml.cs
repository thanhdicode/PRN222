using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MangaWorkflow.Application.DTOs.Submissions;
using MangaWorkflow.Application.DTOs.Tasks;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Web.Pages.Assistant
{
    [Authorize(Roles = "Assistant")]
    public class SubmitTaskModel : PageModel
    {
        private readonly ITaskWorkflowService _taskService;
        private readonly ISubmissionService _submissionService;
        private readonly IFileStorageService _fileStorage;

        public SubmitTaskModel(ITaskWorkflowService taskService, ISubmissionService submissionService, IFileStorageService fileStorage)
        {
            _taskService = taskService;
            _submissionService = submissionService;
            _fileStorage = fileStorage;
        }

        public TaskDetailDto? Task { get; set; }

        [BindProperty] 
        public SubmitTaskDto Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid taskId)
        {
            Task = await _taskService.GetTaskDetailAsync(taskId);
            
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Task == null || 
                !Guid.TryParse(userIdString, out var userId) || 
                Task.AssignedToUserId != userId ||
                (Task.StatusCode != "InProgress" && Task.StatusCode != "RevisionRequired"))
            {
                return RedirectToPage("/Assistant/TaskInbox");
            }
            
            Input.TaskId = taskId;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Task = await _taskService.GetTaskDetailAsync(Input.TaskId);
                return Page();
            }

            if (Input.UploadedFile != null)
            {
                if (!_fileStorage.IsValidSubmissionFile(Input.UploadedFile))
                {
                    ModelState.AddModelError("Input.UploadedFile", "Invalid file extension. Please upload an image, PDF, or ZIP.");
                    Task = await _taskService.GetTaskDetailAsync(Input.TaskId);
                    return Page();
                }

                if (Input.UploadedFile.Length > 20 * 1024 * 1024) // 20MB limit
                {
                    ModelState.AddModelError("Input.UploadedFile", "File size exceeds the 20MB limit.");
                    Task = await _taskService.GetTaskDetailAsync(Input.TaskId);
                    return Page();
                }

                Input.FileUrl = await _fileStorage.SaveFileAsync(Input.UploadedFile, "submissions");
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var userId))
            {
                await _submissionService.SubmitTaskAsync(Input, userId);
                TempData["Success"] = "Submission uploaded successfully.";
            }

            return RedirectToPage("/Assistant/TaskInbox");
        }
    }
}
