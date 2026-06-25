using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class BoardVote
{
    public Guid VoteId { get; set; }

    public Guid SeriesId { get; set; }

    public Guid BoardMemberId { get; set; }

    public int VoteValueId { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User BoardMember { get; set; } = null!;

    public virtual Series Series { get; set; } = null!;

    public virtual VoteValue VoteValue { get; set; } = null!;
}
