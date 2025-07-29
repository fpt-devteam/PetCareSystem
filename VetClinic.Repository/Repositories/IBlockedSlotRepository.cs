using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public interface IBlockedSlotRepository
    {
        Task<BlockedSlot?> GetByIdAsync(int id);
        Task<IEnumerable<BlockedSlot>> GetAllAsync();
        Task<BlockedSlot> CreateAsync(BlockedSlot blockedSlot);
        Task<BlockedSlot> UpdateAsync(BlockedSlot blockedSlot);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        
        // Business-specific methods
        Task<IEnumerable<BlockedSlot>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<BlockedSlot>> GetByDoctorAndDateRangeAsync(int doctorId, DateTime startDate, DateTime endDate);
        Task<bool> IsTimeSlotBlockedAsync(int doctorId, DateTime startTime, DateTime endTime);
        Task<IEnumerable<BlockedSlot>> GetActiveBlockedSlotsAsync(int doctorId, DateTime date);
    }
}
