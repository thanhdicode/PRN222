using MangaWorkflow.Application.DTOs.Board;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Domain.Constants;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Services
{
    public class BoardReviewService : IBoardReviewService
    {
        private readonly ISeriesRepository _seriesRepository;
        private readonly IBoardVoteRepository _voteRepository;
        private readonly INotificationRepository _notificationRepository;

        public BoardReviewService(
            ISeriesRepository seriesRepository,
            IBoardVoteRepository voteRepository,
            INotificationRepository notificationRepository)
        {
            _seriesRepository = seriesRepository;
            _voteRepository = voteRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<List<BoardSeriesListItemDto>> GetSubmittedSeriesAsync(CancellationToken ct = default)
        {
            var series = await _seriesRepository.GetSubmittedSeriesAsync(ct);
            return series.Select(s => new BoardSeriesListItemDto
            {
                SeriesId = s.SeriesId,
                Title = s.Title,
                Genre = s.Genre,
                CoverImageUrl = s.CoverImageUrl,
                MangakaName = s.Mangaka?.FullName ?? "",
                StatusCode = s.SeriesStatus?.StatusCode ?? "",
                StatusName = s.SeriesStatus?.StatusName ?? "",
                SubmittedAt = s.SubmittedAt,
                VoteCount = s.BoardVotes?.Count ?? 0
            }).ToList();
        }

        public async Task<BoardSeriesDetailDto?> GetSeriesForReviewAsync(Guid seriesId, Guid currentMemberId, CancellationToken ct = default)
        {
            var series = await _seriesRepository.GetByIdWithDetailsAsync(seriesId, ct);
            if (series == null) return null;

            var votes = await _voteRepository.GetBySeriesAsync(seriesId, ct);
            var currentVote = votes.FirstOrDefault(v => v.BoardMemberId == currentMemberId);

            return new BoardSeriesDetailDto
            {
                SeriesId = series.SeriesId,
                Title = series.Title,
                Genre = series.Genre,
                Description = series.Description,
                CoverImageUrl = series.CoverImageUrl,
                MangakaId = series.MangakaId,
                MangakaName = series.Mangaka?.FullName ?? "",
                StatusCode = series.SeriesStatus?.StatusCode ?? "",
                StatusName = series.SeriesStatus?.StatusName ?? "",
                SubmittedAt = series.SubmittedAt,
                ChapterCount = series.Chapters?.Count ?? 0,
                Votes = votes.Select(v => new VoteSummaryItemDto
                {
                    BoardMemberName = v.BoardMember?.FullName ?? "",
                    VoteCode = v.VoteValue?.VoteCode ?? "",
                    VoteName = v.VoteValue?.VoteName ?? "",
                    Comment = v.Comment,
                    VotedAt = v.CreatedAt
                }).ToList(),
                CurrentMemberHasVoted = currentVote != null,
                CurrentMemberVote = currentVote?.VoteValue?.VoteCode
            };
        }

        public async Task SubmitVoteAsync(SubmitVoteDto dto, Guid boardMemberId, CancellationToken ct = default)
        {
            // Check for duplicate vote
            var existingVote = await _voteRepository.GetExistingVoteAsync(dto.SeriesId, boardMemberId, ct);
            if (existingVote != null)
                throw new InvalidOperationException("You have already voted on this series.");

            // Get vote value from DB by code (never hardcode ID)
            var voteValues = await _voteRepository.GetVoteValuesAsync(ct);
            var voteValue = voteValues.FirstOrDefault(v => v.VoteCode == dto.VoteValueCode);
            if (voteValue == null)
                throw new InvalidOperationException($"Invalid vote value code: {dto.VoteValueCode}");

            var vote = new BoardVote
            {
                VoteId = Guid.NewGuid(),
                SeriesId = dto.SeriesId,
                BoardMemberId = boardMemberId,
                VoteValueId = voteValue.VoteValueId,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _voteRepository.AddAsync(vote, ct);
            await _voteRepository.SaveChangesAsync(ct);

            // Insert notification for the series Mangaka
            await InsertVoteNotificationAsync(dto.SeriesId, boardMemberId, dto.VoteValueCode, ct);
        }

        public async Task<VoteSummaryDto> GetVoteSummaryAsync(Guid seriesId, CancellationToken ct = default)
        {
            var votes = await _voteRepository.GetBySeriesAsync(seriesId, ct);
            return new VoteSummaryDto
            {
                ApproveCount = votes.Count(v => v.VoteValue?.VoteCode == "Approve"),
                RejectCount = votes.Count(v => v.VoteValue?.VoteCode == "Reject"),
                NeedRevisionCount = votes.Count(v => v.VoteValue?.VoteCode == "NeedRevision"),
                AbstainCount = votes.Count(v => v.VoteValue?.VoteCode == "Abstain"),
                TotalVotes = votes.Count
            };
        }

        private async Task InsertVoteNotificationAsync(Guid seriesId, Guid boardMemberId, string voteCode, CancellationToken ct)
        {
            // Get series to find Mangaka
            var series = await _seriesRepository.GetByIdWithDetailsAsync(seriesId, ct);
            if (series == null) return;

            // Get notification type (BoardVoteCast) — query by code
            var notificationType = await _notificationRepository.GetTypeByCodeAsync(NotificationTypeCodes.BoardVoteCast, ct);
            if (notificationType == null) return; // Graceful fallback if type not in DB

            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = series.MangakaId,
                NotificationTypeId = notificationType.NotificationTypeId,
                Title = "Board Vote Cast on Your Series",
                Message = $"A board member has voted '{voteCode}' on your series '{series.Title}'.",
                ReferenceType = "Series",
                ReferenceId = seriesId,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification, ct);
            await _notificationRepository.SaveChangesAsync(ct);
        }
    }
}
