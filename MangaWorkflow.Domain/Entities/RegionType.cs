using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class RegionType
{
    public int RegionTypeId { get; set; }

    public string TypeCode { get; set; } = null!;

    public string TypeName { get; set; } = null!;

    public virtual ICollection<PageRegion> PageRegions { get; set; } = new List<PageRegion>();
}
