using VetClinic.Repository.Entities;

namespace VetClinic.Service.Interfaces
{
    public interface IBlockedSlotService
    {
        Task<BlockedSlot?> GetBlockedSlotByIdAsync(int id);
        Task<IEnumerable<BlockedSlot>> GetAllBlockedSlotsAsync();
        Task<BlockedSlot> CreateBlockedSlotAsync(BlockedSlot blockedSlot);
        Task<BlockedSlot> UpdateBlockedSlotAsync(BlockedSlot blockedSlot);
        Task<bool> DeleteBlockedSlotAsync(int id);
        Task<bool> BlockedSlotExistsAsync(int id);
        
        // Business-specific methods
        Task<IEnumerable<BlockedSlot>> GetDoctorBlockedSlotsAsync(int doctorId);
        Task<IEnumerable<BlockedSlot>> GetDoctorBlockedSlotsInRangeAsync(int doctorId, DateTime startDate, DateTime endDate);
        Task<bool> IsDoctorTimeSlotBlockedAsync(int doctorId, DateTime startTime, DateTime endTime);
        Task<IEnumerable<BlockedSlot>> GetDoctorDailyBlockedSlotsAsync(int doctorId, DateTime date);
        
        // Convenience methods
        Task<BlockedSlot> CreateBlockedSlotAsync(int doctorId, DateTime startTime, DateTime endTime, string blockType, string? reason = null);
        Task<bool> UnblockTimeSlotAsync(int doctorId, int blockedSlotId);
    }
}
