using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.DTOs.Submissions;

namespace MangaWorkflow.Web.Areas.Assistant.Controllers
{
    [Area("Assistant")]
    [Authorize(Roles = "Assistant")]
    public class SubmissionsController : Controller
    {
        private readonly ISubmissionService _submissionService;

        public SubmissionsController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET /Assistant/Submissions/Submit/{taskId}
        [HttpGet]
        public IActionResult Submit(Guid taskId)
        {
            ViewData["TaskId"] = taskId;
            return View();
        }

        // POST /Assistant/Submissions/Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(SubmitTaskDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _submissionService.SubmitTaskAsync(dto, CurrentUserId);
            TempData["Success"] = "Submission uploaded successfully.";
            return RedirectToAction("Detail", "Tasks", new { area = "Assistant", id = dto.TaskId });
        }
    }
}
