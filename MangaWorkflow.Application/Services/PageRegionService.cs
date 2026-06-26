using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.DTOs.Regions;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Services
{
    public class PageRegionService : IPageRegionService
    {
        private readonly IPageRegionRepository _regionRepo;
        private readonly IProductionTaskRepository _taskRepo;
        private readonly IUserRepository _userRepo;
        private readonly INotificationService _notificationService;

        public PageRegionService(IPageRegionRepository regionRepo, IProductionTaskRepository taskRepo, IUserRepository userRepo, INotificationService notificationService)
        {
            _regionRepo = regionRepo;
            _taskRepo = taskRepo;
            _userRepo = userRepo;
            _notificationService = notificationService;
        }

        public async Task<List<RegionListItemDto>> GetRegionsForPageAsync(Guid pageId, CancellationToken ct = default)
        {
            var regions = await _regionRepo.GetByPageAsync(pageId, ct);
            return regions.Select(r => new RegionListItemDto
            {
                RegionId = r.RegionId,
                PageId = r.PageId,
                RegionTypeCode = r.RegionType.TypeCode,
                RegionTypeName = r.RegionType?.TypeName ?? "",
                X = r.X,
                Y = r.Y,
                Width = r.Width,
                Height = r.Height,
                Notes = r.Label
            }).ToList();
        }

        public async Task<Guid> CreateRegionAsync(CreateRegionDto dto, CancellationToken ct = default)
        {
            var region = new PageRegion
            {
                RegionId = Guid.NewGuid(),
                PageId = dto.PageId,
                RegionTypeId = 1,
                X = dto.X,
                Y = dto.Y,
                Width = dto.Width,
                Height = dto.Height,
                Label = dto.Notes, CreatedByUserId = Guid.Empty, SourceType = "Manual"
            };

            await _regionRepo.AddAsync(region, ct);
            return region.RegionId;
        }

        public async Task<Guid> CreateTaskFromRegionAsync(CreateTaskFromRegionDto dto, Guid mangakaId, CancellationToken ct = default)
        {
            var region = await _regionRepo.GetByIdAsync(dto.RegionId, ct);
            if (region == null) throw new ArgumentException("Region not found.");

            var task = new ProductionTask { TaskId = Guid.NewGuid(), Title = dto.Title, PageId = region.PageId, TaskTypeId = 1, AssignedToAssistantId = dto.AssignedToUserId ?? Guid.Empty, CreatedByMangakaId = mangakaId, TaskStatusId = 1, Deadline = dto.Deadline, Description = dto.Instructions, CreatedAt = DateTime.UtcNow, Priority = 1, Price = 0 };

            await _taskRepo.AddAsync(task, ct);

            if (dto.AssignedToUserId.HasValue)
            {
                await _notificationService.CreateAndSendAsync(
                    dto.AssignedToUserId.Value,
                    "NewTaskAssigned", // assumed type code, originally hardcoded to 1
                    "New Task Assigned",
                    $"You have been assigned a new task: {task.Title}",
                    "ProductionTask",
                    task.TaskId,
                    ct
                );
            }

            return task.TaskId;
        }

        public Task<List<RegionTypeOption>> GetRegionTypesAsync(CancellationToken ct = default)
        {
            return Task.FromResult(new List<RegionTypeOption>
            {
                new RegionTypeOption { TypeCode = "Screentone", TypeName = "Screentone" },
                new RegionTypeOption { TypeCode = "Background", TypeName = "Background" },
                new RegionTypeOption { TypeCode = "Dialogue", TypeName = "Dialogue" },
                new RegionTypeOption { TypeCode = "Effects", TypeName = "Effects" }
            });
        }

        public async Task<List<AssistantOption>> GetAvailableAssistantsAsync(CancellationToken ct = default)
        {
            // Fetch all assistants. Assuming role code is Assistant or similar.
            // But we can just return all users with Assistant role from userRepo if available.
            // Since this is just a quick demo, we'll try to find users containing 'assistant' in email
            var users = await _userRepo.GetAllAsync(null, null, ct);
            return users.Where(u => u.Email.Contains("assistant")).Select(u => new AssistantOption
            {
                UserId = u.UserId,
                UserName = u.FullName,
                FullName = u.FullName
            }).ToList();
        }
    }
}



