using System;
using System.ComponentModel.DataAnnotations;

namespace MangaWorkflow.Application.DTOs.Regions
{
    public class CreateRegionDto
    {
        public Guid PageId { get; set; }
        [Required] public string RegionTypeCode { get; set; } = "";
        [Required] public decimal X { get; set; }
        [Required] public decimal Y { get; set; }
        [Required] public decimal Width { get; set; }
        [Required] public decimal Height { get; set; }
        [MaxLength(500)] public string? Notes { get; set; }
    }
}
