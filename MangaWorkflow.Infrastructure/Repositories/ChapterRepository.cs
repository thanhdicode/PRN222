using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class ChapterRepository : IChapterRepository
    {
        private readonly MangaWorkflowDbContext _context;
        public ChapterRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<int> CountChaptersAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Chapters.CountAsync(cancellationToken);
        }
    }
}
