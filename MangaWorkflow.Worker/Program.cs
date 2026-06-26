using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Application;
using MangaWorkflow.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

// Database context & Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Services
builder.Services.AddApplicationServices();

// Register Hosted Services
builder.Services.AddHostedService<MangaWorkflow.Worker.DeadlineReminderWorker>();
builder.Services.AddHostedService<MangaWorkflow.Worker.OverdueTaskScannerWorker>();
builder.Services.AddHostedService<MangaWorkflow.Worker.RankingRiskWorker>();
builder.Services.AddHostedService<MangaWorkflow.Worker.NotificationCleanupWorker>();
builder.Services.AddHostedService<MangaWorkflow.Worker.MonthlyEarningCalculatorWorker>();
var host = builder.Build();
host.Run();
