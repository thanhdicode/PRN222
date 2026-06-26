using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface IEditorCommentRepository
    {
        Task<EditorComment?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<EditorComment>> GetByPageAsync(Guid pageId, string? statusCode, CancellationToken ct = default);
        Task AddAsync(EditorComment comment, CancellationToken ct = default);
        Task UpdateAsync(EditorComment comment, CancellationToken ct = default);
    }
}
