using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangaWorkflow.Application.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using MangaWorkflow.Application.Interfaces.Repositories;

namespace MangaWorkflow.Web.Controllers
{
    // FIX: Removed space in roles string — "Mangaka, Admin" → "Mangaka,Admin"
    [Authorize(Roles = "Mangaka,Admin")]
    public class AiStudioController : Controller
    {
        private readonly IAiStudioService _aiStudioService;
        private readonly IPageRepository _pageRepository;
        private readonly ISeriesRepository _seriesRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IUserRepository _userRepository;

        public AiStudioController(
            IAiStudioService aiStudioService,
            IPageRepository pageRepository,
            ISeriesRepository seriesRepository,
            IChapterRepository chapterRepository,
            IUserRepository userRepository)
        {
            _aiStudioService = aiStudioService;
            _pageRepository = pageRepository;
            _seriesRepository = seriesRepository;
            _chapterRepository = chapterRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            // Get all series for the current Mangaka
            var series = await _seriesRepository.GetByMangakaAsync(userId.Value, null, null);

            // Build a list of pages with series and chapter context
            var pageViewModels = new List<MangaWorkflow.Web.Models.AiStudio.PageSelectionViewModel>();

            foreach (var s in series)
            {
                var chapters = await _chapterRepository.GetBySeriesAsync(s.SeriesId);
                foreach (var chapter in chapters)
                {
                    var pages = await _pageRepository.GetByChapterAsync(chapter.ChapterId);
                    foreach (var page in pages)
                    {
                        pageViewModels.Add(new MangaWorkflow.Web.Models.AiStudio.PageSelectionViewModel
                        {
                            PageId = page.PageId,
                            SeriesTitle = s.Title,
                            ChapterNumber = chapter.ChapterNumber,
                            PageNumber = page.PageNumber,
                            ImageUrl = page.ImageUrl ?? page.ThumbnailUrl ?? "/images/placeholder.jpg"
                        });
                    }
                }
            }

            return View(pageViewModels
                .OrderBy(p => p.SeriesTitle)
                .ThenBy(p => p.ChapterNumber)
                .ThenBy(p => p.PageNumber)
                .ToList());
        }

        // FIX: Added [ValidateAntiForgeryToken] to prevent CSRF
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Analyze(string pageId)
        {
            if (string.IsNullOrEmpty(pageId) || !Guid.TryParse(pageId, out var pageGuid))
            {
                ModelState.AddModelError("", "Valid Page ID is required.");
                return View("Index");
            }

            var page = await _pageRepository.GetByIdWithDetailsAsync(pageGuid);
            if (page == null)
            {
                ModelState.AddModelError("", "Page not found.");
                return View("Index");
            }

            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            // FIX: Ownership check — Mangaka can only analyze their own pages
            // Admin bypass: skip ownership check for Admin role
            if (!User.IsInRole("Admin"))
            {
                var pageOwnerSeriesId = page.Chapter?.SeriesId;
                if (pageOwnerSeriesId.HasValue)
                {
                    var ownerSeries = await _seriesRepository.GetByIdAsync(pageOwnerSeriesId.Value);
                    if (ownerSeries == null || ownerSeries.MangakaId != userId.Value)
                        return Forbid();
                }
            }

            var inference = await _aiStudioService.RunSegmentationAsync(pageGuid, userId.Value);
            var regions = await _aiStudioService.GetDetectedRegionsAsync(pageGuid);

            var resultDto = new MangaWorkflow.Application.DTOs.Ai.AiSegmentationResponseDto
            {
                PageId = pageId,
                ModelName = "YoloSeg",
                ModelVersion = "v1",
                ImageUrl = page.ImageUrl,
                Detections = regions.Select(r => new MangaWorkflow.Application.DTOs.Ai.AiDetectedRegionDto
                {
                    DetectedRegionId = r.DetectedRegionId,
                    Label = r.RegionTypeCode,
                    Confidence = r.Confidence,
                    X = r.X,
                    Y = r.Y,
                    Width = r.Width,
                    Height = r.Height,
                    Polygon = r.PolygonJson,
                    IsAccepted = r.IsAccepted
                }).ToList()
            };

            return View("SegmentationResult", resultDto);
        }

        // FIX: Added [ValidateAntiForgeryToken] to prevent CSRF
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateTaskSuggestions(string pageId)
        {
            if (string.IsNullOrEmpty(pageId) || !Guid.TryParse(pageId, out var pageGuid))
                return BadRequest("Valid Page ID is required.");

            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var suggestions = await _aiStudioService.SuggestTasksForPageAsync(pageGuid, userId.Value);

            var assistants = await _userRepository.GetAllAsync(null, "Assistant");
            ViewBag.Assistants = assistants;
            ViewBag.PageId = pageId;

            return View("TaskSuggestions", suggestions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptRegion(Guid detectedRegionId, Guid pageId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            try
            {
                await _aiStudioService.AcceptRegionAsync(detectedRegionId, userId.Value);
                TempData["SuccessMessage"] = "Region accepted successfully";
                return RedirectToAction(nameof(Analyze), new { pageId = pageId.ToString() });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to accept region: {ex.Message}";
                return RedirectToAction(nameof(Analyze), new { pageId = pageId.ToString() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRegion(Guid detectedRegionId, Guid pageId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            try
            {
                await _aiStudioService.RejectRegionAsync(detectedRegionId, userId.Value);
                TempData["SuccessMessage"] = "Region rejected successfully";
                return RedirectToAction(nameof(Analyze), new { pageId = pageId.ToString() });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to reject region: {ex.Message}";
                return RedirectToAction(nameof(Analyze), new { pageId = pageId.ToString() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveTaskSuggestion(Guid suggestionId, Guid? assistantId, DateTime? deadline, Guid pageId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            try
            {
                await _aiStudioService.ApproveTaskSuggestionAsync(suggestionId, userId.Value, assistantId, deadline);
                TempData["SuccessMessage"] = "Task suggestion approved and production task created successfully";
                return RedirectToAction(nameof(GenerateTaskSuggestions), new { pageId = pageId.ToString() });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to approve suggestion: {ex.Message}";
                return RedirectToAction(nameof(GenerateTaskSuggestions), new { pageId = pageId.ToString() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectTaskSuggestion(Guid suggestionId, Guid pageId)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            try
            {
                await _aiStudioService.RejectTaskSuggestionAsync(suggestionId, userId.Value);
                TempData["SuccessMessage"] = "Task suggestion rejected successfully";
                return RedirectToAction(nameof(GenerateTaskSuggestions), new { pageId = pageId.ToString() });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to reject suggestion: {ex.Message}";
                return RedirectToAction(nameof(GenerateTaskSuggestions), new { pageId = pageId.ToString() });
            }
        }

        /// <summary>Returns current user ID from claims, or null if claim is missing/invalid.</summary>
        private Guid? GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(idClaim, out var id) ? id : null;
        }
    }
}
