using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public interface IVaccinationRepository
    {
        Task<Vaccination?> GetByIdAsync(int id);
        Task<IEnumerable<Vaccination>> GetAllAsync();
        Task<Vaccination> CreateAsync(Vaccination vaccination);
        Task<Vaccination> UpdateAsync(Vaccination vaccination);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Vaccination-specific methods
        Task<IEnumerable<Vaccination>> GetVaccinationsByPetAsync(int petId);
        Task<IEnumerable<Vaccination>> GetDueVaccinationsAsync();
        Task<IEnumerable<Vaccination>> GetOverdueVaccinationsAsync();
        Task<IEnumerable<Vaccination>> GetUpcomingVaccinationsAsync(int days = 30);
        Task<bool> MarkVaccinationCompleteAsync(int vaccinationId, DateTime completedDate);
        Task<IEnumerable<Vaccination>> GetVaccinationsByStatusAsync(string status);
    }
}