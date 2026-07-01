using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.Services;
using MangaWorkflow.Application.DTOs.Notifications;
using MangaWorkflow.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MangaWorkflow.Tests
{
    public class BackgroundJobServiceTests
    {
        [Fact]
        public async Task ProcessRankingRisksAsync_ClampsCancellationRiskScoreAt100()
        {
            var seriesId = Guid.NewGuid();
            var mangakaId = Guid.NewGuid();
            var updatedScores = new List<decimal>();

            var taskService = new Mock<ITaskWorkflowService>();
            var jobQueries = new Mock<IBackgroundJobQueriesRepository>();
            var notifications = new Mock<INotificationService>();

            jobQueries
                .Setup(r => r.GetSeriesForRankingRiskAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Series>
                {
                    new()
                    {
                        SeriesId = seriesId,
                        MangakaId = mangakaId,
                        Title = "High Risk Demo Series",
                        CancellationRiskScore = 96m,
                        RankingRecords = new List<RankingRecord>
                        {
                            new() { SeriesId = seriesId, RankPosition = 12, CalculatedAt = DateTime.UtcNow },
                            new() { SeriesId = seriesId, RankPosition = 5, CalculatedAt = DateTime.UtcNow.AddDays(-7) }
                        }
                    }
                });

            jobQueries
                .Setup(r => r.UpdateSeriesCancellationRiskAsync(seriesId, It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
                .Callback<Guid, decimal, CancellationToken>((_, score, _) => updatedScores.Add(score))
                .Returns(Task.CompletedTask);

            notifications
                .Setup(s => s.CreateAndSendAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new NotificationDto());

            var service = new BackgroundJobService(
                taskService.Object,
                jobQueries.Object,
                notifications.Object,
                NullLogger<BackgroundJobService>.Instance);

            var processed = await service.ProcessRankingRisksAsync();

            Assert.Equal(1, processed);
            Assert.Equal(100m, Assert.Single(updatedScores));
        }
    }
}
