using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public interface IUserDAO : IBaseDAO<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersWithPetsAsync();
        Task<User?> GetByEmailAndPasswordAsync(string email, string passwordHash);
        Task UpdateLastLoginAsync(int userId);
    }
}
