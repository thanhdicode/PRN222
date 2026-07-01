using System.Security.Claims;
using MangaWorkflow.Application.Interfaces;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Domain.Entities;
using MangaWorkflow.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace MangaWorkflow.Tests
{
    public class AiStudioControllerTests
    {
        [Fact]
        public async Task Analyze_AllowsMangakaOwnedPageUsingExistingSeriesLookup()
        {
            var userId = Guid.NewGuid();
            var pageId = Guid.NewGuid();
            var seriesId = Guid.NewGuid();

            var aiStudioService = new Mock<IAiStudioService>();
            var pageRepository = new Mock<IPageRepository>();
            var seriesRepository = new Mock<ISeriesRepository>();
            var chapterRepository = new Mock<IChapterRepository>();
            var userRepository = new Mock<IUserRepository>();

            pageRepository
                .Setup(r => r.GetByIdWithDetailsAsync(pageId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MangaPage
                {
                    PageId = pageId,
                    ImageUrl = "/uploads/page.png",
                    Chapter = new Chapter { SeriesId = seriesId }
                });

            seriesRepository
                .Setup(r => r.GetByIdWithDetailsAsync(seriesId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Series { SeriesId = seriesId, MangakaId = userId });

            aiStudioService
                .Setup(s => s.RunSegmentationAsync(pageId, userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AiInferenceRequest { InferenceRequestId = Guid.NewGuid(), PageId = pageId });

            aiStudioService
                .Setup(s => s.GetDetectedRegionsAsync(pageId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AiDetectedRegion>());

            var controller = new AiStudioController(
                aiStudioService.Object,
                pageRepository.Object,
                seriesRepository.Object,
                chapterRepository.Object,
                userRepository.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                        new Claim(ClaimTypes.Role, "Mangaka")
                    }, "TestAuth"))
                }
            };

            var result = await controller.Analyze(pageId.ToString());

            Assert.IsType<ViewResult>(result);
            seriesRepository.Verify(
                r => r.GetByIdWithDetailsAsync(seriesId, It.IsAny<CancellationToken>()),
                Times.Once);
            aiStudioService.Verify(
                s => s.RunSegmentationAsync(pageId, userId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
