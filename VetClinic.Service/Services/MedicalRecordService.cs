using VetClinic.Repository.Entities;
using VetClinic.Repository.Repositories;
using VetClinic.Service.Interfaces;

namespace VetClinic.Service.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPetRepository _petRepository;
        private readonly IUserRepository _userRepository;

        public MedicalRecordService(
            IMedicalRecordRepository medicalRecordRepository,
            IAppointmentRepository appointmentRepository,
            IPetRepository petRepository,
            IUserRepository userRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _appointmentRepository = appointmentRepository;
            _petRepository = petRepository;
            _userRepository = userRepository;
        }

        public async Task<MedicalRecord?> GetMedicalRecordByIdAsync(int id)
        {
            return await _medicalRecordRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<MedicalRecord>> GetAllMedicalRecordsAsync()
        {
            return await _medicalRecordRepository.GetAllAsync();
        }

        public async Task<MedicalRecord> CreateMedicalRecordAsync(MedicalRecord medicalRecord)
        {
            // Business validation
            if (medicalRecord.AppointmentId <= 0)
                throw new ArgumentException("Valid appointment ID is required");

            if (medicalRecord.PetId <= 0)
                throw new ArgumentException("Valid pet ID is required");

            if (medicalRecord.DoctorId <= 0)
                throw new ArgumentException("Valid doctor ID is required");

            // Verify entities exist
            var appointment = await _appointmentRepository.GetByIdAsync(medicalRecord.AppointmentId);
            if (appointment == null)
                throw new ArgumentException("Appointment not found");

            var pet = await _petRepository.GetByIdAsync(medicalRecord.PetId);
            if (pet == null)
                throw new ArgumentException("Pet not found");

            var doctor = await _userRepository.GetByIdAsync(medicalRecord.DoctorId);
            if (doctor == null || doctor.Role != "Doctor")
                throw new ArgumentException("Doctor not found");

            return await _medicalRecordRepository.CreateAsync(medicalRecord);
        }

        public async Task<MedicalRecord> UpdateMedicalRecordAsync(MedicalRecord medicalRecord)
        {
            return await _medicalRecordRepository.UpdateAsync(medicalRecord);
        }

        public async Task<bool> DeleteMedicalRecordAsync(int id)
        {
            return await _medicalRecordRepository.DeleteAsync(id);
        }

        public async Task<bool> MedicalRecordExistsAsync(int id)
        {
            return await _medicalRecordRepository.ExistsAsync(id);
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByPetAsync(int petId)
        {
            return await _medicalRecordRepository.GetMedicalRecordsByPetAsync(petId);
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDoctorAsync(int doctorId)
        {
            return await _medicalRecordRepository.GetMedicalRecordsByDoctorAsync(doctorId);
        }

        public async Task<MedicalRecord?> GetMedicalRecordByAppointmentAsync(int appointmentId)
        {
            return await _medicalRecordRepository.GetMedicalRecordByAppointmentAsync(appointmentId);
        }

        public async Task<IEnumerable<MedicalRecord>> GetMedicalRecordsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _medicalRecordRepository.GetMedicalRecordsByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<MedicalRecord>> GetPetMedicalHistoryAsync(int petId)
        {
            return await _medicalRecordRepository.GetMedicalRecordsByPetAsync(petId);
        }

        public async Task<bool> CanUserAccessMedicalRecordAsync(int userId, int medicalRecordId, string userRole)
        {
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(medicalRecordId);
            if (medicalRecord == null) return false;

            // Admins and managers can access all medical records
            if (userRole == "Admin" || userRole == "Manager") return true;

            // Doctors can access medical records they created
            if (userRole == "Doctor" && medicalRecord.DoctorId == userId) return true;

            // Customers can access medical records for their pets
            if (userRole == "Customer")
            {
                var pet = await _petRepository.GetByIdAsync(medicalRecord.PetId);
                return pet != null && pet.OwnerId == userId;
            }

            // Staff can access all medical records
            if (userRole == "Staff") return true;

            return false;
        }

        public async Task<MedicalRecord> CreateMedicalRecordFromAppointmentAsync(int appointmentId, string diagnosis, string treatmentNotes, string? prescription = null)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment == null)
                throw new ArgumentException("Appointment not found");

            var medicalRecord = new MedicalRecord
            {
                AppointmentId = appointmentId,
                PetId = appointment.PetId,
                DoctorId = appointment.DoctorId,
                VisitDate = appointment.AppointmentTime,
                Diagnosis = diagnosis,
                TreatmentNotes = treatmentNotes,
                Prescription = prescription
            };

            return await CreateMedicalRecordAsync(medicalRecord);
        }
    }
}