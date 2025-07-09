using VetClinic.Repository.Entities;
using VetClinic.Repository.Repositories;
using VetClinic.Service.Interfaces;

namespace VetClinic.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<IEnumerable<User>> GetUsersWithPetsAsync()
        {
            return await _userRepository.GetUsersWithPetsAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            // Business logic validation
            if (string.IsNullOrWhiteSpace(user.FullName))
                throw new ArgumentException("Full name is required");

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email is required");

            if (!await IsEmailUniqueAsync(user.Email))
                throw new ArgumentException("Email already exists");

            user.CreatedDate = DateTime.Now;
            return await _userRepository.CreateAsync(user);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            // Business logic validation
            var existingUser = await _userRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
                throw new ArgumentException("User not found");

            if (!await IsEmailUniqueAsync(user.Email, user.Id))
                throw new ArgumentException("Email already exists");

            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            return await _userRepository.DeleteAsync(id);
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            return await _userRepository.ExistsAsync(id);
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser == null)
                return true;

            return excludeUserId.HasValue && existingUser.Id == excludeUserId.Value;
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            var allUsers = await _userRepository.GetAllAsync();
            return allUsers.Where(u => u.Role == role && u.IsActive);
        }
    }
}
