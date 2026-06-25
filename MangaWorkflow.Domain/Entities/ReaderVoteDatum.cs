using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class ReaderVoteDatum
{
    public Guid ReaderVoteDataId { get; set; }

    public Guid SeriesId { get; set; }

    public string IssueNumber { get; set; } = null!;

    public int VoteCount { get; set; }

    public int RankPosition { get; set; }

    public Guid ImportedByUserId { get; set; }

    public DateTime ImportedAt { get; set; }

    public string? Notes { get; set; }

    public virtual User ImportedByUser { get; set; } = null!;

    public virtual Series Series { get; set; } = null!;
}
