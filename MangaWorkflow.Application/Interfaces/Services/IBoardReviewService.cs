using MangaWorkflow.Application.DTOs.Board;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface IBoardReviewService
    {
        Task<List<BoardSeriesListItemDto>> GetSubmittedSeriesAsync(CancellationToken ct = default);
        Task<BoardSeriesDetailDto?> GetSeriesForReviewAsync(Guid seriesId, Guid currentMemberId, CancellationToken ct = default);
        Task SubmitVoteAsync(SubmitVoteDto dto, Guid boardMemberId, CancellationToken ct = default);
        Task<VoteSummaryDto> GetVoteSummaryAsync(Guid seriesId, CancellationToken ct = default);
    }
}
