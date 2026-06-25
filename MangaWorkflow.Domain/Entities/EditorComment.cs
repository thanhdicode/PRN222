using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class EditorComment
{
    public Guid CommentId { get; set; }

    public Guid PageId { get; set; }

    public Guid EditorId { get; set; }

    public decimal? X { get; set; }

    public decimal? Y { get; set; }

    public decimal? Width { get; set; }

    public decimal? Height { get; set; }

    public string CommentText { get; set; } = null!;

    public int CommentStatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public Guid? ResolvedByUserId { get; set; }

    public virtual CommentStatus CommentStatus { get; set; } = null!;

    public virtual User Editor { get; set; } = null!;

    public virtual MangaPage Page { get; set; } = null!;

    public virtual User? ResolvedByUser { get; set; }
}
