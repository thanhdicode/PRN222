using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MangaWorkflow.Application.DTOs.Submissions;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Web.Pages.Mangaka
{
    [Authorize(Roles = "Mangaka,Admin")]
    public class ReviewSubmissionDetailModel : PageModel
    {
        private readonly ISubmissionService _submissionService;

        public ReviewSubmissionDetailModel(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        public SubmissionDetailDto? Submission { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid submissionId)
        {
            Submission = await _submissionService.GetSubmissionDetailAsync(submissionId);
            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(Guid submissionId)
        {
            return await ProcessDecision(submissionId, "Approved", null);
        }

        public async Task<IActionResult> OnPostRevisionAsync(Guid submissionId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                ModelState.AddModelError("", "Reason is required for revision.");
                Submission = await _submissionService.GetSubmissionDetailAsync(submissionId);
                return Page();
            }
            return await ProcessDecision(submissionId, "RevisionRequired", reason);
        }

        public async Task<IActionResult> OnPostRejectAsync(Guid submissionId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                ModelState.AddModelError("", "Reason is required for rejection.");
                Submission = await _submissionService.GetSubmissionDetailAsync(submissionId);
                return Page();
            }
            return await ProcessDecision(submissionId, "Rejected", reason);
        }

        private async Task<IActionResult> ProcessDecision(Guid submissionId, string decision, string? reason)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var mangakaId))
            {
                var dto = new ReviewSubmissionDto
                {
                    SubmissionId = submissionId,
                    Decision = decision,
                    Reason = reason
                };
                await _submissionService.ReviewSubmissionAsync(dto, mangakaId);
                TempData["Success"] = $"Submission marked as {decision}.";
            }
            return RedirectToPage("/Mangaka/ReviewSubmissions");
        }
    }
}
