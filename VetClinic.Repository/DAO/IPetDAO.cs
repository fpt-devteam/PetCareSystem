using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public interface IPetDAO : IBaseDAO<Pet>
    {
        Task<IEnumerable<Pet>> GetPetsByOwnerIdAsync(int ownerId);
        Task<IEnumerable<Pet>> GetPetsWithAppointmentsAsync();
        Task<Pet?> GetPetWithOwnerAsync(int petId);
        Task<IEnumerable<Pet>> GetPetsBySpeciesAsync(string species);
        Task<IEnumerable<Pet>> GetPetsForVaccinationAsync();
    }
}