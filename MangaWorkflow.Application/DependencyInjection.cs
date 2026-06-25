using Microsoft.Extensions.DependencyInjection;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Application.Services;

namespace MangaWorkflow.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ISeriesService, SeriesService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}
