using MangaWorkflow.Application.DTOs.Chapters;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Domain.Constants;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Services
{
    public class ChapterService : IChapterService
    {
        private readonly IChapterRepository _chapterRepository;

        public ChapterService(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        public async Task<List<ChapterListItemDto>> GetChaptersBySeriesAsync(Guid seriesId, CancellationToken ct = default)
        {
            var chapters = await _chapterRepository.GetBySeriesAsync(seriesId, ct);
            return chapters.Select(c => new ChapterListItemDto
            {
                ChapterId = c.ChapterId,
                SeriesId = c.SeriesId,
                SeriesTitle = c.Series?.Title ?? "",
                ChapterNumber = c.ChapterNumber,
                Title = c.Title,
                StatusCode = c.ChapterStatus?.StatusCode ?? "",
                StatusName = c.ChapterStatus?.StatusName ?? "",
                Deadline = c.Deadline,
                CreatedAt = c.CreatedAt,
                PageCount = c.MangaPages?.Count ?? 0
            }).ToList();
        }

        public async Task<ChapterDetailDto?> GetChapterDetailAsync(Guid chapterId, CancellationToken ct = default)
        {
            var chapter = await _chapterRepository.GetByIdWithDetailsAsync(chapterId, ct);
            if (chapter == null) return null;

            return new ChapterDetailDto
            {
                ChapterId = chapter.ChapterId,
                SeriesId = chapter.SeriesId,
                SeriesTitle = chapter.Series?.Title ?? "",
                ChapterNumber = chapter.ChapterNumber,
                Title = chapter.Title,
                Synopsis = chapter.Summary,
                StatusCode = chapter.ChapterStatus?.StatusCode ?? "",
                StatusName = chapter.ChapterStatus?.StatusName ?? "",
                Deadline = chapter.Deadline,
                CreatedAt = chapter.CreatedAt,
                CompletedAt = chapter.CompletedAt,
                TotalPages = chapter.MangaPages?.Count ?? 0,
                TotalTasks = 0, // Populated by task counts in later phases
                ApprovedTasks = 0,
                OverdueTasks = 0
            };
        }

        public async Task<Guid> CreateChapterAsync(CreateChapterDto dto, CancellationToken ct = default)
        {
            var statuses = await _chapterRepository.GetAllStatusesAsync(ct);
            var draftStatus = statuses.First(s => s.StatusCode == ChapterStatusCodes.Draft);

            var chapter = new Chapter
            {
                ChapterId = Guid.NewGuid(),
                SeriesId = dto.SeriesId,
                ChapterNumber = dto.ChapterNumber,
                Title = dto.Title,
                Summary = dto.Synopsis,
                Deadline = dto.Deadline,
                ChapterStatusId = draftStatus.ChapterStatusId,
                CreatedAt = DateTime.UtcNow
            };

            await _chapterRepository.AddAsync(chapter, ct);
            await _chapterRepository.SaveChangesAsync(ct);
            return chapter.ChapterId;
        }

        public async Task UpdateChapterAsync(EditChapterDto dto, CancellationToken ct = default)
        {
            var chapter = await _chapterRepository.GetByIdWithDetailsAsync(dto.ChapterId, ct);
            if (chapter == null)
                throw new InvalidOperationException($"Chapter {dto.ChapterId} not found.");

            chapter.ChapterNumber = dto.ChapterNumber;
            chapter.Title = dto.Title;
            chapter.Summary = dto.Synopsis;
            chapter.Deadline = dto.Deadline;

            await _chapterRepository.SaveChangesAsync(ct);
        }

        public async Task DeleteChapterAsync(Guid chapterId, CancellationToken ct = default)
        {
            var chapter = await _chapterRepository.GetByIdWithDetailsAsync(chapterId, ct);
            if (chapter == null)
                throw new InvalidOperationException($"Chapter {chapterId} not found.");

            if (chapter.ChapterStatus?.StatusCode != ChapterStatusCodes.Draft)
                throw new InvalidOperationException("Only Draft chapters can be deleted.");

            // EF Core remove — actual delete (chapters don't have IsDeleted)
            // TODO: Consider adding soft-delete to chapters in later phases
            throw new InvalidOperationException("Chapter deletion not supported without soft-delete. Mark chapter as cancelled instead.");
        }

        public async Task StartProductionAsync(Guid chapterId, CancellationToken ct = default)
        {
            var chapter = await _chapterRepository.GetByIdWithDetailsAsync(chapterId, ct);
            if (chapter == null)
                throw new InvalidOperationException($"Chapter {chapterId} not found.");

            if (chapter.ChapterStatus?.StatusCode != ChapterStatusCodes.Draft)
                throw new InvalidOperationException("Only Draft chapters can be started.");

            var statuses = await _chapterRepository.GetAllStatusesAsync(ct);
            var inProductionStatus = statuses.First(s => s.StatusCode == ChapterStatusCodes.InProduction);

            chapter.ChapterStatusId = inProductionStatus.ChapterStatusId;
            await _chapterRepository.SaveChangesAsync(ct);
        }
    }
}
