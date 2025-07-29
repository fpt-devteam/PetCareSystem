using VetClinic.Repository.Entities;
using VetClinic.Repository.Repositories;
using VetClinic.Service.Interfaces;

namespace VetClinic.Service.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _userRepository.AuthenticateAsync(email, password);
            if (user != null && user.IsActive)
            {
                await _userRepository.UpdateLastLoginAsync(user.Id);
                return user;
            }

            return null;
        }

        public async Task<User> RegisterAsync(string fullName, string email, string password, string? phoneNumber = null, string? address = null)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required");

            if (password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters long");

            if (!await IsEmailUniqueAsync(email))
                throw new ArgumentException("Email already exists");

            // Hash password
            var passwordHash = HashPassword(password);

            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = passwordHash,
                PhoneNumber = phoneNumber,
                Address = address,
                Role = "Customer",
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            return await _userRepository.CreateAsync(user);
        }

        public async Task<User> RegisterWithRoleAsync(string fullName, string email, string password, string role, string? phoneNumber = null, string? address = null)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required");

            if (password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters long");

            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role is required");

            // Validate role
            var validRoles = new[] { "Customer", "Staff", "Doctor", "Manager", "Admin" };
            if (!validRoles.Contains(role))
                throw new ArgumentException("Invalid role specified");

            if (!await IsEmailUniqueAsync(email))
                throw new ArgumentException("Email already exists");

            // Hash password
            var passwordHash = HashPassword(password);

            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = passwordHash,
                PhoneNumber = phoneNumber,
                Address = address,
                Role = role,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            return await _userRepository.CreateAsync(user);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            return existingUser == null;
        }

        public async Task LogoutAsync(int userId)
        {
            // Could implement logout logic here if needed
            // For now, session is cleared in the controller/page
            await Task.CompletedTask;
        }

        public string HashPassword(string password)
        {
            // Simple hash for demo - use BCrypt in production
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool VerifyPassword(string password, string hash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == hash;
        }
    }
}
