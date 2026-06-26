using System;

namespace MangaWorkflow.Application.DTOs.Comments
{
    public class CommentListItemDto
    {
        public Guid CommentId { get; set; }
        public string CommentText { get; set; } = "";
        public decimal? RegionX { get; set; }
        public decimal? RegionY { get; set; }
        public string StatusCode { get; set; } = ""; // Open, Resolved
        public DateTime CreatedAt { get; set; }
        public string EditorName { get; set; } = "";
    }
}
