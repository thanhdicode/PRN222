using MangaWorkflow.Application.Interfaces.Repositories;
using MangaWorkflow.Application.Interfaces.Services;
using MangaWorkflow.Domain.Entities;

namespace MangaWorkflow.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Validates user credentials using BCrypt password verification.
        /// </summary>
        public async Task<User?> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByEmailAsync(email, ct);

            if (user == null || !user.IsActive)
                return null;

            if (user.PasswordHash == null)
                return null;

            // BCrypt verification — supports both hashed passwords and legacy plain-text (fallback)
            bool isValid;
            if (user.PasswordHash.StartsWith("$2"))
            {
                // Modern: BCrypt hash
                isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            }
            else
            {
                // Legacy fallback for existing seed data plain-text passwords
                // TODO: Force password reset or re-hash all seed data after migration
                isValid = user.PasswordHash == password;
            }

            return isValid ? user : null;
        }
    }
}
