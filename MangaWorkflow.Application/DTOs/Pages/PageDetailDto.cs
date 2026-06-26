namespace MangaWorkflow.Application.DTOs.Pages
{
    public class PageDetailDto
    {
        public Guid PageId { get; set; }
        public Guid ChapterId { get; set; }
        public string ChapterTitle { get; set; } = "";
        public int ChapterNumber { get; set; }
        public int PageNumber { get; set; }
        public string? ImageUrl { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public long? FileSizeBytes { get; set; }
        public int VersionNumber { get; set; }
        public string StatusCode { get; set; } = "";
        public string StatusName { get; set; } = "";
        public string UploadedByName { get; set; } = "";
        public DateTime UploadedAt { get; set; }
        public int RegionCount { get; set; }
    }
}
