using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace MangaWorkflow.Worker
{
    public class DummyWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ApplicationName { get; set; } = "MangaWorkflow.Worker";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        public string EnvironmentName { get; set; } = "Development";
    }
}
