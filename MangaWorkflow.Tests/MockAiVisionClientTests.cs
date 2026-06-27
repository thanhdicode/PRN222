using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Infrastructure.Ai;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using Xunit;

namespace MangaWorkflow.Tests
{
    public class MockAiVisionClientTests
    {
        [Fact]
        public async Task SegmentPageAsync_ApiCallFails_ReturnsMockFallbackData()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                // Return a 500 error to simulate a failure
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost:8001")
            };

            var logger = NullLogger<MockAiVisionClient>.Instance;
            var client = new MockAiVisionClient(httpClient, logger);

            // Act
            var result = await client.SegmentPageAsync("test-page-id");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test-page-id", result.PageId);
            Assert.Equal("mock-yolo-seg-fallback", result.ModelName);
            Assert.Equal(3, result.Detections.Count);
            
            // Verify handler was called
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post
                    && req.RequestUri != null
                    && req.RequestUri.ToString() == "http://localhost:8001/api/segment"),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
