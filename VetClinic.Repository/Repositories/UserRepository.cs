using VetClinic.Repository.DAO;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IUserDAO _userDAO;

        public UserRepository(IUserDAO userDAO)
        {
            _userDAO = userDAO;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userDAO.GetByIdAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userDAO.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userDAO.GetAllAsync();
        }

        public async Task<IEnumerable<User>> GetUsersWithPetsAsync()
        {
            return await _userDAO.GetUsersWithPetsAsync();
        }

        public async Task<User> CreateAsync(User user)
        {
            return await _userDAO.AddAsync(user);
        }

        public async Task<User> UpdateAsync(User user)
        {
            return await _userDAO.UpdateAsync(user);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _userDAO.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _userDAO.ExistsAsync(id);
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            // Hash password (in production, use proper hashing like BCrypt)
            var passwordHash = HashPassword(password);
            return await _userDAO.GetByEmailAndPasswordAsync(email, passwordHash);
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            await _userDAO.UpdateLastLoginAsync(userId);
        }

        private string HashPassword(string password)
        {
            // Simple hash for demo - use BCrypt in production
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
