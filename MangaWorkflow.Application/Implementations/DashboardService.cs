using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.DTOs;
using MangaWorkflow.Application.Interfaces.Services;

namespace MangaWorkflow.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISeriesRepository _seriesRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IProductionTaskRepository _taskRepository;
        private readonly INotificationRepository _notificationRepository;

        public DashboardService(
            IUserRepository userRepository,
            ISeriesRepository seriesRepository,
            IChapterRepository chapterRepository,
            IProductionTaskRepository taskRepository,
            INotificationRepository notificationRepository)
        {
            _userRepository = userRepository;
            _seriesRepository = seriesRepository;
            _chapterRepository = chapterRepository;
            _taskRepository = taskRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
        {
            return new DashboardSummaryDto
            {
                TotalUsers = await _userRepository.CountUsersAsync(cancellationToken),
                TotalSeries = await _seriesRepository.CountSeriesAsync(cancellationToken),
                TotalChapters = await _chapterRepository.CountChaptersAsync(cancellationToken),
                TotalTasks = await _taskRepository.CountTasksAsync(cancellationToken),
                UnreadNotifications = 0 // Dummy parameter for smoke test if needed, or pass real user ID
            };
        }
    }
}
