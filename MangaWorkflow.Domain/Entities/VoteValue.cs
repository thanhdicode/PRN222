using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class VoteValue
{
    public int VoteValueId { get; set; }

    public string VoteCode { get; set; } = null!;

    public string VoteName { get; set; } = null!;

    public virtual ICollection<BoardVote> BoardVotes { get; set; } = new List<BoardVote>();
}
