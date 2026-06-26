using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Infrastructure.Persistence;

namespace MangaWorkflow.Infrastructure.Repositories
{
    public class SubmissionStatusRepository : ISubmissionStatusRepository
    {
        private readonly MangaWorkflowDbContext _context;

        public SubmissionStatusRepository(MangaWorkflowDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetIdByCodeAsync(string statusCode, CancellationToken ct = default)
        {
            var status = await _context.SubmissionStatuses
                .FirstOrDefaultAsync(s => s.StatusCode == statusCode, ct);

            if (status == null)
                throw new InvalidOperationException($"SubmissionStatus with code '{statusCode}' was not found in the database.");

            return status.SubmissionStatusId;
        }
    }
}
