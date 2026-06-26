using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var summary = await _dashboardService.GetDashboardSummaryAsync(ct);
            return View(summary);
        }
    }
}
