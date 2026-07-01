using MangaWorkflow.Application.DTOs.Board;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Domain.Constants;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Services
{
    public class BoardReviewService : IBoardReviewService
    {
        // Minimum votes for a decision to be finalised automatically.
        // SET 1 does not fix an exact number, so we use 3 as a safe default.
        private const int VoteThreshold = 3;

        private readonly ISeriesRepository _seriesRepository;
        private readonly IBoardVoteRepository _voteRepository;
        private readonly INotificationService _notificationService;

        public BoardReviewService(
            ISeriesRepository seriesRepository,
            IBoardVoteRepository voteRepository,
            INotificationService notificationService)
        {
            _seriesRepository = seriesRepository;
            _voteRepository = voteRepository;
            _notificationService = notificationService;
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
            // Guard: duplicate vote
            var existingVote = await _voteRepository.GetExistingVoteAsync(dto.SeriesId, boardMemberId, ct);
            if (existingVote != null)
                throw new InvalidOperationException("You have already voted on this series.");

            // Resolve vote value from DB by code — never hardcode IDs
            var voteValues = await _voteRepository.GetVoteValuesAsync(ct);
            var voteValue = voteValues.FirstOrDefault(v => v.VoteCode == dto.VoteValueCode)
                ?? throw new InvalidOperationException($"Invalid vote value code: {dto.VoteValueCode}");

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

            // Notify Mangaka a vote was cast
            await InsertVoteNotificationAsync(dto.SeriesId, boardMemberId, dto.VoteValueCode, ct);

            // Check if enough votes have accumulated to auto-finalise the series
            await TryFinaliseSeriesAsync(dto.SeriesId, ct);
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

        // -----------------------------------------------------------------------
        // Private helpers
        // -----------------------------------------------------------------------

        /// <summary>
        /// After each vote, check whether a majority decision has been reached and
        /// update the Series status + notify the Mangaka accordingly.
        /// </summary>
        private async Task TryFinaliseSeriesAsync(Guid seriesId, CancellationToken ct)
        {
            var series = await _seriesRepository.GetByIdWithDetailsAsync(seriesId, ct);
            if (series == null) return;

            // Only auto-finalise a series that is still under review (Submitted or UnderReview)
            var currentCode = series.SeriesStatus?.StatusCode;
            if (currentCode != SeriesStatusCodes.Submitted && currentCode != SeriesStatusCodes.UnderReview)
                return;

            var votes = await _voteRepository.GetBySeriesAsync(seriesId, ct);
            int approveCount = votes.Count(v => v.VoteValue?.VoteCode == "Approve");
            int rejectCount  = votes.Count(v => v.VoteValue?.VoteCode == "Reject");

            string? newStatusCode = null;
            string notificationTypeCode;
            string notificationTitle;
            string notificationMessage;

            if (approveCount >= VoteThreshold)
            {
                newStatusCode = SeriesStatusCodes.Approved;
                notificationTypeCode = NotificationTypeCodes.SeriesApproved;
                notificationTitle = "Your Series Has Been Approved!";
                notificationMessage = $"Congratulations! Your series \"{series.Title}\" has been approved by the editorial board.";
            }
            else if (rejectCount >= VoteThreshold)
            {
                newStatusCode = SeriesStatusCodes.Rejected;
                notificationTypeCode = NotificationTypeCodes.SeriesRejected;
                notificationTitle = "Your Series Has Been Rejected";
                notificationMessage = $"We're sorry. Your series \"{series.Title}\" has been rejected by the editorial board.";
            }
            else
            {
                // Not yet enough votes for a decision — update to UnderReview if still Submitted
                if (currentCode == SeriesStatusCodes.Submitted)
                {
                    var statuses = await _seriesRepository.GetAllStatusesAsync(ct);
                    var underReviewStatus = statuses.FirstOrDefault(s => s.StatusCode == SeriesStatusCodes.UnderReview);
                    if (underReviewStatus != null)
                    {
                        series.SeriesStatusId = underReviewStatus.SeriesStatusId;
                        await _seriesRepository.SaveChangesAsync(ct);
                    }
                }
                return;
            }

            // Apply final decision
            var allStatuses = await _seriesRepository.GetAllStatusesAsync(ct);
            var finalStatus = allStatuses.First(s => s.StatusCode == newStatusCode);
            series.SeriesStatusId = finalStatus.SeriesStatusId;

            if (newStatusCode == SeriesStatusCodes.Approved)
                series.ApprovedAt = DateTime.UtcNow;

            await _seriesRepository.SaveChangesAsync(ct);

            // Notify Mangaka of the final decision
            await _notificationService.CreateAndSendAsync(
                series.MangakaId,
                notificationTypeCode,
                notificationTitle,
                notificationMessage,
                "Series",
                seriesId,
                ct
            );
        }

        private async Task InsertVoteNotificationAsync(Guid seriesId, Guid boardMemberId, string voteCode, CancellationToken ct)
        {
            var series = await _seriesRepository.GetByIdWithDetailsAsync(seriesId, ct);
            if (series == null) return;

            await _notificationService.CreateAndSendAsync(
                series.MangakaId,
                NotificationTypeCodes.BoardVoteCast,
                "Board Vote Cast on Your Series",
                $"A board member has voted '{voteCode}' on your series '{series.Title}'.",
                "Series",
                seriesId,
                ct
            );
        }
    }
}
