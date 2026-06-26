using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.DTOs.Rankings;

namespace MangaWorkflow.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class RankingsController : Controller
    {
        private readonly IRankingService _rankingService;
        private readonly ISeriesService _seriesService;

        public RankingsController(IRankingService rankingService, ISeriesService seriesService)
        {
            _rankingService = rankingService;
            _seriesService = seriesService;
        }

        // GET /Admin/Rankings
        public async Task<IActionResult> Index(string? issueNumber, CancellationToken ct)
        {
            var issues = await _rankingService.GetAvailableIssueNumbersAsync(ct);
            ViewBag.Issues = issues;
            ViewBag.SelectedIssue = issueNumber;

            List<RankingListItemDto> rankings;
            if (!string.IsNullOrWhiteSpace(issueNumber))
                rankings = await _rankingService.GetRankingsByIssueAsync(issueNumber, ct);
            else
                rankings = await _rankingService.GetLatestRankingsAsync(ct);

            return View(rankings);
        }

        // GET /Admin/Rankings/Create
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var series = await _seriesService.GetAllSeriesAsync(null, null, ct);
            ViewBag.SeriesList = series;
            return View();
        }

        // POST /Admin/Rankings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRankingDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var series = await _seriesService.GetAllSeriesAsync(null, null, ct);
                ViewBag.SeriesList = series;
                return View(dto);
            }

            try
            {
                await _rankingService.CreateOrUpdateRankingAsync(dto, ct);
                TempData["Success"] = $"Ranking for issue '{dto.IssueNumber}' saved.";
                return RedirectToAction(nameof(Index), new { issueNumber = dto.IssueNumber });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var series = await _seriesService.GetAllSeriesAsync(null, null, ct);
                ViewBag.SeriesList = series;
                return View(dto);
            }
        }
    }
}
