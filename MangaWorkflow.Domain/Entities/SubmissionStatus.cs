using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class SubmissionStatus
{
    public int SubmissionStatusId { get; set; }

    public string StatusCode { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public virtual ICollection<TaskSubmission> TaskSubmissions { get; set; } = new List<TaskSubmission>();
}
