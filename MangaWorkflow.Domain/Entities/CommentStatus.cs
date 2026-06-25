using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class CommentStatus
{
    public int CommentStatusId { get; set; }

    public string StatusCode { get; set; } = null!;

    public string StatusName { get; set; } = null!;

    public virtual ICollection<EditorComment> EditorComments { get; set; } = new List<EditorComment>();
}
