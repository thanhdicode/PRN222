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

        public AiStudioController(IAiStudioService aiStudioService, IPageRepository pageRepository)
        {
            _aiStudioService = aiStudioService;
            _pageRepository = pageRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Just returning a simple view where the user can enter a PageId
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PageAnalysis(string pageId)
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
                Detections = regions.Select(r => new MangaWorkflow.Application.DTOs.Ai.AiDetectedRegionDto
                {
                    Label = r.RegionTypeCode,
                    Confidence = r.Confidence,
                    X = r.X,
                    Y = r.Y,
                    Width = r.Width,
                    Height = r.Height,
                    Polygon = r.PolygonJson
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
            return View("TaskSuggestions", suggestions);
        }
    }
}
