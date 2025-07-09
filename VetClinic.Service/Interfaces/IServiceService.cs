using VetClinic.Repository.Entities;

namespace VetClinic.Service.Interfaces
{
    public interface IServiceService
    {
        Task<Repository.Entities.Service?> GetServiceByIdAsync(int id);
        Task<IEnumerable<Repository.Entities.Service>> GetAllServicesAsync();
        Task<Repository.Entities.Service> CreateServiceAsync(Repository.Entities.Service service);
        Task<Repository.Entities.Service> UpdateServiceAsync(Repository.Entities.Service service, int userId);
        Task<bool> DeleteServiceAsync(int id, int userId);
        Task<bool> ServiceExistsAsync(int id);

        // Service-specific business methods
        Task<IEnumerable<Repository.Entities.Service>> GetActiveServicesAsync();
        Task<Repository.Entities.Service?> GetServiceByNameAsync(string name);
        Task<IEnumerable<Repository.Entities.Service>> GetServicesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<bool> DeactivateServiceAsync(int serviceId);
        Task<bool> IsServiceNameUniqueAsync(string name, int? excludeServiceId = null);
    }
}