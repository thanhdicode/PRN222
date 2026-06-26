using System.Collections.Generic;

namespace MangaWorkflow.Application.DTOs.Dashboard
{
    public class MangakaDashboardDto
    {
        public int ActiveSeriesCount { get; set; }
        public int PendingReviewCount { get; set; }
        public List<ChapterProgressItemDto> ChapterProgress { get; set; } = new();
    }
}
