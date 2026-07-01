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
        private readonly IFileStorageService _fileStorage;

        public SubmissionsController(
            ISubmissionService submissionService,
            IFileStorageService fileStorage)
        {
            _submissionService = submissionService;
            _fileStorage = fileStorage;
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

            if (dto.UploadedFile != null)
            {
                if (!_fileStorage.IsValidSubmissionFile(dto.UploadedFile))
                {
                    ModelState.AddModelError(nameof(dto.UploadedFile), "Invalid file type. Please upload an image, PDF, ZIP, or RAR file.");
                    return View(dto);
                }

                if (dto.UploadedFile.Length > 20 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(dto.UploadedFile), "File size exceeds the 20MB limit.");
                    return View(dto);
                }

                dto.FileUrl = await _fileStorage.SaveFileAsync(dto.UploadedFile, "submissions");
            }

            await _submissionService.SubmitTaskAsync(dto, CurrentUserId);
            TempData["Success"] = "Submission uploaded successfully.";
            return RedirectToAction("Detail", "Tasks", new { area = "Assistant", id = dto.TaskId });
        }
    }
}
