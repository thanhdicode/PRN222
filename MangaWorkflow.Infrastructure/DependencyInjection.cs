using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.Options;
using MangaWorkflow.Infrastructure.Repositories;

namespace MangaWorkflow.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MangaWorkflowDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Phase 1 repositories
            services.AddScoped<IBackgroundJobLogRepository, BackgroundJobLogRepository>();
            services.AddScoped<IBackgroundJobQueriesRepository, BackgroundJobQueriesRepository>();
            services.AddScoped<IChapterRepository, ChapterRepository>();
            services.AddScoped<IProductionTaskRepository, ProductionTaskRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ISeriesRepository, SeriesRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Phase 2 repositories
            services.AddScoped<IPageRepository, PageRepository>();
            services.AddScoped<IBoardVoteRepository, BoardVoteRepository>();
            services.AddScoped<IRankingRepository, RankingRepository>();

            // Phase 3 repositories & services
            services.AddScoped<ITaskSubmissionRepository, TaskSubmissionRepository>();
            services.AddScoped<IEditorCommentRepository, EditorCommentRepository>();
            services.AddScoped<IPageRegionRepository, PageRegionRepository>();
            services.AddScoped<MangaWorkflow.Application.Interfaces.Services.IFileStorageService, MangaWorkflow.Infrastructure.FileStorage.LocalFileStorageService>();

            // Phase 3 stabilization — lookup repos (no hardcoded IDs)
            services.AddScoped<ISubmissionStatusRepository, SubmissionStatusRepository>();
            services.AddScoped<INotificationTypeRepository, NotificationTypeRepository>();
            services.AddScoped<ITaskTypeRepository, TaskTypeRepository>();
            services.AddScoped<ITaskStatusRepository, TaskStatusRepository>();

            // Phase 4 repositories
            services.AddScoped<IDashboardRepository, DashboardRepository>();

            // Phase 5 AI repositories
            services.AddScoped<IAiInferenceRepository, AiInferenceRepository>();
            services.AddScoped<IAiDetectedRegionRepository, AiDetectedRegionRepository>();
            services.AddScoped<IAiTaskSuggestionRepository, AiTaskSuggestionRepository>();

            // Audit Log repository
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();

            // Phase 5 AI configuration
            services.Configure<AiVisionOptions>(configuration.GetSection("AI"));

            // Phase 5 AI clients
            services.AddHttpClient<MangaWorkflow.Application.Interfaces.IAiVisionClient, MangaWorkflow.Infrastructure.Ai.MockAiVisionClient>(client =>
            {
                var aiBaseUrl = configuration["AI:BaseUrl"] ?? "http://localhost:8001";
                client.BaseAddress = new System.Uri(aiBaseUrl);

                var timeoutSeconds = configuration.GetValue<int>("AI:TimeoutSeconds", 60);
                client.Timeout = System.TimeSpan.FromSeconds(timeoutSeconds);
            });

            return services;
        }
    }
}
