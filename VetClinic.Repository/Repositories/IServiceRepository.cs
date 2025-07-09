using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public interface IServiceRepository
    {
        Task<Service?> GetByIdAsync(int id);
        Task<IEnumerable<Service>> GetAllAsync();
        Task<Service> CreateAsync(Service service);
        Task<Service> UpdateAsync(Service service);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Service-specific methods
        Task<IEnumerable<Service>> GetActiveServicesAsync();
        Task<Service?> GetServiceByNameAsync(string name);
        Task<IEnumerable<Service>> GetServicesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<bool> DeactivateServiceAsync(int serviceId);
    }
}