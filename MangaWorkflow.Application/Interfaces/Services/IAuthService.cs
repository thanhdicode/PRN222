using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Interfaces.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Validates user credentials.
        /// PRN222 Demo: compares plain-text password stored in PasswordHash column.
        /// TODO: Replace with proper BCrypt hashing before production use.
        /// </summary>
        Task<User?> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default);
    }
}
