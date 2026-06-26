using Microsoft.Extensions.DependencyInjection;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.Services;

namespace MangaWorkflow.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Phase 1 services
            services.AddScoped<ISeriesService, SeriesService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<INotificationService, NotificationService>();

            // Phase 2 services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IChapterService, ChapterService>();
            services.AddScoped<IPageService, PageService>();
            services.AddScoped<IBoardReviewService, BoardReviewService>();
            services.AddScoped<IRankingService, RankingService>();

            // Phase 3 services
            services.AddScoped<ITaskWorkflowService, TaskWorkflowService>();
            services.AddScoped<ISubmissionService, SubmissionService>();
            services.AddScoped<IEditorCommentService, EditorCommentService>();
            services.AddScoped<IPageRegionService, PageRegionService>();

            // Phase 4 services
            services.AddSingleton<IWorkflowHubNotifier, MangaWorkflow.Application.Implementations.NoOpWorkflowHubNotifier>();

            return services;
        }
    }
}

