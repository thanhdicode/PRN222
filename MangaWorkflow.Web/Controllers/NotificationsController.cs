using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("GetUnread")]
        public async Task<IActionResult> GetUnread()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var notifications = await _notificationService.GetUnreadAsync(userId);
            return Ok(notifications);
        }

        [HttpPost("MarkRead/{id}")]
        public async Task<IActionResult> MarkRead(Guid id)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            await _notificationService.MarkReadAsync(id, userId);
            return Ok();
        }

        [HttpPost("MarkAllRead")]
        public async Task<IActionResult> MarkAllRead()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            await _notificationService.MarkAllReadAsync(userId);
            return Ok();
        }
    }
}
