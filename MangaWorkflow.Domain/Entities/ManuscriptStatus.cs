using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class ManuscriptStatus
{
    public int ManuscriptStatusId { get; set; }

    public string StatusCode { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Manuscript> Manuscripts { get; set; } = new List<Manuscript>();
}
