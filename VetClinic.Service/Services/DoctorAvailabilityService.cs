using VetClinic.Repository.Repositories;
using VetClinic.Service.Interfaces;

namespace VetClinic.Service.Services
{
    public class DoctorAvailabilityService : IDoctorAvailabilityService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IBlockedSlotRepository _blockedSlotRepository;

        public DoctorAvailabilityService(IAppointmentRepository appointmentRepository, IBlockedSlotRepository blockedSlotRepository)
        {
            _appointmentRepository = appointmentRepository;
            _blockedSlotRepository = blockedSlotRepository;
        }

        public async Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime startTime, DateTime endTime)
        {
            // Check appointment conflicts
            var durationMinutes = (int)(endTime - startTime).TotalMinutes;
            var hasAppointmentConflict = await _appointmentRepository.IsDoctorAvailableAsync(doctorId, startTime, durationMinutes);
            if (!hasAppointmentConflict)
                return false;

            // Check blocked slots conflicts
            var hasBlockedSlotConflict = await _blockedSlotRepository.IsTimeSlotBlockedAsync(doctorId, startTime, endTime);
            if (hasBlockedSlotConflict)
                return false;

            return true;
        }

        public async Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime appointmentTime, int durationMinutes)
        {
            var endTime = appointmentTime.AddMinutes(durationMinutes);
            return await IsDoctorAvailableAsync(doctorId, appointmentTime, endTime);
        }

        public async Task<string?> GetAvailabilityReasonAsync(int doctorId, DateTime startTime, DateTime endTime)
        {
            // Check appointment conflicts first
            var durationMinutes = (int)(endTime - startTime).TotalMinutes;
            var hasAppointmentConflict = await _appointmentRepository.IsDoctorAvailableAsync(doctorId, startTime, durationMinutes);
            if (!hasAppointmentConflict)
                return "Doctor has another appointment at this time";

            // Check blocked slots conflicts
            var hasBlockedSlotConflict = await _blockedSlotRepository.IsTimeSlotBlockedAsync(doctorId, startTime, endTime);
            if (hasBlockedSlotConflict)
                return "Doctor is not available due to personal schedule";

            return null; // Available
        }
    }
}
