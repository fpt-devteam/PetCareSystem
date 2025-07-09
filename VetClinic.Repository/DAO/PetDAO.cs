using Microsoft.EntityFrameworkCore;
using VetClinic.Repository.Data;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public class PetDAO : BaseDAO<Pet>, IPetDAO
    {
        public PetDAO(VetClinicDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Pet>> GetPetsByOwnerIdAsync(int ownerId)
        {
            return await _dbSet
                .Where(p => p.OwnerId == ownerId)
                .Include(p => p.Owner)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pet>> GetPetsWithAppointmentsAsync()
        {
            return await _dbSet
                .Include(p => p.Owner)
                .Include(p => p.Appointments)
                .ThenInclude(a => a.Doctor)
                .Include(p => p.Appointments)
                .ThenInclude(a => a.Service)
                .ToListAsync();
        }

        public async Task<Pet?> GetPetWithOwnerAsync(int petId)
        {
            return await _dbSet
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == petId);
        }

        public async Task<IEnumerable<Pet>> GetPetsBySpeciesAsync(string species)
        {
            return await _dbSet
                .Where(p => p.Species.ToLower() == species.ToLower())
                .Include(p => p.Owner)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pet>> GetPetsForVaccinationAsync()
        {
            return await _dbSet
                .Include(p => p.Owner)
                .Include(p => p.Vaccinations)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}