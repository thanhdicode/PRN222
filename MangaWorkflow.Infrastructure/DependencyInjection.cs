using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MangaWorkflow.Infrastructure.Persistence;
using MangaWorkflow.Application.Interfaces.Repositories;
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
            services.AddScoped<ISeriesRepository, SeriesRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IChapterRepository, ChapterRepository>();
            services.AddScoped<IProductionTaskRepository, ProductionTaskRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

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

            // Phase 4 repositories
            services.AddScoped<IDashboardRepository, DashboardRepository>();

            return services;
        }
    }
}

