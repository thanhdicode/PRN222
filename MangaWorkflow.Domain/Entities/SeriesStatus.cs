using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class SeriesStatus
{
    public int SeriesStatusId { get; set; }

    public string StatusCode { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Series> Series { get; set; } = new List<Series>();
}
