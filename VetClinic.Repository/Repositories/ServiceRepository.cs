using VetClinic.Repository.DAO;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly IServiceDAO _serviceDAO;

        public ServiceRepository(IServiceDAO serviceDAO)
        {
            _serviceDAO = serviceDAO;
        }

        public async Task<Service?> GetByIdAsync(int id)
        {
            return await _serviceDAO.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Service>> GetAllAsync()
        {
            return await _serviceDAO.GetAllAsync();
        }

        public async Task<Service> CreateAsync(Service service)
        {
            // Business logic validation
            if (string.IsNullOrWhiteSpace(service.Name))
                throw new ArgumentException("Service name is required");

            if (service.Price <= 0 && service.IsActive)
                throw new ArgumentException("Active service price must be greater than zero");

            if (service.DurationMinutes <= 0)
                throw new ArgumentException("Service duration must be greater than zero");

            // Check if service name already exists
            var existingService = await _serviceDAO.GetServiceByNameAsync(service.Name);
            if (existingService != null)
                throw new ArgumentException("Service with this name already exists");

            service.CreatedDate = DateTime.Now;
            return await _serviceDAO.AddAsync(service);
        }

        public async Task<Service> UpdateAsync(Service service)
        {
            // Business logic validation
            var existingService = await _serviceDAO.GetByIdAsync(service.Id);
            if (existingService == null)
                throw new ArgumentException("Service not found");

            if (string.IsNullOrWhiteSpace(service.Name))
                throw new ArgumentException("Service name is required");

            if (service.Price <= 0 && service.IsActive)
                throw new ArgumentException("Active service price must be greater than zero");

            if (service.DurationMinutes <= 0)
                throw new ArgumentException("Service duration must be greater than zero");

            // Check if service name already exists (excluding current service)
            var nameExists = await _serviceDAO.GetServiceByNameAsync(service.Name);
            if (nameExists != null && nameExists.Id != service.Id)
                throw new ArgumentException("Service with this name already exists");

            return await _serviceDAO.UpdateAsync(service);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _serviceDAO.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _serviceDAO.ExistsAsync(id);
        }

        public async Task<IEnumerable<Service>> GetActiveServicesAsync()
        {
            return await _serviceDAO.GetActiveServicesAsync();
        }

        public async Task<Service?> GetServiceByNameAsync(string name)
        {
            return await _serviceDAO.GetServiceByNameAsync(name);
        }

        public async Task<IEnumerable<Service>> GetServicesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _serviceDAO.GetServicesByPriceRangeAsync(minPrice, maxPrice);
        }

        public async Task<bool> DeactivateServiceAsync(int serviceId)
        {
            return await _serviceDAO.DeactivateServiceAsync(serviceId);
        }
    }
}