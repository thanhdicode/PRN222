using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Web.Areas.Mangaka.Controllers
{
    [Area("Mangaka")]
    [Authorize(Policy = "MangakaOnly")]
    public class DashboardController : Controller
    {
        private readonly ISeriesService _seriesService;

        public DashboardController(ISeriesService seriesService)
        {
            _seriesService = seriesService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var mangakaId = GetCurrentUserId();
            var series = await _seriesService.GetSeriesForMangakaAsync(mangakaId, null, null, ct);
            return View(series);
        }

        private Guid GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(idClaim!);
        }
    }
}
