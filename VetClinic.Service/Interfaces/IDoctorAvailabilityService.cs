using VetClinic.Service.Interfaces;

namespace VetClinic.Service.Interfaces
{
    public interface IDoctorAvailabilityService
    {
        Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime startTime, DateTime endTime);
        Task<bool> IsDoctorAvailableAsync(int doctorId, DateTime appointmentTime, int durationMinutes);
        Task<string?> GetAvailabilityReasonAsync(int doctorId, DateTime startTime, DateTime endTime);
    }
}
