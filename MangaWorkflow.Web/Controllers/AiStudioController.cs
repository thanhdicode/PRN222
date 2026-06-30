using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangaWorkflow.Application.Interfaces;
using System.Threading.Tasks;
using MangaWorkflow.Application.Interfaces.Repositories;

namespace MangaWorkflow.Web.Controllers
{
    [Authorize(Roles = "Mangaka, Admin")]
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

        public async Task<IActionResult> Index()
        {
            // Get current user ID
            var userId = Guid.Empty;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var claimUserId))
            {
                userId = claimUserId;
            }

            if (userId == Guid.Empty)
            {
                ModelState.AddModelError("", "User not authenticated");
                return View(new List<MangaWorkflow.Web.Models.AiStudio.PageSelectionViewModel>());
            }

            // Get all series for the current Mangaka
            var series = await _seriesRepository.GetByMangakaAsync(userId, null, null);
            
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

            return View(pageViewModels.OrderBy(p => p.SeriesTitle).ThenBy(p => p.ChapterNumber).ThenBy(p => p.PageNumber).ToList());
        }

        [HttpPost]
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

            // Assume System User for now if not found
            var userId = Guid.Empty;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var claimUserId))
            {
                userId = claimUserId;
            }

            // Call the AI Studio service to perform segmentation
            var inference = await _aiStudioService.RunSegmentationAsync(pageGuid, userId);
            var regions = await _aiStudioService.GetDetectedRegionsAsync(pageGuid);
            
            var resultDto = new MangaWorkflow.Application.DTOs.Ai.AiSegmentationResponseDto
            {
                PageId = pageId,
                ModelName = "YoloSeg", // Hardcoded or from DB
                ModelVersion = "v1",
                ImageUrl = page.ImageUrl, // Pass actual image URL to view
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

        [HttpPost]
        public async Task<IActionResult> GenerateTaskSuggestions(string pageId)
        {
            if (string.IsNullOrEmpty(pageId) || !Guid.TryParse(pageId, out var pageGuid))
            {
                return BadRequest("Valid Page ID is required.");
            }

            var userId = Guid.Empty;
            if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var claimUserId))
            {
                userId = claimUserId;
            }

            var suggestions = await _aiStudioService.SuggestTasksForPageAsync(pageGuid, userId);
            
            // Load assistants for the dropdown
            var assistants = await _userRepository.GetAllAsync(null, "Assistant");
            ViewBag.Assistants = assistants;
            ViewBag.PageId = pageId;
            
            return View("TaskSuggestions", suggestions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptRegion(Guid detectedRegionId, Guid pageId)
        {
            try
            {
                var userId = Guid.Empty;
                if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var claimUserId))
                {
                    userId = claimUserId;
                }

                if (userId == Guid.Empty)
                {
                    return Unauthorized("User not authenticated");
                }

                await _aiStudioService.AcceptRegionAsync(detectedRegionId, userId);
                
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
            try
            {
                var userId = Guid.Empty;
                if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var claimUserId))
                {
                    userId = claimUserId;
                }

                if (userId == Guid.Empty)
                {
                    return Unauthorized("User not authenticated");
                }

                await _aiStudioService.RejectRegionAsync(detectedRegionId, userId);
                
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
            try
            {
                var userId = Guid.Empty;
                if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var claimUserId))
                {
                    userId = claimUserId;
                }

                if (userId == Guid.Empty)
                {
                    return Unauthorized("User not authenticated");
                }

                await _aiStudioService.ApproveTaskSuggestionAsync(suggestionId, userId, assistantId, deadline);
                
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
            try
            {
                var userId = Guid.Empty;
                if (Guid.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var claimUserId))
                {
                    userId = claimUserId;
                }

                if (userId == Guid.Empty)
                {
                    return Unauthorized("User not authenticated");
                }

                await _aiStudioService.RejectTaskSuggestionAsync(suggestionId, userId);
                
                TempData["SuccessMessage"] = "Task suggestion rejected successfully";
                return RedirectToAction(nameof(GenerateTaskSuggestions), new { pageId = pageId.ToString() });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to reject suggestion: {ex.Message}";
                return RedirectToAction(nameof(GenerateTaskSuggestions), new { pageId = pageId.ToString() });
            }
        }
    }
}
