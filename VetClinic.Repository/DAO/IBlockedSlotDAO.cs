using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public interface IBlockedSlotDAO : IBaseDAO<BlockedSlot>
    {
        Task<IEnumerable<BlockedSlot>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<BlockedSlot>> GetByDoctorAndDateRangeAsync(int doctorId, DateTime startDate, DateTime endDate);
        Task<bool> IsTimeSlotBlockedAsync(int doctorId, DateTime startTime, DateTime endTime);
        Task<IEnumerable<BlockedSlot>> GetActiveBlockedSlotsAsync(int doctorId, DateTime date);
    }
}
