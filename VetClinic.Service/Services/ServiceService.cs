using VetClinic.Repository.Entities;
using VetClinic.Repository.Repositories;
using VetClinic.Service.Interfaces;

namespace VetClinic.Service.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<Repository.Entities.Service?> GetServiceByIdAsync(int id)
        {
            return await _serviceRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Repository.Entities.Service>> GetAllServicesAsync()
        {
            return await _serviceRepository.GetAllAsync();
        }

        public async Task<Repository.Entities.Service> CreateServiceAsync(Repository.Entities.Service service)
        {
            // Additional business validation beyond repository level
            if (service.DurationMinutes < 15)
                throw new ArgumentException("Service duration must be at least 15 minutes");

            if (service.DurationMinutes > 480) // 8 hours max
                throw new ArgumentException("Service duration cannot exceed 8 hours");

            return await _serviceRepository.CreateAsync(service);
        }

        public async Task<Repository.Entities.Service> UpdateServiceAsync(Repository.Entities.Service service, int userId)
        {
            // Additional business validation
            if (service.DurationMinutes < 15)
                throw new ArgumentException("Service duration must be at least 15 minutes");

            if (service.DurationMinutes > 480) // 8 hours max
                throw new ArgumentException("Service duration cannot exceed 8 hours");

            return await _serviceRepository.UpdateAsync(service);
        }

        public async Task<bool> DeleteServiceAsync(int id, int userId)
        {
            // Soft delete: Set isActive to false instead of hard delete
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null) return false;

            // Check if service is already inactive (soft deleted)
            if (!service.IsActive)
                throw new InvalidOperationException("Service is already deactivated.");

            // Perform soft delete by setting IsActive to false
            service.IsActive = false;
            await _serviceRepository.UpdateAsync(service);

            return true;
        }

        public async Task<bool> ServiceExistsAsync(int id)
        {
            return await _serviceRepository.ExistsAsync(id);
        }

        public async Task<IEnumerable<Repository.Entities.Service>> GetActiveServicesAsync()
        {
            return await _serviceRepository.GetActiveServicesAsync();
        }

        public async Task<Repository.Entities.Service?> GetServiceByNameAsync(string name)
        {
            return await _serviceRepository.GetServiceByNameAsync(name);
        }

        public async Task<IEnumerable<Repository.Entities.Service>> GetServicesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            if (minPrice < 0 || maxPrice < 0)
                throw new ArgumentException("Price values must be non-negative");

            if (minPrice > maxPrice)
                throw new ArgumentException("Minimum price cannot be greater than maximum price");

            return await _serviceRepository.GetServicesByPriceRangeAsync(minPrice, maxPrice);
        }

        public async Task<bool> DeactivateServiceAsync(int serviceId)
        {
            return await _serviceRepository.DeactivateServiceAsync(serviceId);
        }

        public async Task<bool> IsServiceNameUniqueAsync(string name, int? excludeServiceId = null)
        {
            var existingService = await _serviceRepository.GetServiceByNameAsync(name);
            if (existingService == null) return true;

            // If we're excluding a service ID (for updates), check if it's the same service
            if (excludeServiceId.HasValue && existingService.Id == excludeServiceId.Value)
                return true;

            return false;
        }
    }
}