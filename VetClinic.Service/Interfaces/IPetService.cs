using VetClinic.Repository.Entities;

namespace VetClinic.Service.Interfaces
{
    public interface IPetService
    {
        Task<Pet?> GetPetByIdAsync(int id);
        Task<IEnumerable<Pet>> GetAllPetsAsync();
        Task<Pet> CreatePetAsync(Pet pet, int userId);
        Task<Pet> UpdatePetAsync(Pet pet);
        Task<bool> DeletePetAsync(int id);
        Task<bool> PetExistsAsync(int id);

        // Pet-specific business methods
        Task<IEnumerable<Pet>> GetPetsByOwnerIdAsync(int ownerId);
        Task<Pet?> GetPetWithOwnerAsync(int petId);
        Task<IEnumerable<Pet>> GetPetsBySpeciesAsync(string species);
        Task<IEnumerable<Pet>> GetPetsWithAppointmentsAsync();
        Task<IEnumerable<Pet>> GetPetsForVaccinationAsync();
        Task<bool> CanUserAccessPetAsync(int userId, int petId);
    }
}