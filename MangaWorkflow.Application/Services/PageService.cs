using MangaWorkflow.Application.DTOs.Pages;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Services
{
    public class PageService : IPageService
    {
        private readonly IPageRepository _pageRepository;
        private readonly IChapterRepository _chapterRepository;

        // Allowed image extensions for file upload
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

        public PageService(IPageRepository pageRepository, IChapterRepository chapterRepository)
        {
            _pageRepository = pageRepository;
            _chapterRepository = chapterRepository;
        }

        public async Task<List<PageListItemDto>> GetPagesByChapterAsync(Guid chapterId, CancellationToken ct = default)
        {
            var pages = await _pageRepository.GetByChapterAsync(chapterId, ct);
            return pages.OrderBy(p => p.PageNumber).Select(p => new PageListItemDto
            {
                PageId = p.PageId,
                ChapterId = p.ChapterId,
                PageNumber = p.PageNumber,
                ImageUrl = p.ImageUrl,
                StatusCode = p.PageStatus?.StatusCode ?? "",
                StatusName = p.PageStatus?.StatusName ?? "",
                VersionNumber = p.VersionNo,
                RegionCount = p.PageRegions?.Count ?? 0,
                UploadedAt = p.UploadedAt
            }).ToList();
        }

        public async Task<PageDetailDto?> GetPageDetailAsync(Guid pageId, CancellationToken ct = default)
        {
            var page = await _pageRepository.GetByIdWithDetailsAsync(pageId, ct);
            if (page == null) return null;

            return new PageDetailDto
            {
                PageId = page.PageId,
                ChapterId = page.ChapterId,
                ChapterTitle = page.Chapter?.Title ?? "",
                ChapterNumber = page.Chapter?.ChapterNumber ?? 0,
                PageNumber = page.PageNumber,
                ImageUrl = page.ImageUrl,
                FileName = page.FileName,
                ContentType = page.ContentType,
                FileSizeBytes = page.FileSizeBytes,
                VersionNumber = page.VersionNo,
                StatusCode = page.PageStatus?.StatusCode ?? "",
                StatusName = page.PageStatus?.StatusName ?? "",
                UploadedByName = page.UploadedByUser?.FullName ?? "",
                UploadedAt = page.UploadedAt,
                RegionCount = page.PageRegions?.Count ?? 0
            };
        }

        public async Task<Guid> CreatePageAsync(CreatePageDto dto, Guid uploadedByUserId, string wwwRootPath, CancellationToken ct = default)
        {
            string imageUrl;
            string? fileName = null;
            string? contentType = null;
            long? fileSizeBytes = null;

            if (dto.UploadedFile != null && dto.UploadedFile.Length > 0)
            {
                // Validate extension
                var ext = Path.GetExtension(dto.UploadedFile.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(ext))
                    throw new InvalidOperationException($"Invalid file type. Allowed: {string.Join(", ", AllowedExtensions)}");

                // Validate size
                if (dto.UploadedFile.Length > MaxFileSizeBytes)
                    throw new InvalidOperationException("File size exceeds 10 MB limit.");

                // Save to wwwroot/uploads/pages/
                var uploadsDir = Path.Combine(wwwRootPath, "uploads", "pages");
                Directory.CreateDirectory(uploadsDir);

                var uniqueFileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsDir, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.UploadedFile.CopyToAsync(stream, ct);
                }

                imageUrl = $"/uploads/pages/{uniqueFileName}";
                fileName = dto.UploadedFile.FileName;
                contentType = dto.UploadedFile.ContentType;
                fileSizeBytes = dto.UploadedFile.Length;
            }
            else if (!string.IsNullOrWhiteSpace(dto.FileUrl))
            {
                imageUrl = dto.FileUrl;
            }
            else
            {
                throw new InvalidOperationException("Either an uploaded file or a URL must be provided.");
            }

            // Get default page status (Draft)
            var pageStatus = await _pageRepository.GetStatusByCodeAsync("Draft", ct);
            if (pageStatus == null)
                throw new InvalidOperationException("Default page status 'Draft' not found.");

            var page = new MangaPage
            {
                PageId = Guid.NewGuid(),
                ChapterId = dto.ChapterId,
                PageNumber = dto.PageNumber,
                ImageUrl = imageUrl,
                FileName = fileName,
                ContentType = contentType,
                FileSizeBytes = fileSizeBytes,
                VersionNo = 1,
                PageStatusId = pageStatus.PageStatusId,
                UploadedByUserId = uploadedByUserId,
                UploadedAt = DateTime.UtcNow
            };

            await _pageRepository.AddAsync(page, ct);
            await _pageRepository.SaveChangesAsync(ct);
            return page.PageId;
        }

        public async Task DeletePageAsync(Guid pageId, CancellationToken ct = default)
        {
            var page = await _pageRepository.GetByIdWithDetailsAsync(pageId, ct);
            if (page == null)
                throw new InvalidOperationException($"Page {pageId} not found.");

            // TODO: Physical file deletion if needed
            // For now, just remove the entity
            throw new InvalidOperationException("Page deletion via service not implemented — use repository directly if needed.");
        }
    }
}
