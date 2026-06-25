using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class VwSeriesLatestRanking
{
    public Guid SeriesId { get; set; }

    public string SeriesTitle { get; set; } = null!;

    public string? IssueNumber { get; set; }

    public int? VoteCount { get; set; }

    public int? RankPosition { get; set; }

    public int? PreviousRankPosition { get; set; }

    public string? Trend { get; set; }

    public DateTime? CalculatedAt { get; set; }

    public decimal CancellationRiskScore { get; set; }
}
