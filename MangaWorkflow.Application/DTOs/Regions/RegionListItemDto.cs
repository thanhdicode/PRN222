using System;

namespace MangaWorkflow.Application.DTOs.Regions
{
    public class RegionListItemDto
    {
        public Guid RegionId { get; set; }
        public Guid PageId { get; set; }
        public string RegionTypeCode { get; set; } = "";
        public string RegionTypeName { get; set; } = "";
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string? Notes { get; set; }
    }
}
