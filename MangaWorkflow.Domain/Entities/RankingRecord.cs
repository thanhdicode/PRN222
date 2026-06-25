using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class RankingRecord
{
    public Guid RankingRecordId { get; set; }

    public Guid SeriesId { get; set; }

    public string IssueNumber { get; set; } = null!;

    public int VoteCount { get; set; }

    public int RankPosition { get; set; }

    public int? PreviousRankPosition { get; set; }

    public string Trend { get; set; } = null!;

    public DateTime CalculatedAt { get; set; }

    public virtual Series Series { get; set; } = null!;
}
