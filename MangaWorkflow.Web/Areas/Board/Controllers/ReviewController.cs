using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.DTOs.Board;

namespace MangaWorkflow.Web.Areas.Board.Controllers
{
    [Area("Board")]
    [Authorize(Policy = "AdminOrBoard")]
    public class ReviewController : Controller
    {
        private readonly IBoardReviewService _reviewService;

        public ReviewController(IBoardReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET /Board/Review
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var series = await _reviewService.GetSubmittedSeriesAsync(ct);
            return View(series);
        }

        // GET /Board/Review/Details/{id}
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var memberId = GetCurrentUserId();
            var detail = await _reviewService.GetSeriesForReviewAsync(id, memberId, ct);
            if (detail == null) return NotFound();

            var summary = await _reviewService.GetVoteSummaryAsync(id, ct);
            ViewBag.VoteSummary = summary;
            return View(detail);
        }

        // GET /Board/Review/Vote/{id}
        [Authorize(Policy = "BoardMember")]
        public async Task<IActionResult> Vote(Guid id, CancellationToken ct)
        {
            var memberId = GetCurrentUserId();
            var detail = await _reviewService.GetSeriesForReviewAsync(id, memberId, ct);
            if (detail == null) return NotFound();

            if (detail.CurrentMemberHasVoted)
            {
                TempData["Error"] = "You have already voted on this series.";
                return RedirectToAction(nameof(Details), new { id });
            }

            ViewBag.SeriesTitle = detail.Title;
            return View(new SubmitVoteDto { SeriesId = id });
        }

        // POST /Board/Review/Vote/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "BoardMember")]
        public async Task<IActionResult> Vote(Guid id, SubmitVoteDto dto, CancellationToken ct)
        {
            if (id != dto.SeriesId) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.SeriesTitle = "Series";
                return View(dto);
            }

            try
            {
                var memberId = GetCurrentUserId();
                await _reviewService.SubmitVoteAsync(dto, memberId, ct);
                TempData["Success"] = "Vote submitted successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.SeriesTitle = "Series";
                return View(dto);
            }
        }

        private Guid GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(idClaim!);
        }
    }
}
