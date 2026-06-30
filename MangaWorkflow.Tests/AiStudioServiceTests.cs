using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MangaWorkflow.Application.DTOs.Ai;
using MangaWorkflow.Application.Interfaces;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.Services;
using MangaWorkflow.Domain.Entities;
using Moq;
using Xunit;

namespace MangaWorkflow.Tests
{
    /// <summary>
    /// Unit tests for AiStudioService (Application layer).
    /// Covers: region type mapping, AcceptRegion, ApproveTaskSuggestion, RejectTaskSuggestion.
    /// </summary>
    public class AiStudioServiceTests
    {
        private readonly Mock<IAiInferenceRepository> _inferenceRepo = new();
        private readonly Mock<IAiDetectedRegionRepository> _detectedRegionRepo = new();
        private readonly Mock<IAiTaskSuggestionRepository> _suggestionRepo = new();
        private readonly Mock<IPageRepository> _pageRepo = new();
        private readonly Mock<IPageRegionRepository> _pageRegionRepo = new();
        private readonly Mock<ITaskTypeRepository> _taskTypeRepo = new();
        private readonly Mock<ITaskStatusRepository> _taskStatusRepo = new();
        private readonly Mock<IProductionTaskRepository> _productionTaskRepo = new();
        private readonly Mock<IAiVisionClient> _aiClient = new();

        private AiStudioService CreateService()
        {
            return new AiStudioService(
                _inferenceRepo.Object,
                _detectedRegionRepo.Object,
                _suggestionRepo.Object,
                _pageRepo.Object,
                _pageRegionRepo.Object,
                _taskTypeRepo.Object,
                _taskStatusRepo.Object,
                _productionTaskRepo.Object,
                _aiClient.Object);
        }

