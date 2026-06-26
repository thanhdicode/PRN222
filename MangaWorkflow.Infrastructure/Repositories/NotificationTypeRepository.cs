using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Infrastructure.Persistence;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class NotificationTypeRepository : INotificationTypeRepository
    {
        private readonly MangaWorkflowDbContext _context;

        public NotificationTypeRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<int?> GetIdByCodeAsync(string typeCode, CancellationToken ct = default)
        {
            var notifType = await _context.NotificationTypes
                .FirstOrDefaultAsync(n => n.TypeCode == typeCode, ct);

            return notifType?.NotificationTypeId;
        }
    }
}
