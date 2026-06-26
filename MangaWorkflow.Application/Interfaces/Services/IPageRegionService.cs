using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.DTOs.Regions;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface IPageRegionService
    {
        Task<List<RegionListItemDto>> GetRegionsForPageAsync(Guid pageId, CancellationToken ct = default);
        Task<Guid> CreateRegionAsync(CreateRegionDto dto, CancellationToken ct = default);
        Task<Guid> CreateTaskFromRegionAsync(CreateTaskFromRegionDto dto, Guid mangakaId, CancellationToken ct = default);
        Task<List<RegionTypeOption>> GetRegionTypesAsync(CancellationToken ct = default);
        Task<List<AssistantOption>> GetAvailableAssistantsAsync(CancellationToken ct = default);
    }
}
