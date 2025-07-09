using VetClinic.Repository.Entities;
using VetClinic.Repository.Repositories;
using VetClinic.Service.Interfaces;

namespace VetClinic.Service.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;
        private readonly IUserRepository _userRepository;

        public PetService(IPetRepository petRepository, IUserRepository userRepository)
        {
            _petRepository = petRepository;
            _userRepository = userRepository;
        }

        public async Task<Pet?> GetPetByIdAsync(int id)
        {
            return await _petRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Pet>> GetAllPetsAsync()
        {
            return await _petRepository.GetAllAsync();
        }

        public async Task<Pet> CreatePetAsync(Pet pet, int userId)
        {
            // Business validation
            if (pet.OwnerId <= 0)
                throw new ArgumentException("Valid owner ID is required");

            // Verify owner exists
            var owner = await _userRepository.GetByIdAsync(pet.OwnerId);
            if (owner == null || owner.Role != "Customer")
                throw new ArgumentException("Owner not found or is not a customer");

            // Authorization check - customers can only create pets for themselves
            var currentUser = await _userRepository.GetByIdAsync(userId);
            if (currentUser == null)
                throw new UnauthorizedAccessException("User not found");

            if (currentUser.Role == "Customer" && pet.OwnerId != userId)
                throw new UnauthorizedAccessException("You can only create pets for yourself");

            return await _petRepository.CreateAsync(pet);
        }

        public async Task<Pet> UpdatePetAsync(Pet pet)
        {
            return await _petRepository.UpdateAsync(pet);
        }

        public async Task<bool> DeletePetAsync(int id)
        {
            return await _petRepository.DeleteAsync(id);
        }

        public async Task<bool> PetExistsAsync(int id)
        {
            return await _petRepository.ExistsAsync(id);
        }

        public async Task<IEnumerable<Pet>> GetPetsByOwnerIdAsync(int ownerId)
        {
            return await _petRepository.GetPetsByOwnerIdAsync(ownerId);
        }

        public async Task<Pet?> GetPetWithOwnerAsync(int petId)
        {
            return await _petRepository.GetPetWithOwnerAsync(petId);
        }

        public async Task<IEnumerable<Pet>> GetPetsBySpeciesAsync(string species)
        {
            return await _petRepository.GetPetsBySpeciesAsync(species);
        }

        public async Task<IEnumerable<Pet>> GetPetsWithAppointmentsAsync()
        {
            return await _petRepository.GetPetsWithAppointmentsAsync();
        }

        public async Task<IEnumerable<Pet>> GetPetsForVaccinationAsync()
        {
            return await _petRepository.GetPetsForVaccinationAsync();
        }

        public async Task<bool> CanUserAccessPetAsync(int userId, int petId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            // Admins and managers can access all pets
            if (user.Role == "Admin" || user.Role == "Manager") return true;

            // Customers can only access their own pets
            if (user.Role == "Customer")
            {
                var pet = await _petRepository.GetByIdAsync(petId);
                return pet != null && pet.OwnerId == userId;
            }

            // Doctors and staff can access all pets for professional purposes
            if (user.Role == "Doctor" || user.Role == "Staff") return true;

            return false;
        }
    }
}