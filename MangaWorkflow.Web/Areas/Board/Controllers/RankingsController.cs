using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MangaWorkflow.Web.Areas.Board.Controllers
{
    [Area("Board")]
    [Authorize(Policy = "AdminOrBoard")]
    public class RankingsController : Controller
    {
        // GET /Board/Rankings
        // B2 FIX: Placeholder — full Rankings feature to be implemented in a future phase.
        // Previously returned 404 because no controller existed for this route.
        public IActionResult Index()
        {
            return View();
        }
    }
}
