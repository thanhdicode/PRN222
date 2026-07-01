using System.Security.Claims;
using MangaWorkflow.Application.DTOs.Submissions;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Web.Areas.Assistant.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace MangaWorkflow.Tests
{
    public class AssistantSubmissionsControllerTests
    {
        [Fact]
        public async Task Submit_SavesUploadedFileBeforeCallingSubmissionService()
        {
            var assistantId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var submissionService = new Mock<ISubmissionService>();
            var fileStorage = new Mock<IFileStorageService>();
            SubmitTaskDto? capturedDto = null;

            var content = new MemoryStream(new byte[] { 1, 2, 3 });
            var uploadedFile = new FormFile(content, 0, content.Length, "UploadedFile", "work.png")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };

            fileStorage
                .Setup(s => s.IsValidSubmissionFile(uploadedFile))
                .Returns(true);
            fileStorage
                .Setup(s => s.SaveFileAsync(uploadedFile, "submissions", It.IsAny<CancellationToken>()))
                .ReturnsAsync("/uploads/submissions/work.png");

            submissionService
                .Setup(s => s.SubmitTaskAsync(It.IsAny<SubmitTaskDto>(), assistantId, It.IsAny<CancellationToken>()))
                .Callback<SubmitTaskDto, Guid, CancellationToken>((dto, _, _) => capturedDto = dto)
                .Returns(Task.CompletedTask);

            var controller = new SubmissionsController(submissionService.Object, fileStorage.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, assistantId.ToString())
                        }, "TestAuth"))
                    }
                }
            };
            controller.TempData = new TempDataDictionary(
                controller.HttpContext,
                Mock.Of<ITempDataProvider>());

            var result = await controller.Submit(new SubmitTaskDto
            {
                TaskId = taskId,
                UploadedFile = uploadedFile,
                Notes = "done"
            });

            Assert.IsType<RedirectToActionResult>(result);
            Assert.NotNull(capturedDto);
            Assert.Equal("/uploads/submissions/work.png", capturedDto!.FileUrl);
            Assert.Equal(taskId, capturedDto.TaskId);
        }
    }
}
