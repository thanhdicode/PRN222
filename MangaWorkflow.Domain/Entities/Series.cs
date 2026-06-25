using System;
using System.Collections.Generic;

namespace MangaWorkflow.Domain.Entities;

public partial class Series
{
    public Guid SeriesId { get; set; }

    public string Title { get; set; } = null!;

    public string? AlternativeTitle { get; set; }

    public string? Description { get; set; }

    public string? Genre { get; set; }

    public string? CoverImageUrl { get; set; }

    public Guid MangakaId { get; set; }

    public Guid? TantouEditorId { get; set; }

    public int SeriesStatusId { get; set; }

    public int? PublicationScheduleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime? RejectedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public decimal CancellationRiskScore { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<BoardVote> BoardVotes { get; set; } = new List<BoardVote>();

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();

    public virtual User Mangaka { get; set; } = null!;

    public virtual ICollection<Manuscript> Manuscripts { get; set; } = new List<Manuscript>();

    public virtual PublicationSchedule? PublicationSchedule { get; set; }

    public virtual ICollection<PublishingDecision> PublishingDecisions { get; set; } = new List<PublishingDecision>();

    public virtual ICollection<RankingRecord> RankingRecords { get; set; } = new List<RankingRecord>();

    public virtual ICollection<ReaderVoteDatum> ReaderVoteData { get; set; } = new List<ReaderVoteDatum>();

    public virtual SeriesStatus SeriesStatus { get; set; } = null!;

    public virtual ICollection<SeriesTeamMember> SeriesTeamMembers { get; set; } = new List<SeriesTeamMember>();

    public virtual User? TantouEditor { get; set; }
}
