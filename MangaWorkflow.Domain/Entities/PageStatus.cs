using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class PageStatus
{
    public int PageStatusId { get; set; }

    public string StatusCode { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public virtual ICollection<MangaPage> MangaPages { get; set; } = new List<MangaPage>();
}
