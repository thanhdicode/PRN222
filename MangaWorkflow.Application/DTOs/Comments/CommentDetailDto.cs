using System;

namespace MangaWorkflow.Application.DTOs.Comments
{
    public class CommentDetailDto
    {
        public Guid CommentId { get; set; }
        public Guid PageId { get; set; }
        public string CommentText { get; set; } = "";
        public decimal? RegionX { get; set; }
        public decimal? RegionY { get; set; }
        public decimal? RegionWidth { get; set; }
        public decimal? RegionHeight { get; set; }
        public string StatusCode { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string EditorName { get; set; } = "";
    }
}
