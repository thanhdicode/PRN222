using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.DTOs.Series;

namespace MangaWorkflow.Web.Areas.Mangaka.Controllers
{
    [Area("Mangaka")]
    [Authorize(Policy = "MangakaOnly")]
    public class SeriesController : Controller
    {
        private readonly ISeriesService _seriesService;

        public SeriesController(ISeriesService seriesService)
        {
            _seriesService = seriesService;
        }

        // GET /Mangaka/Series
        [HttpGet]
        public async Task<IActionResult> Index(string? statusCode, string? keyword, CancellationToken ct)
        {
            var mangakaId = GetCurrentUserId();
            if (mangakaId == null) return Unauthorized();

            var series = await _seriesService.GetSeriesForMangakaAsync(mangakaId.Value, statusCode, keyword, ct);
            var statuses = await _seriesService.GetStatusesAsync(ct);
            ViewBag.Statuses = statuses;
            ViewBag.SelectedStatus = statusCode;
            ViewBag.Keyword = keyword;
            return View(series);
        }

        // GET /Mangaka/Series/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var series = await _seriesService.GetSeriesDetailAsync(id, ct);
            if (series == null) return NotFound();

            if (!OwnsSeries(series.MangakaId)) return Forbid();
            return View(series);
        }

        // GET /Mangaka/Series/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST /Mangaka/Series/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSeriesDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var mangakaId = GetCurrentUserId();
            if (mangakaId == null) return Unauthorized();

            try
            {
                var seriesId = await _seriesService.CreateSeriesAsync(dto, mangakaId.Value, ct);
                TempData["Success"] = $"Series '{dto.Title}' created successfully.";
                return RedirectToAction(nameof(Details), new { id = seriesId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        // GET /Mangaka/Series/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
        {
            var series = await _seriesService.GetSeriesDetailAsync(id, ct);
            if (series == null) return NotFound();

            if (!OwnsSeries(series.MangakaId)) return Forbid();

            var dto = new EditSeriesDto
            {
                SeriesId = series.SeriesId,
                Title = series.Title,
                Genre = series.Genre,
                Description = series.Description,
                CoverImageUrl = series.CoverImageUrl
            };
            return View(dto);
        }

        // POST /Mangaka/Series/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditSeriesDto dto, CancellationToken ct)
        {
            if (id != dto.SeriesId) return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            // Re-check ownership on POST to prevent IDOR
            var series = await _seriesService.GetSeriesDetailAsync(id, ct);
            if (series == null) return NotFound();
            if (!OwnsSeries(series.MangakaId)) return Forbid();

            try
            {
                await _seriesService.UpdateSeriesAsync(dto, ct);
                TempData["Success"] = "Series updated.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        // POST /Mangaka/Series/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var mangakaId = GetCurrentUserId();
            if (mangakaId == null) return Unauthorized();

            try
            {
                await _seriesService.DeleteSeriesAsync(id, mangakaId.Value, isAdmin: false, ct);
                TempData["Success"] = "Series deleted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        // POST /Mangaka/Series/Submit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(Guid id, CancellationToken ct)
        {
            var mangakaId = GetCurrentUserId();
            if (mangakaId == null) return Unauthorized();

            try
            {
                await _seriesService.SubmitSeriesAsync(id, mangakaId.Value, ct);
                TempData["Success"] = "Series submitted for board review.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        /// <summary>Returns current user ID from claims, or null if claim is missing/invalid.</summary>
        private Guid? GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(idClaim, out var id) ? id : null;
        }

        /// <summary>Returns true if the current user owns the given series.</summary>
        private bool OwnsSeries(Guid mangakaId)
        {
            var currentUserId = GetCurrentUserId();
            return currentUserId.HasValue && mangakaId == currentUserId.Value;
        }
    }
}
