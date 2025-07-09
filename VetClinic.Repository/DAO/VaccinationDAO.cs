using Microsoft.EntityFrameworkCore;
using VetClinic.Repository.Data;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public class VaccinationDAO : BaseDAO<Vaccination>, IVaccinationDAO
    {
        public VaccinationDAO(VetClinicDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Vaccination>> GetVaccinationsByPetAsync(int petId)
        {
            return await _dbSet
                .Where(v => v.PetId == petId)
                .Include(v => v.Pet)
                .OrderBy(v => v.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vaccination>> GetDueVaccinationsAsync()
        {
            return await _dbSet
                .Where(v => v.Status == "Due" && v.DueDate <= DateTime.Today)
                .Include(v => v.Pet)
                .ThenInclude(p => p.Owner)
                .OrderBy(v => v.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vaccination>> GetOverdueVaccinationsAsync()
        {
            return await _dbSet
                .Where(v => v.Status == "Due" && v.DueDate < DateTime.Today)
                .Include(v => v.Pet)
                .ThenInclude(p => p.Owner)
                .OrderBy(v => v.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vaccination>> GetUpcomingVaccinationsAsync(int days = 30)
        {
            var endDate = DateTime.Today.AddDays(days);
            return await _dbSet
                .Where(v => v.Status == "Due" && v.DueDate >= DateTime.Today && v.DueDate <= endDate)
                .Include(v => v.Pet)
                .ThenInclude(p => p.Owner)
                .OrderBy(v => v.DueDate)
                .ToListAsync();
        }

        public async Task<bool> MarkVaccinationCompleteAsync(int vaccinationId, DateTime completedDate)
        {
            var vaccination = await GetByIdAsync(vaccinationId);
            if (vaccination == null)
                return false;

            vaccination.CompletedDate = completedDate;
            vaccination.Status = "Completed";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Vaccination>> GetVaccinationsByStatusAsync(string status)
        {
            return await _dbSet
                .Where(v => v.Status == status)
                .Include(v => v.Pet)
                .ThenInclude(p => p.Owner)
                .OrderBy(v => v.DueDate)
                .ToListAsync();
        }
    }
}