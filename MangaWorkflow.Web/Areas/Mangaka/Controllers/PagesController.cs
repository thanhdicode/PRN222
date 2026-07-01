using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.DTOs.Pages;

namespace MangaWorkflow.Web.Areas.Mangaka.Controllers
{
    [Area("Mangaka")]
    [Authorize(Policy = "MangakaOnly")]
    public class PagesController : Controller
    {
        private readonly IPageService _pageService;
        private readonly IChapterService _chapterService;
        private readonly IWebHostEnvironment _env;

        public PagesController(IPageService pageService, IChapterService chapterService, IWebHostEnvironment env)
        {
            _pageService = pageService;
            _chapterService = chapterService;
            _env = env;
        }

        // GET /Mangaka/Pages?chapterId={id}
        [HttpGet]
        public async Task<IActionResult> Index(Guid chapterId, CancellationToken ct)
        {
            var chapter = await _chapterService.GetChapterDetailAsync(chapterId, ct);
            if (chapter == null) return NotFound();

            var pages = await _pageService.GetPagesByChapterAsync(chapterId, ct);
            ViewBag.Chapter = chapter;
            return View(pages);
        }

        // GET /Mangaka/Pages/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var page = await _pageService.GetPageDetailAsync(id, ct);
            if (page == null) return NotFound();
            return View(page);
        }

        // GET /Mangaka/Pages/Upload?chapterId={id}
        [HttpGet]
        public async Task<IActionResult> Upload(Guid chapterId, CancellationToken ct)
        {
            var chapter = await _chapterService.GetChapterDetailAsync(chapterId, ct);
            if (chapter == null) return NotFound();

            ViewBag.Chapter = chapter;
            return View(new CreatePageDto { ChapterId = chapterId });
        }

        // POST /Mangaka/Pages/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(CreatePageDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var chapter = await _chapterService.GetChapterDetailAsync(dto.ChapterId, ct);
                ViewBag.Chapter = chapter;
                return View(dto);
            }

            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            try
            {
                await _pageService.CreatePageAsync(dto, userId.Value, _env.WebRootPath, ct);
                TempData["Success"] = $"Page {dto.PageNumber} uploaded.";
                return RedirectToAction(nameof(Index), new { chapterId = dto.ChapterId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var chapter = await _chapterService.GetChapterDetailAsync(dto.ChapterId, ct);
                ViewBag.Chapter = chapter;
                return View(dto);
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
