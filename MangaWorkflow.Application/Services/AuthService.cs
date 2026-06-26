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
        /// Validates user credentials.
        /// PRN222 Demo Mode: compares plain-text password stored in PasswordHash column.
        /// TODO: Replace with BCrypt.Net-Next hashing before any production deployment.
        /// </summary>
        public async Task<User?> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByEmailAsync(email, ct);

            if (user == null || !user.IsActive)
                return null;

            // Demo mode: plain-text password comparison
            // The seed database stores plaintext passwords in PasswordHash for demo purposes
            if (user.PasswordHash == null)
                return null;

            bool isValid = user.PasswordHash == password;
            return isValid ? user : null;
        }
    }
}
