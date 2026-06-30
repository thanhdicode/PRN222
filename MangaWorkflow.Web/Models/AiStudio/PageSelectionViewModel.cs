namespace MangaWorkflow.Web.Models.AiStudio
{
    public class PageSelectionViewModel
    {
        public Guid PageId { get; set; }
        public string SeriesTitle { get; set; } = string.Empty;
        public int ChapterNumber { get; set; }
        public int PageNumber { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
