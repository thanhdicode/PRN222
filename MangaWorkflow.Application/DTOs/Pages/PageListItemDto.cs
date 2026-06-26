namespace MangaWorkflow.Application.DTOs.Pages
{
    public class PageListItemDto
    {
        public Guid PageId { get; set; }
        public Guid ChapterId { get; set; }
        public int PageNumber { get; set; }
        public string? ImageUrl { get; set; }
        public string StatusCode { get; set; } = "";
        public string StatusName { get; set; } = "";
        public int VersionNumber { get; set; }
        public int RegionCount { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
