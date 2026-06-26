using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.DTOs.Chapters;

namespace MangaWorkflow.Web.Areas.Mangaka.Controllers
{
    [Area("Mangaka")]
    [Authorize(Policy = "MangakaOnly")]
    public class ChaptersController : Controller
    {
        private readonly IChapterService _chapterService;
        private readonly ISeriesService _seriesService;

        public ChaptersController(IChapterService chapterService, ISeriesService seriesService)
        {
            _chapterService = chapterService;
            _seriesService = seriesService;
        }

        // GET /Mangaka/Chapters?seriesId={id}
        public async Task<IActionResult> Index(Guid seriesId, CancellationToken ct)
        {
            var series = await _seriesService.GetSeriesDetailAsync(seriesId, ct);
            if (series == null) return NotFound();

            var chapters = await _chapterService.GetChaptersBySeriesAsync(seriesId, ct);
            ViewBag.Series = series;
            return View(chapters);
        }

        // GET /Mangaka/Chapters/Details/{id}
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var chapter = await _chapterService.GetChapterDetailAsync(id, ct);
            if (chapter == null) return NotFound();
            return View(chapter);
        }

        // GET /Mangaka/Chapters/Create?seriesId={id}
        public async Task<IActionResult> Create(Guid seriesId, CancellationToken ct)
        {
            var series = await _seriesService.GetSeriesDetailAsync(seriesId, ct);
            if (series == null) return NotFound();

            ViewBag.Series = series;
            return View(new CreateChapterDto { SeriesId = seriesId });
        }

        // POST /Mangaka/Chapters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateChapterDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var series = await _seriesService.GetSeriesDetailAsync(dto.SeriesId, ct);
                ViewBag.Series = series;
                return View(dto);
            }

            try
            {
                var chapterId = await _chapterService.CreateChapterAsync(dto, ct);
                TempData["Success"] = $"Chapter {dto.ChapterNumber} created.";
                return RedirectToAction(nameof(Details), new { id = chapterId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var series = await _seriesService.GetSeriesDetailAsync(dto.SeriesId, ct);
                ViewBag.Series = series;
                return View(dto);
            }
        }

        // GET /Mangaka/Chapters/Edit/{id}
        public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
        {
            var chapter = await _chapterService.GetChapterDetailAsync(id, ct);
            if (chapter == null) return NotFound();

            var dto = new EditChapterDto
            {
                ChapterId = chapter.ChapterId,
                ChapterNumber = chapter.ChapterNumber,
                Title = chapter.Title,
                Synopsis = chapter.Synopsis,
                Deadline = chapter.Deadline
            };
            return View(dto);
        }

        // POST /Mangaka/Chapters/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditChapterDto dto, CancellationToken ct)
        {
            if (id != dto.ChapterId) return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _chapterService.UpdateChapterAsync(dto, ct);
                TempData["Success"] = "Chapter updated.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        // POST /Mangaka/Chapters/StartProduction/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartProduction(Guid id, CancellationToken ct)
        {
            try
            {
                await _chapterService.StartProductionAsync(id, ct);
                TempData["Success"] = "Chapter moved to In Production.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
