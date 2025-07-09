using VetClinic.Repository.DAO;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public class VaccinationRepository : IVaccinationRepository
    {
        private readonly IVaccinationDAO _vaccinationDAO;

        public VaccinationRepository(IVaccinationDAO vaccinationDAO)
        {
            _vaccinationDAO = vaccinationDAO;
        }

        public async Task<Vaccination?> GetByIdAsync(int id)
        {
            return await _vaccinationDAO.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Vaccination>> GetAllAsync()
        {
            return await _vaccinationDAO.GetAllAsync();
        }

        public async Task<Vaccination> CreateAsync(Vaccination vaccination)
        {
            // Business logic validation
            if (vaccination.PetId <= 0)
                throw new ArgumentException("Valid pet ID is required");

            if (string.IsNullOrWhiteSpace(vaccination.VaccineName))
                throw new ArgumentException("Vaccine name is required");

            if (vaccination.DueDate == default)
                throw new ArgumentException("Due date is required");

            vaccination.CreatedDate = DateTime.Now;
            vaccination.Status = "Due";
            return await _vaccinationDAO.AddAsync(vaccination);
        }

        public async Task<Vaccination> UpdateAsync(Vaccination vaccination)
        {
            // Business logic validation
            var existingVaccination = await _vaccinationDAO.GetByIdAsync(vaccination.Id);
            if (existingVaccination == null)
                throw new ArgumentException("Vaccination not found");

            if (string.IsNullOrWhiteSpace(vaccination.VaccineName))
                throw new ArgumentException("Vaccine name is required");

            if (vaccination.DueDate == default)
                throw new ArgumentException("Due date is required");

            // Update status based on completion
            if (vaccination.CompletedDate.HasValue && vaccination.Status != "Completed")
                vaccination.Status = "Completed";

            return await _vaccinationDAO.UpdateAsync(vaccination);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _vaccinationDAO.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _vaccinationDAO.ExistsAsync(id);
        }

        public async Task<IEnumerable<Vaccination>> GetVaccinationsByPetAsync(int petId)
        {
            return await _vaccinationDAO.GetVaccinationsByPetAsync(petId);
        }

        public async Task<IEnumerable<Vaccination>> GetDueVaccinationsAsync()
        {
            return await _vaccinationDAO.GetDueVaccinationsAsync();
        }

        public async Task<IEnumerable<Vaccination>> GetOverdueVaccinationsAsync()
        {
            return await _vaccinationDAO.GetOverdueVaccinationsAsync();
        }

        public async Task<IEnumerable<Vaccination>> GetUpcomingVaccinationsAsync(int days = 30)
        {
            return await _vaccinationDAO.GetUpcomingVaccinationsAsync(days);
        }

        public async Task<bool> MarkVaccinationCompleteAsync(int vaccinationId, DateTime completedDate)
        {
            return await _vaccinationDAO.MarkVaccinationCompleteAsync(vaccinationId, completedDate);
        }

        public async Task<IEnumerable<Vaccination>> GetVaccinationsByStatusAsync(string status)
        {
            return await _vaccinationDAO.GetVaccinationsByStatusAsync(status);
        }
    }
}