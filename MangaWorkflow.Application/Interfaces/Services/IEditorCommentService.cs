using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.DTOs.Comments;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface IEditorCommentService
    {
        Task<List<CommentListItemDto>> GetCommentsForPageAsync(Guid pageId, string? statusFilter, CancellationToken ct = default);
        Task<CommentDetailDto?> GetCommentDetailAsync(Guid commentId, CancellationToken ct = default);
        Task AddCommentAsync(AddCommentDto dto, Guid editorId, CancellationToken ct = default);
        Task ResolveCommentAsync(Guid commentId, Guid editorId, CancellationToken ct = default);
    }
}
