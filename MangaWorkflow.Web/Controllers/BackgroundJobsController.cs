using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Web.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Route("admin/jobs")]
    public class BackgroundJobsController : Controller
    {
        private readonly IBackgroundJobService _jobService;

        public BackgroundJobsController(IBackgroundJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("deadline-reminders")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunDeadlineReminders(CancellationToken ct)
        {
            int count = await _jobService.ProcessDeadlineRemindersAsync(ct);
            TempData["SuccessMessage"] = $"Manually triggered Deadline Reminders. Sent {count} reminders.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("overdue-tasks")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunOverdueTasks(CancellationToken ct)
        {
            int count = await _jobService.ProcessOverdueTasksAsync(ct);
            TempData["SuccessMessage"] = $"Manually triggered Overdue Task Scanner. Marked {count} tasks as overdue.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("ranking-risks")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunRankingRisks(CancellationToken ct)
        {
            int count = await _jobService.ProcessRankingRisksAsync(ct);
            TempData["SuccessMessage"] = $"Manually triggered Ranking Risk Scanner. Updated risk scores for {count} series.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("notification-cleanup")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunNotificationCleanup(CancellationToken ct)
        {
            int count = await _jobService.ProcessNotificationCleanupAsync(ct);
            TempData["SuccessMessage"] = $"Manually triggered Notification Cleanup. Cleaned up {count} old notifications.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("monthly-earnings")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunMonthlyEarnings(CancellationToken ct)
        {
            int count = await _jobService.ProcessMonthlyEarningsAsync(ct);
            TempData["SuccessMessage"] = $"Manually triggered Monthly Earnings Calculator. Created {count} earning records.";
            return RedirectToAction(nameof(Index));
        }
    }
}
