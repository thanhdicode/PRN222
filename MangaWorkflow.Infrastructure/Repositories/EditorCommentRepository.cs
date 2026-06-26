using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class EditorCommentRepository : IEditorCommentRepository
    {
        private readonly MangaWorkflowDbContext _context;
        public EditorCommentRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<EditorComment?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.EditorComments.FindAsync(new object[] { id }, ct);
        }

        public async Task<List<EditorComment>> GetByPageAsync(Guid pageId, string? statusCode, CancellationToken ct = default)
        {
            var query = _context.EditorComments
                .Include(c => c.Editor)
                .Where(c => c.PageId == pageId);
                
            if (!string.IsNullOrEmpty(statusCode))
            {
                query = query.Where(c => c.CommentStatus.StatusCode == statusCode);
            }
            
            return await query.OrderByDescending(c => c.CreatedAt).ToListAsync(ct);
        }

        public async Task AddAsync(EditorComment comment, CancellationToken ct = default)
        {
            _context.EditorComments.Add(comment);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(EditorComment comment, CancellationToken ct = default)
        {
            _context.EditorComments.Update(comment);
            await _context.SaveChangesAsync(ct);
        }
    }
}

