using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public interface IVaccinationDAO : IBaseDAO<Vaccination>
    {
        Task<IEnumerable<Vaccination>> GetVaccinationsByPetAsync(int petId);
        Task<IEnumerable<Vaccination>> GetDueVaccinationsAsync();
        Task<IEnumerable<Vaccination>> GetOverdueVaccinationsAsync();
        Task<IEnumerable<Vaccination>> GetUpcomingVaccinationsAsync(int days = 30);
        Task<bool> MarkVaccinationCompleteAsync(int vaccinationId, DateTime completedDate);
        Task<IEnumerable<Vaccination>> GetVaccinationsByStatusAsync(string status);
    }
}