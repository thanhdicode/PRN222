using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MangaWorkflow.Application.DTOs.Submissions
{
    public class SubmitTaskDto
    {
        public Guid TaskId { get; set; }
        [MaxLength(2000)] public string? Notes { get; set; }
        public string? FileUrl { get; set; }
        public IFormFile? UploadedFile { get; set; }
    }
}
