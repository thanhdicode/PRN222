using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MangaWorkflow.Application.DTOs.Submissions;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Web.Pages.Mangaka
{
    [Authorize(Roles = "Mangaka,Admin")]
    public class ReviewSubmissionsModel : PageModel
    {
        private readonly ISubmissionService _submissionService;

        public ReviewSubmissionsModel(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        public List<PendingSubmissionDto> Submissions { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var mangakaId))
            {
                Submissions = await _submissionService.GetPendingSubmissionsForMangakaAsync(mangakaId);
            }
        }
    }
}
