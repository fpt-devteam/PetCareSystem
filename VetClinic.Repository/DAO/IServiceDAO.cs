using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public interface IServiceDAO : IBaseDAO<Service>
    {
        Task<IEnumerable<Service>> GetActiveServicesAsync();
        Task<Service?> GetServiceByNameAsync(string name);
        Task<IEnumerable<Service>> GetServicesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<bool> DeactivateServiceAsync(int serviceId);
    }
}