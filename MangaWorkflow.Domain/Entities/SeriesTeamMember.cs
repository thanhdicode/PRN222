using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class SeriesTeamMember
{
    public Guid SeriesTeamMemberId { get; set; }

    public Guid SeriesId { get; set; }

    public Guid UserId { get; set; }

    public string RoleInSeries { get; set; } = null!;

    public DateTime JoinedAt { get; set; }

    public bool IsActive { get; set; }

    public virtual Series Series { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
