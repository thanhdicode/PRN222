using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.DTOs.Auth;

namespace MangaWorkflow.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IAuditLogService _auditLog;

        public AuthController(IAuthService authService, IAuditLogService auditLog)
        {
            _authService = authService;
            _auditLog    = auditLog;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var user = await _authService.ValidateCredentialsAsync(dto.Email, dto.Password);
            if (user == null)
            {
                // Log failed login attempt (actorUserId = null — unknown user)
                await _auditLog.LogAsync(
                    actionName: "LoginFailed",
                    entityName: "User",
                    entityId:   null,
                    actorUserId: null,
                    details: $"Failed login attempt for email: {dto.Email}");

                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(dto);
            }

            // Build claims for cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? user.Email),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleCode));
            }

            var identity  = new ClaimsIdentity(claims, "MangaWorkflowCookie");
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = dto.RememberMe,
                ExpiresUtc   = dto.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync("MangaWorkflowCookie", principal, authProperties);

            // Audit log — successful login
            await _auditLog.LogAsync(
                actionName:  "Login",
                entityName:  "User",
                entityId:    user.UserId,
                actorUserId: user.UserId,
                details: $"User {user.Email} logged in successfully.");

            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            var primaryRole = user.UserRoles.FirstOrDefault()?.Role.RoleCode;
            return primaryRole switch
            {
                "Admin"          => RedirectToAction("Index", "Dashboard", new { area = "Admin" }),
                "Mangaka"        => RedirectToAction("Index", "Dashboard", new { area = "Mangaka" }),
                "EditorialBoard" => RedirectToAction("Index", "Review",    new { area = "Board" }),
                "Assistant"      => RedirectToAction("Index", "Tasks",     new { area = "Assistant" }),
                "TantouEditor"   => RedirectToAction("Index", "Home"),
                _                => RedirectToAction("Index", "Home")
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Capture userId before signing out (claims will be cleared after)
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email     = User.FindFirstValue(ClaimTypes.Email);

            await HttpContext.SignOutAsync("MangaWorkflowCookie");

            if (Guid.TryParse(userIdStr, out var userId))
            {
                await _auditLog.LogAsync(
                    actionName:  "Logout",
                    entityName:  "User",
                    entityId:    userId,
                    actorUserId: userId,
                    details: $"User {email ?? userIdStr} logged out.");
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
