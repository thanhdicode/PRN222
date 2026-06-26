using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Infrastructure.FileStorage
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        public LocalFileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public bool IsValidImageFile(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".webp";
        }

        public bool IsValidSubmissionFile(IFormFile file)
        {
            // For tasks we might accept images or zip files
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            return IsValidImageFile(file) || ext == ".zip" || ext == ".rar" || ext == ".pdf";
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subfolder, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", subfolder);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, ct);
            }

            return $"/uploads/{subfolder}/{uniqueFileName}";
        }
    }
}
