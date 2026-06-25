using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class ChapterStatus
{
    public int ChapterStatusId { get; set; }

    public string StatusCode { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
}
