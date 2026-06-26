using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MangaWorkflow.Application.DTOs.Comments;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Web.Pages.Editor
{
    [Authorize(Roles = "TantouEditor,Admin,EditorialBoard")]
    public class PageCommentsModel : PageModel
    {
        private readonly IEditorCommentService _commentService;

        public PageCommentsModel(IEditorCommentService commentService)
        {
            _commentService = commentService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid PageId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        public List<CommentListItemDto> Comments { get; set; } = new();

        [BindProperty]
        public AddCommentDto CommentInput { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (PageId == Guid.Empty) return RedirectToPage("/Index");
            
            Comments = await _commentService.GetCommentsForPageAsync(PageId, StatusFilter);
            return Page();
        }

        public async Task<IActionResult> OnPostAddCommentAsync()
        {
            if (!ModelState.IsValid)
            {
                PageId = CommentInput.PageId;
                Comments = await _commentService.GetCommentsForPageAsync(PageId, StatusFilter);
                return Page();
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var editorId))
            {
                await _commentService.AddCommentAsync(CommentInput, editorId);
                TempData["Success"] = "Comment added.";
            }

            return RedirectToPage(new { pageId = CommentInput.PageId, statusFilter = StatusFilter });
        }

        public async Task<IActionResult> OnPostResolveCommentAsync(Guid commentId, Guid pageId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdString, out var editorId))
            {
                await _commentService.ResolveCommentAsync(commentId, editorId);
                TempData["Success"] = "Comment resolved.";
            }

            return RedirectToPage(new { pageId = pageId, statusFilter = StatusFilter });
        }
    }
}
