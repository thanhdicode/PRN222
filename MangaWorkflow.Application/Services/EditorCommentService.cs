using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.DTOs.Comments;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Services
{
    public class EditorCommentService : IEditorCommentService
    {
        private readonly IEditorCommentRepository _commentRepo;
        private readonly INotificationRepository _notificationRepo;
        // In real app we might need IPageRepository to find Mangaka to notify, mocking that for now

        public EditorCommentService(IEditorCommentRepository commentRepo, INotificationRepository notificationRepo)
        {
            _commentRepo = commentRepo;
            _notificationRepo = notificationRepo;
        }

        public async Task<List<CommentListItemDto>> GetCommentsForPageAsync(Guid pageId, string? statusFilter, CancellationToken ct = default)
        {
            var comments = await _commentRepo.GetByPageAsync(pageId, statusFilter, ct);
            return comments.Select(c => new CommentListItemDto
            {
                CommentId = c.CommentId,
                CommentText = c.CommentText,
                RegionX = c.X,
                RegionY = c.Y,
                StatusCode = c.CommentStatus.StatusCode,
                CreatedAt = c.CreatedAt,
                EditorName = c.Editor?.FullName ?? ""
            }).ToList();
        }

        public async Task<CommentDetailDto?> GetCommentDetailAsync(Guid commentId, CancellationToken ct = default)
        {
            var comment = await _commentRepo.GetByIdAsync(commentId, ct);
            if (comment == null) return null;

            return new CommentDetailDto
            {
                CommentId = comment.CommentId,
                PageId = comment.PageId,
                CommentText = comment.CommentText,
                RegionX = comment.X,
                RegionY = comment.Y,
                RegionWidth = comment.Width,
                RegionHeight = comment.Height,
                StatusCode = comment.CommentStatus.StatusCode,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task AddCommentAsync(AddCommentDto dto, Guid editorId, CancellationToken ct = default)
        {
            var comment = new EditorComment { CommentId = Guid.NewGuid(), PageId = dto.PageId, EditorId = editorId, X = dto.RegionX, Y = dto.RegionY, CommentText = dto.CommentText, CommentStatusId = 1, CreatedAt = DateTime.UtcNow };

            await _commentRepo.AddAsync(comment, ct);
            
            // Notification would normally go to Mangaka here
        }

        public async Task ResolveCommentAsync(Guid commentId, Guid editorId, CancellationToken ct = default)
        {
            var comment = await _commentRepo.GetByIdAsync(commentId, ct);
            if (comment != null)
            {
                comment.CommentStatus.StatusCode = "Resolved";
                comment.ResolvedAt = DateTime.UtcNow;
                comment.ResolvedByUserId = editorId;
                await _commentRepo.UpdateAsync(comment, ct);
            }
        }
    }
}






