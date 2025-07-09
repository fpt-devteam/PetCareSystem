using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public interface IPetRepository
    {
        Task<Pet?> GetByIdAsync(int id);
        Task<IEnumerable<Pet>> GetAllAsync();
        Task<Pet> CreateAsync(Pet pet);
        Task<Pet> UpdateAsync(Pet pet);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Pet-specific methods
        Task<IEnumerable<Pet>> GetPetsByOwnerIdAsync(int ownerId);
        Task<Pet?> GetPetWithOwnerAsync(int petId);
        Task<IEnumerable<Pet>> GetPetsBySpeciesAsync(string species);
        Task<IEnumerable<Pet>> GetPetsWithAppointmentsAsync();
        Task<IEnumerable<Pet>> GetPetsForVaccinationAsync();
    }
}