using VetClinic.Repository.Entities;

namespace VetClinic.Service.Interfaces
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string email, string password);
        Task<User> RegisterAsync(string fullName, string email, string password, string? phoneNumber = null, string? address = null);
        Task<bool> IsEmailUniqueAsync(string email);
        Task LogoutAsync(int userId);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
