using Microsoft.EntityFrameworkCore;
using VetClinic.Repository.Data;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public class UserDAO : BaseDAO<User>, IUserDAO
    {
        public UserDAO(VetClinicDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersWithPetsAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<User?> GetByEmailAndPasswordAsync(string email, string passwordHash)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == passwordHash && u.IsActive);
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _dbSet.FindAsync(userId);
            if (user != null)
            {
                user.LastLoginDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
