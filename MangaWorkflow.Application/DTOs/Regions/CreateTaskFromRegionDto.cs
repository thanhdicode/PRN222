using System;
using System.ComponentModel.DataAnnotations;

namespace MangaWorkflow.Application.DTOs.Regions
{
    public class CreateTaskFromRegionDto
    {
        public Guid RegionId { get; set; }
        [Required] public string TaskTypeCode { get; set; } = "";
        [Required] [MaxLength(300)] public string Title { get; set; } = "";
        public Guid? AssignedToUserId { get; set; }
        public DateTime? Deadline { get; set; }
        [MaxLength(2000)] public string? Instructions { get; set; }
    }
}
