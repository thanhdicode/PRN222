using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MangaWorkflow.Application;
using MangaWorkflow.Infrastructure;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Services;

Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddInfrastructure(context.Configuration);
        services.AddApplicationServices();
    })
    .Build();

Console.WriteLine("==============================================");
Console.WriteLine("    DB SMOKE TEST - MANGA WORKFLOW SYSTEM     ");
Console.WriteLine("==============================================");

using var scope = host.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<MangaWorkflowDbContext>();
var dashboardService = scope.ServiceProvider.GetRequiredService<IDashboardService>();

try
{
    Console.WriteLine("Testing Database Connection...");
    Console.WriteLine("Connection String: " + dbContext.Database.GetDbConnection().ConnectionString);
    bool canConnect = await dbContext.Database.CanConnectAsync();
    
    if (canConnect)
    {
        Console.WriteLine("[OK] Database connection successful.\n");

        var summary = await dashboardService.GetDashboardSummaryAsync();
        var rolesCount = await dbContext.Roles.CountAsync();
        
        Console.WriteLine($"Total Roles:            {rolesCount}");
        Console.WriteLine($"Total Users:            {summary.TotalUsers}");
        Console.WriteLine($"Total Series:           {summary.TotalSeries}");
        Console.WriteLine($"Total Chapters:         {summary.TotalChapters}");
        Console.WriteLine($"Total ProductionTasks:  {summary.TotalTasks}");
        
        var totalNotifications = await dbContext.Notifications.CountAsync();
        Console.WriteLine($"Total Notifications:    {totalNotifications}");
        
        Console.WriteLine("\n[SUCCESS] DbSmokeTest completed successfully.");
    }
    else
    {
        Console.WriteLine("[FAIL] Cannot connect to database.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"[ERROR] {ex.Message}");
    Console.WriteLine(ex.StackTrace);
}

Console.WriteLine("==============================================");
