using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.DTOs.Users;

namespace MangaWorkflow.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET /Admin/Users
        public async Task<IActionResult> Index(string? keyword, string? roleCode, CancellationToken ct)
        {
            var users = await _userService.GetUsersAsync(keyword, roleCode, ct);
            var roles = await _userService.GetRolesAsync(ct);
            ViewBag.Roles = roles;
            ViewBag.Keyword = keyword;
            ViewBag.SelectedRole = roleCode;
            return View(users);
        }

        // GET /Admin/Users/Details/{id}
        public async Task<IActionResult> Details(Guid id, CancellationToken ct)
        {
            var user = await _userService.GetUserDetailAsync(id, ct);
            if (user == null) return NotFound();
            return View(user);
        }

        // GET /Admin/Users/Create
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            ViewBag.Roles = await _userService.GetRolesAsync(ct);
            return View();
        }

        // POST /Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _userService.GetRolesAsync(ct);
                return View(dto);
            }

            try
            {
                await _userService.CreateUserAsync(dto, ct);
                TempData["Success"] = $"User '{dto.Email}' created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Roles = await _userService.GetRolesAsync(ct);
                return View(dto);
            }
        }

        // GET /Admin/Users/Edit/{id}
        public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
        {
            var user = await _userService.GetUserDetailAsync(id, ct);
            if (user == null) return NotFound();

            var dto = new EditUserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                IsActive = user.IsActive
            };
            return View(dto);
        }

        // POST /Admin/Users/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, EditUserDto dto, CancellationToken ct)
        {
            if (id != dto.UserId) return BadRequest();

            if (!ModelState.IsValid)
                return View(dto);

            try
            {
                await _userService.UpdateUserAsync(dto, ct);
                TempData["Success"] = "User updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        // GET /Admin/Users/AssignRole/{id}
        public async Task<IActionResult> AssignRole(Guid id, CancellationToken ct)
        {
            var user = await _userService.GetUserDetailAsync(id, ct);
            if (user == null) return NotFound();

            ViewBag.Roles = await _userService.GetRolesAsync(ct);
            ViewBag.UserEmail = user.Email;
            return View(new AssignRoleDto { UserId = id });
        }

        // POST /Admin/Users/AssignRole/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(Guid id, AssignRoleDto dto, CancellationToken ct)
        {
            if (id != dto.UserId) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _userService.GetRolesAsync(ct);
                return View(dto);
            }

            try
            {
                await _userService.AssignRoleAsync(dto, ct);
                TempData["Success"] = "Role assigned successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.Roles = await _userService.GetRolesAsync(ct);
                return View(dto);
            }
        }

        // POST /Admin/Users/Deactivate/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        {
            await _userService.SetUserStatusAsync(id, false, ct);
            TempData["Success"] = "User deactivated.";
            return RedirectToAction(nameof(Index));
        }

        // POST /Admin/Users/Activate/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
        {
            await _userService.SetUserStatusAsync(id, true, ct);
            TempData["Success"] = "User activated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
