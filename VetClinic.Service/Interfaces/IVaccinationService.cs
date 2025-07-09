using VetClinic.Repository.Entities;

namespace VetClinic.Service.Interfaces
{
    public interface IVaccinationService
    {
        Task<Vaccination?> GetVaccinationByIdAsync(int id);
        Task<IEnumerable<Vaccination>> GetAllVaccinationsAsync();
        Task<Vaccination> CreateVaccinationAsync(Vaccination vaccination);
        Task<Vaccination> UpdateVaccinationAsync(Vaccination vaccination);
        Task<bool> DeleteVaccinationAsync(int id);
        Task<bool> VaccinationExistsAsync(int id);

        // Vaccination-specific business methods
        Task<IEnumerable<Vaccination>> GetVaccinationsByPetAsync(int petId);
        Task<IEnumerable<Vaccination>> GetDueVaccinationsAsync();
        Task<IEnumerable<Vaccination>> GetOverdueVaccinationsAsync();
        Task<IEnumerable<Vaccination>> GetUpcomingVaccinationsAsync(int days = 30);
        Task<bool> MarkVaccinationCompleteAsync(int vaccinationId, DateTime completedDate);
        Task<IEnumerable<Vaccination>> GetVaccinationsByStatusAsync(string status);
        Task<bool> CanUserAccessVaccinationAsync(int userId, int vaccinationId, string userRole);
        Task<IEnumerable<Vaccination>> CreateVaccinationScheduleAsync(int petId, string species, DateTime birthDate);
    }
}