        /// <summary>
        /// FR-4: Region type "Panel" maps to "LayoutCleanup" when available.
        /// </summary>
        [Theory]
        [InlineData("Panel", "LayoutCleanup")]
        [InlineData("SpeechBubble", "SpeechBubbleCleanup")]
        [InlineData("Background", "BackgroundDrawing")]
        [InlineData("Shading", "Shading")]
        [InlineData("Effect", "Effect")]
        public async Task SuggestTasksForPageAsync_MapsRegionTypeToCorrectTaskType(string regionType, string expectedTaskType)
        {
            // Arrange
            var pageId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var pageRegionId = Guid.NewGuid();

            var page = new MangaPage { PageId = pageId };
            _pageRepo.Setup(r => r.GetByIdWithDetailsAsync(pageId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(page);

            // Accepted detection with linked PageRegionId
            var detection = new AiDetectedRegion
            {
                DetectedRegionId = Guid.NewGuid(),
                PageId = pageId,
                RegionTypeCode = regionType,
                PageRegionId = pageRegionId,
                IsAccepted = true,
                X = 10, Y = 10, Width = 100, Height = 100
            };
            _detectedRegionRepo.Setup(r => r.GetByPageIdAndAcceptedStatusAsync(pageId, true, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(new List<AiDetectedRegion> { detection });

            // TaskTypes including the expected one
            var taskTypes = new List<TaskType>
            {
                new() { TaskTypeId = 1, TypeCode = expectedTaskType },
                new() { TaskTypeId = 99, TypeCode = "Other" }
            };
            _taskTypeRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(taskTypes);

            var service = CreateService();

            // Act
            var suggestions = await service.SuggestTasksForPageAsync(pageId, userId);

            // Assert
            var suggestion = Assert.Single(suggestions);
            Assert.Equal(expectedTaskType, suggestion.TaskTypeCode);
            Assert.Equal("Draft", suggestion.Status);
            Assert.Equal(pageRegionId, suggestion.PageRegionId);
        }

        /// <summary>
        /// FR-4: Region type "Character" prefers "CleanLine", falls back to "Tone", then "Other".
        /// </summary>
        [Fact]
        public async Task SuggestTasksForPageAsync_CharacterPrefersCleanLineThenToneThenOther()
        {
            // Arrange - no CleanLine, has Tone
            var pageId = Guid.NewGuid();
            _pageRepo.Setup(r => r.GetByIdWithDetailsAsync(pageId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new MangaPage { PageId = pageId });

            var detection = new AiDetectedRegion
            {
                PageId = pageId,
                RegionTypeCode = "Character",
                PageRegionId = Guid.NewGuid(),
                IsAccepted = true
            };
            _detectedRegionRepo.Setup(r => r.GetByPageIdAndAcceptedStatusAsync(pageId, true, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(new List<AiDetectedRegion> { detection });

            _taskTypeRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new List<TaskType>
                         {
                             new() { TaskTypeId = 2, TypeCode = "Tone" },
                             new() { TaskTypeId = 99, TypeCode = "Other" }
                         });

            var service = CreateService();

            // Act
            var suggestions = await service.SuggestTasksForPageAsync(pageId, Guid.NewGuid());

            // Assert - falls back to Tone
            Assert.Equal("Tone", suggestions.Single().TaskTypeCode);
        }

        /// <summary>
        /// FR-4: When preferred TaskType is missing, falls back to "Other".
        /// </summary>
        [Fact]
        public async Task SuggestTasksForPageAsync_FallsBackToOtherWhenPreferredMissing()
        {
            // Arrange - no LayoutCleanup, only Other
            var pageId = Guid.NewGuid();
            _pageRepo.Setup(r => r.GetByIdWithDetailsAsync(pageId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new MangaPage { PageId = pageId });

            var detection = new AiDetectedRegion
            {
                PageId = pageId,
                RegionTypeCode = "Panel",
                PageRegionId = Guid.NewGuid(),
                IsAccepted = true
            };
            _detectedRegionRepo.Setup(r => r.GetByPageIdAndAcceptedStatusAsync(pageId, true, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(new List<AiDetectedRegion> { detection });

            _taskTypeRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new List<TaskType>
                         {
                             new() { TaskTypeId = 99, TypeCode = "Other" }
                         });

            var service = CreateService();

            // Act
            var suggestions = await service.SuggestTasksForPageAsync(pageId, Guid.NewGuid());

            // Assert
            Assert.Equal("Other", suggestions.Single().TaskTypeCode);
        }

        /// <summary>
        /// FR-4: Suggestions are only generated from regions that have a PageRegionId (accepted).
        /// </summary>
        [Fact]
        public async Task SuggestTasksForPageAsync_SkipsRegionsWithoutPageRegionId()
        {
            // Arrange
            var pageId = Guid.NewGuid();
            _pageRepo.Setup(r => r.GetByIdWithDetailsAsync(pageId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new MangaPage { PageId = pageId });

            // Detection WITHOUT PageRegionId (not yet accepted/linked)
            var detection = new AiDetectedRegion
            {
                PageId = pageId,
                RegionTypeCode = "Panel",
                PageRegionId = null, // not linked
                IsAccepted = true
            };
            _detectedRegionRepo.Setup(r => r.GetByPageIdAndAcceptedStatusAsync(pageId, true, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(new List<AiDetectedRegion> { detection });

            _taskTypeRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new List<TaskType> { new() { TaskTypeId = 1, TypeCode = "Other" } });

            var service = CreateService();

            // Act
            var suggestions = await service.SuggestTasksForPageAsync(pageId, Guid.NewGuid());

            // Assert - no suggestions generated
            Assert.Empty(suggestions);
        }

        /// <summary>
        /// FR-3: AcceptRegion sets IsAccepted=true and creates a PageRegion with SourceType="AI"
        /// and CreatedByUserId equal to the current user.
        /// </summary>
        [Fact]
        public async Task AcceptRegionAsync_CreatesPageRegionWithCorrectCreatedByUserId()
        {
            // Arrange
            var regionId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var pageId = Guid.NewGuid();

            var region = new AiDetectedRegion
            {
                DetectedRegionId = regionId,
                PageId = pageId,
                RegionTypeCode = "Panel",
                X = 5, Y = 5, Width = 50, Height = 50,
                Confidence = 0.9m,
                IsAccepted = null
            };
            _detectedRegionRepo.Setup(r => r.GetByIdAsync(regionId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(region);

            PageRegion? capturedRegion = null;
            _pageRegionRepo.Setup(r => r.AddAsync(It.IsAny<PageRegion>(), It.IsAny<CancellationToken>()))
                           .Callback<PageRegion, CancellationToken>((pr, _) => capturedRegion = pr)
                           .Returns(Task.CompletedTask);

            var service = CreateService();

            // Act
            await service.AcceptRegionAsync(regionId, currentUserId);

            // Assert - PageRegion created with correct values
            Assert.NotNull(capturedRegion);
            Assert.Equal(pageId, capturedRegion!.PageId);
            Assert.Equal("AI", capturedRegion.SourceType);
            Assert.Equal(currentUserId, capturedRegion.CreatedByUserId);
            Assert.Equal(0.9m, capturedRegion.Confidence);

            // Detection marked as accepted
            Assert.True(region.IsAccepted);
        }

        /// <summary>
        /// FR-3: AcceptRegion throws if region not found.
        /// </summary>
        [Fact]
        public async Task AcceptRegionAsync_ThrowsWhenRegionNotFound()
        {
            var service = CreateService();
            _detectedRegionRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                               .ReturnsAsync((AiDetectedRegion?)null);

            await Assert.ThrowsAsync<ArgumentException>(
                () => service.AcceptRegionAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        /// <summary>
        /// FR-3: RejectRegion sets IsAccepted=false without creating a PageRegion.
        /// </summary>
        [Fact]
        public async Task RejectRegionAsync_SetsIsAcceptedFalseWithoutCreatingPageRegion()
        {
            // Arrange
            var regionId = Guid.NewGuid();
            var region = new AiDetectedRegion
            {
                DetectedRegionId = regionId,
                PageId = Guid.NewGuid(),
                RegionTypeCode = "Panel",
                IsAccepted = null
            };
            _detectedRegionRepo.Setup(r => r.GetByIdAsync(regionId, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(region);

            var service = CreateService();

            // Act
            await service.RejectRegionAsync(regionId, Guid.NewGuid());

            // Assert - marked rejected, no PageRegion created
            Assert.False(region.IsAccepted);
            _pageRegionRepo.Verify(
                r => r.AddAsync(It.IsAny<PageRegion>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        /// <summary>
        /// FR-5: ApproveTaskSuggestion creates a ProductionTask with correct properties
        /// and marks suggestion as "Approved".
        /// </summary>
        [Fact]
        public async Task ApproveTaskSuggestionAsync_CreatesProductionTaskAndMarksApproved()
        {
            // Arrange
            var suggestionId = Guid.NewGuid();
            var mangakaId = Guid.NewGuid();
            var assistantId = Guid.NewGuid();
            var pageRegionId = Guid.NewGuid();
            var pageId = Guid.NewGuid();
            var deadline = DateTime.UtcNow.AddDays(7);

            var suggestion = new AiTaskSuggestion
            {
                TaskSuggestionId = suggestionId,
                PageId = pageId,
                PageRegionId = pageRegionId,
                TaskTypeCode = "SpeechBubbleCleanup",
                Title = "Speech Bubble Cleanup",
                Description = "Clean bubbles",
                ComplexityScore = 2,
                Status = "Draft"
            };
            _suggestionRepo.Setup(r => r.GetByIdAsync(suggestionId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(suggestion);

            _pageRegionRepo.Setup(r => r.GetByIdAsync(pageRegionId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new PageRegion { RegionId = pageRegionId, PageId = pageId });

            _taskTypeRepo.Setup(r => r.GetByTypeCodeAsync("SpeechBubbleCleanup", It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new TaskType { TaskTypeId = 5, TypeCode = "SpeechBubbleCleanup" });

            _taskStatusRepo.Setup(r => r.GetByStatusCodeAsync("Assigned", It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new Domain.Entities.TaskStatus { TaskStatusId = 2, StatusCode = "Assigned" });

            ProductionTask? capturedTask = null;
            _productionTaskRepo.Setup(r => r.AddAsync(It.IsAny<ProductionTask>(), It.IsAny<CancellationToken>()))
                               .Callback<ProductionTask, CancellationToken>((pt, _) => capturedTask = pt)
                               .Returns(Task.CompletedTask);

            var service = CreateService();

            // Act
            await service.ApproveTaskSuggestionAsync(suggestionId, mangakaId, assistantId, deadline);

            // Assert - ProductionTask created with correct properties
            Assert.NotNull(capturedTask);
            Assert.Equal(5, capturedTask!.TaskTypeId);
            Assert.Equal(2, capturedTask.TaskStatusId);
            Assert.Equal(pageId, capturedTask.PageId);
            Assert.Equal(mangakaId, capturedTask.CreatedByMangakaId);
            Assert.Equal(assistantId, capturedTask.AssignedToAssistantId);
            Assert.Equal(deadline, capturedTask.Deadline);

            // Suggestion marked Approved
            Assert.Equal("Approved", suggestion.Status);
        }

        /// <summary>
        /// FR-5: ApproveTaskSuggestion rejects non-Draft suggestions.
        /// </summary>
        [Fact]
        public async Task ApproveTaskSuggestionAsync_ThrowsForNonDraftSuggestion()
        {
            var suggestion = new AiTaskSuggestion
            {
                Status = "Approved",
                TaskTypeCode = "Other"
            };
            _suggestionRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(suggestion);

            var service = CreateService();

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.ApproveTaskSuggestionAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null));
        }

        /// <summary>
        /// FR-5: ApproveTaskSuggestion requires an assistant to be assigned.
        /// </summary>
        [Fact]
        public async Task ApproveTaskSuggestionAsync_ThrowsWhenNoAssistantProvided()
        {
            var suggestion = new AiTaskSuggestion
            {
                Status = "Draft",
                TaskTypeCode = "Other",
                PageRegionId = Guid.NewGuid()
            };
            _suggestionRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(suggestion);

            var service = CreateService();

            await Assert.ThrowsAsync<ArgumentException>(
                () => service.ApproveTaskSuggestionAsync(Guid.NewGuid(), Guid.NewGuid(), null, null));
        }

        /// <summary>
        /// FR-5: RejectTaskSuggestion marks the suggestion as "Rejected".
        /// </summary>
        [Fact]
        public async Task RejectTaskSuggestionAsync_MarksSuggestionRejected()
        {
            // Arrange
            var suggestion = new AiTaskSuggestion
            {
                TaskSuggestionId = Guid.NewGuid(),
                Status = "Draft"
            };
            _suggestionRepo.Setup(r => r.GetByIdAsync(suggestion.TaskSuggestionId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(suggestion);

            var service = CreateService();

            // Act
            await service.RejectTaskSuggestionAsync(suggestion.TaskSuggestionId, Guid.NewGuid());

            // Assert
            Assert.Equal("Rejected", suggestion.Status);
            _productionTaskRepo.Verify(
                r => r.AddAsync(It.IsAny<ProductionTask>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
