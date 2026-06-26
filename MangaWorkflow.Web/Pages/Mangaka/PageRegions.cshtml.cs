using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MangaWorkflow.Application.DTOs.Regions;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.Interfaces.Repositories;

namespace MangaWorkflow.Web.Pages.Mangaka
{
    [Authorize(Roles = "Mangaka,Admin")]
    public class PageRegionsModel : PageModel
    {
        private readonly IPageRegionService _regionService;
        private readonly IPageRepository _pageRepo; // Assuming from Phase 2

        public PageRegionsModel(IPageRegionService regionService, IPageRepository pageRepo)
        {
            _regionService = regionService;
            _pageRepo = pageRepo;
        }

        [BindProperty(SupportsGet = true)]
        public Guid PageId { get; set; }

        public List<RegionListItemDto> Regions { get; set; } = new();
        public List<RegionTypeOption> RegionTypes { get; set; } = new();
        public List<AssistantOption> Assistants { get; set; } = new();

        [BindProperty]
        public CreateRegionDto RegionInput { get; set; } = new();
        
        [BindProperty]
        public CreateTaskFromRegionDto TaskInput { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (PageId == Guid.Empty) return RedirectToPage("/Index");

            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAddRegionAsync()
        {
            if (RegionInput.X < 0 || RegionInput.Y < 0 || RegionInput.Width <= 0 || RegionInput.Height <= 0)
            {
                ModelState.AddModelError("", "Coordinates and dimensions must be positive values.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                return Page();
            }

            await _regionService.CreateRegionAsync(RegionInput);
            TempData["Success"] = "Region added.";
            return RedirectToPage(new { pageId = RegionInput.PageId });
        }

        public async Task<IActionResult> OnPostCreateTaskAsync(Guid pageId)
        {
            if (!ModelState.IsValid)
            {
                PageId = pageId;
                await LoadDataAsync();
                return Page();
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var mangakaId))
            {
                await _regionService.CreateTaskFromRegionAsync(TaskInput, mangakaId);
                TempData["Success"] = "Task created and assigned.";
            }

            return RedirectToPage(new { pageId = pageId });
        }

        private async Task LoadDataAsync()
        {
            Regions = await _regionService.GetRegionsForPageAsync(PageId);
            RegionTypes = await _regionService.GetRegionTypesAsync();
            Assistants = await _regionService.GetAvailableAssistantsAsync();
        }
    }
}
