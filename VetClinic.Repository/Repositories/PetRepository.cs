using VetClinic.Repository.DAO;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly IPetDAO _petDAO;

        public PetRepository(IPetDAO petDAO)
        {
            _petDAO = petDAO;
        }

        public async Task<Pet?> GetByIdAsync(int id)
        {
            return await _petDAO.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Pet>> GetAllAsync()
        {
            return await _petDAO.GetAllAsync();
        }

        public async Task<Pet> CreateAsync(Pet pet)
        {
            // Business logic validation
            if (string.IsNullOrWhiteSpace(pet.Name))
                throw new ArgumentException("Pet name is required");

            if (string.IsNullOrWhiteSpace(pet.Species))
                throw new ArgumentException("Pet species is required");

            if (pet.OwnerId <= 0)
                throw new ArgumentException("Valid owner ID is required");

            pet.CreatedDate = DateTime.Now;
            return await _petDAO.AddAsync(pet);
        }

        public async Task<Pet> UpdateAsync(Pet pet)
        {
            // Business logic validation
            var existingPet = await _petDAO.GetByIdAsync(pet.Id);
            if (existingPet == null)
                throw new ArgumentException("Pet not found");

            if (string.IsNullOrWhiteSpace(pet.Name))
                throw new ArgumentException("Pet name is required");

            if (string.IsNullOrWhiteSpace(pet.Species))
                throw new ArgumentException("Pet species is required");

            // Update only the properties that should be updated
            existingPet.Name = pet.Name;
            existingPet.Species = pet.Species;
            existingPet.Breed = pet.Breed;
            existingPet.BirthDate = pet.BirthDate;
            existingPet.Gender = pet.Gender;
            existingPet.Weight = pet.Weight;
            existingPet.Color = pet.Color;
            existingPet.MicrochipId = pet.MicrochipId;
            existingPet.MedicalNotes = pet.MedicalNotes;
            existingPet.IsActive = pet.IsActive;
            existingPet.PhotoUrl = pet.PhotoUrl;

            return await _petDAO.UpdateAsync(existingPet);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _petDAO.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _petDAO.ExistsAsync(id);
        }

        public async Task<IEnumerable<Pet>> GetPetsByOwnerIdAsync(int ownerId)
        {
            return await _petDAO.GetPetsByOwnerIdAsync(ownerId);
        }

        public async Task<Pet?> GetPetWithOwnerAsync(int petId)
        {
            return await _petDAO.GetPetWithOwnerAsync(petId);
        }

        public async Task<IEnumerable<Pet>> GetPetsBySpeciesAsync(string species)
        {
            return await _petDAO.GetPetsBySpeciesAsync(species);
        }

        public async Task<IEnumerable<Pet>> GetPetsWithAppointmentsAsync()
        {
            return await _petDAO.GetPetsWithAppointmentsAsync();
        }

        public async Task<IEnumerable<Pet>> GetPetsForVaccinationAsync()
        {
            return await _petDAO.GetPetsForVaccinationAsync();
        }
    }
}