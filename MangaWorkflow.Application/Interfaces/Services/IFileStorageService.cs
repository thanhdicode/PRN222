using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string subfolder, CancellationToken ct = default);
        bool IsValidImageFile(IFormFile file);
        bool IsValidSubmissionFile(IFormFile file);
    }
}
