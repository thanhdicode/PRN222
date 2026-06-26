using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Repositories
{
    public interface IPageRegionRepository
    {
        Task<PageRegion?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<PageRegion>> GetByPageAsync(Guid pageId, CancellationToken ct = default);
        Task AddAsync(PageRegion region, CancellationToken ct = default);
    }
}
