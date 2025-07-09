using Microsoft.EntityFrameworkCore;
using VetClinic.Repository.Data;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public class ServiceDAO : BaseDAO<Service>, IServiceDAO
    {
        public ServiceDAO(VetClinicDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Service>> GetActiveServicesAsync()
        {
            return await _dbSet
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Service?> GetServiceByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Service>> GetServicesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet
                .Where(s => s.IsActive && s.Price >= minPrice && s.Price <= maxPrice)
                .OrderBy(s => s.Price)
                .ToListAsync();
        }

        public async Task<bool> DeactivateServiceAsync(int serviceId)
        {
            var service = await GetByIdAsync(serviceId);
            if (service == null)
                return false;

            service.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}