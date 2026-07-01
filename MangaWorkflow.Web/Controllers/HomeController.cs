using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MangaWorkflow.Web.Models;

namespace MangaWorkflow.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // B9 FIX: Handle HTTP status codes (404, 403, 500, etc.) with a friendly page
    // Called by UseStatusCodePagesWithReExecute in Program.cs
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode)
    {
        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StatusCode = statusCode
        };
        return View(model);
    }
}
