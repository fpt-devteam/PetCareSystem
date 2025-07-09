using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetUsersWithPetsAsync();
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<User?> AuthenticateAsync(string email, string password);
        Task UpdateLastLoginAsync(int userId);
    }
}
