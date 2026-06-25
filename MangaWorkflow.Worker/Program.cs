using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Application;
using MangaWorkflow.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

// Database context & Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Services
builder.Services.AddApplicationServices();

// builder.Services.AddHostedService<OverdueTaskScannerWorker>(); // To be implemented

var host = builder.Build();
host.Run();
