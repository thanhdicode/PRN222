using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class PublicationSchedule
{
    public int PublicationScheduleId { get; set; }

    public string ScheduleCode { get; set; } = null!;

    public string ScheduleName { get; set; } = null!;

    public virtual ICollection<Series> Series { get; set; } = new List<Series>();
}
