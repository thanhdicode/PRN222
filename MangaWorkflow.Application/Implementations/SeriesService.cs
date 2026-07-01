using MangaWorkflow.Domain.Entities;
using MangaWorkflow.Domain.Constants;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.DTOs.Series;

namespace MangaWorkflow.Application.Services
{
    public class SeriesService : ISeriesService
    {
        private readonly ISeriesRepository _seriesRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public SeriesService(
            ISeriesRepository seriesRepository,
            IUserRepository userRepository,
            INotificationService notificationService)
        {
            _seriesRepository = seriesRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        // --- Phase 1 legacy methods ---

        public async Task<int> CountSeriesAsync(CancellationToken cancellationToken = default)
        {
            return await _seriesRepository.CountSeriesAsync(cancellationToken);
        }

        public async Task<List<Series>> GetRecentSeriesAsync(int take = 10, CancellationToken cancellationToken = default)
        {
            return await _seriesRepository.GetRecentSeriesAsync(take, cancellationToken);
        }

        // --- Phase 2 methods ---

        public async Task<List<SeriesListItemDto>> GetSeriesForMangakaAsync(Guid mangakaId, string? statusCode, string? keyword, CancellationToken ct = default)
        {
            var series = await _seriesRepository.GetByMangakaAsync(mangakaId, statusCode, keyword, ct);
            return series.Select(MapToListItem).ToList();
        }

        public async Task<List<SeriesListItemDto>> GetAllSeriesAsync(string? statusCode, string? keyword, CancellationToken ct = default)
        {
            var series = await _seriesRepository.GetAllFilteredAsync(statusCode, keyword, ct);
            return series.Select(MapToListItem).ToList();
        }

        public async Task<SeriesDetailDto?> GetSeriesDetailAsync(Guid seriesId, CancellationToken ct = default)
        {
            var series = await _seriesRepository.GetByIdWithDetailsAsync(seriesId, ct);
            if (series == null) return null;

            return new SeriesDetailDto
            {
                SeriesId = series.SeriesId,
                Title = series.Title,
                AlternativeTitle = series.AlternativeTitle,
                Genre = series.Genre,
                Description = series.Description,
                CoverImageUrl = series.CoverImageUrl,
                StatusCode = series.SeriesStatus.StatusCode,
                StatusName = series.SeriesStatus.StatusName,
                MangakaId = series.MangakaId,
                MangakaName = series.Mangaka.FullName,
                CreatedAt = series.CreatedAt,
                SubmittedAt = series.SubmittedAt,
                ApprovedAt = series.ApprovedAt,
                ChapterCount = series.Chapters.Count,
                IsDeleted = series.IsDeleted
            };
        }

        public async Task<Guid> CreateSeriesAsync(CreateSeriesDto dto, Guid mangakaId, CancellationToken ct = default)
        {
            var statuses = await _seriesRepository.GetAllStatusesAsync(ct);
            var draftStatus = statuses.First(s => s.StatusCode == SeriesStatusCodes.Draft);

            var series = new Series
            {
                SeriesId = Guid.NewGuid(),
                Title = dto.Title,
                Genre = dto.Genre,
                Description = dto.Description,
                CoverImageUrl = dto.CoverImageUrl,
                MangakaId = mangakaId,
                SeriesStatusId = draftStatus.SeriesStatusId,
                CreatedAt = DateTime.UtcNow,
                CancellationRiskScore = 0,
                IsDeleted = false
            };

            await _seriesRepository.AddAsync(series, ct);
            await _seriesRepository.SaveChangesAsync(ct);
            return series.SeriesId;
        }

        public async Task UpdateSeriesAsync(EditSeriesDto dto, CancellationToken ct = default)
        {
            var series = await _seriesRepository.GetByIdWithDetailsAsync(dto.SeriesId, ct);
            if (series == null)
                throw new InvalidOperationException($"Series {dto.SeriesId} not found.");

            series.Title = dto.Title;
            series.Genre = dto.Genre;
            series.Description = dto.Description;
            series.CoverImageUrl = dto.CoverImageUrl;

            await _seriesRepository.SaveChangesAsync(ct);
        }

        public async Task DeleteSeriesAsync(Guid seriesId, Guid requestingUserId, bool isAdmin, CancellationToken ct = default)
        {
            var series = await _seriesRepository.GetByIdWithDetailsAsync(seriesId, ct);
            if (series == null)
                throw new InvalidOperationException($"Series {seriesId} not found.");

            if (!isAdmin && series.MangakaId != requestingUserId)
                throw new UnauthorizedAccessException("You do not own this series.");

            if (series.SeriesStatus.StatusCode != SeriesStatusCodes.Draft)
                throw new InvalidOperationException("Only Draft series can be deleted.");

            // Soft delete
            series.IsDeleted = true;
            await _seriesRepository.SaveChangesAsync(ct);
        }

        public async Task SubmitSeriesAsync(Guid seriesId, Guid mangakaId, CancellationToken ct = default)
        {
            var series = await _seriesRepository.GetByIdWithDetailsAsync(seriesId, ct);
            if (series == null)
                throw new InvalidOperationException($"Series {seriesId} not found.");

            if (series.MangakaId != mangakaId)
                throw new UnauthorizedAccessException("You do not own this series.");

            if (series.SeriesStatus.StatusCode != SeriesStatusCodes.Draft)
                throw new InvalidOperationException("Only Draft series can be submitted.");

            var statuses = await _seriesRepository.GetAllStatusesAsync(ct);
            var submittedStatus = statuses.First(s => s.StatusCode == SeriesStatusCodes.Submitted);

            series.SeriesStatusId = submittedStatus.SeriesStatusId;
            series.SubmittedAt = DateTime.UtcNow;
            await _seriesRepository.SaveChangesAsync(ct);

            // Notify all Admin and EditorialBoard members about the new submission
            var allUsers = await _userRepository.GetAllAsync(null, null, ct);
            var reviewers = allUsers.Where(u =>
                u.IsActive &&
                u.UserRoles.Any(ur =>
                    ur.Role.RoleCode == "Admin" ||
                    ur.Role.RoleCode == "EditorialBoard")).ToList();

            foreach (var reviewer in reviewers)
            {
                await _notificationService.CreateAndSendAsync(
                    reviewer.UserId,
                    "System",
                    "New Series Submitted for Review",
                    $"Series \"{series.Title}\" has been submitted by {series.Mangaka?.FullName ?? "a Mangaka"} and is awaiting board review.",
                    "Series",
                    seriesId,
                    ct
                );
            }
        }

        public async Task<List<SeriesStatus>> GetStatusesAsync(CancellationToken ct = default)
        {
            return await _seriesRepository.GetAllStatusesAsync(ct);
        }

        // --- Mapping helper ---

        private static SeriesListItemDto MapToListItem(Series s) => new SeriesListItemDto
        {
            SeriesId = s.SeriesId,
            Title = s.Title,
            Genre = s.Genre,
            StatusCode = s.SeriesStatus?.StatusCode ?? "",
            StatusName = s.SeriesStatus?.StatusName ?? "",
            CoverImageUrl = s.CoverImageUrl,
            MangakaName = s.Mangaka?.FullName,
            CreatedAt = s.CreatedAt,
            ChapterCount = s.Chapters?.Count ?? 0,
            IsDeleted = s.IsDeleted
        };
    }
}
