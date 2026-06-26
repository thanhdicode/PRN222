using System;
using System.ComponentModel.DataAnnotations;

namespace MangaWorkflow.Application.DTOs.Comments
{
    public class AddCommentDto
    {
        public Guid PageId { get; set; }
        [Required] [MaxLength(2000)] public string CommentText { get; set; } = "";
        public decimal? RegionX { get; set; }
        public decimal? RegionY { get; set; }
        public decimal? RegionWidth { get; set; }
        public decimal? RegionHeight { get; set; }
    }
}
