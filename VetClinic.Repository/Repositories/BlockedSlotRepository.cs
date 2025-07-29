using VetClinic.Repository.DAO;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public class BlockedSlotRepository : IBlockedSlotRepository
    {
        private readonly IBlockedSlotDAO _blockedSlotDAO;

        public BlockedSlotRepository(IBlockedSlotDAO blockedSlotDAO)
        {
            _blockedSlotDAO = blockedSlotDAO;
        }

        public async Task<BlockedSlot?> GetByIdAsync(int id)
        {
            return await _blockedSlotDAO.GetByIdAsync(id);
        }

        public async Task<IEnumerable<BlockedSlot>> GetAllAsync()
        {
            return await _blockedSlotDAO.GetAllAsync();
        }

        public async Task<BlockedSlot> CreateAsync(BlockedSlot blockedSlot)
        {
            // Business logic validation
            if (blockedSlot.StartTime >= blockedSlot.EndTime)
                throw new ArgumentException("End time must be after start time");

            if (blockedSlot.StartTime < DateTime.Now.AddMinutes(-5)) // Allow 5 minutes buffer
                throw new ArgumentException("Cannot block time slots in the past");

            var duration = blockedSlot.EndTime - blockedSlot.StartTime;
            if (duration.TotalMinutes < 30)
                throw new ArgumentException("Minimum block duration is 30 minutes");

            if (duration.TotalHours > 12)
                throw new ArgumentException("Maximum block duration is 12 hours");

            // Check for overlapping blocked slots
            var isBlocked = await _blockedSlotDAO.IsTimeSlotBlockedAsync(
                blockedSlot.DoctorId, 
                blockedSlot.StartTime, 
                blockedSlot.EndTime);

            if (isBlocked)
                throw new ArgumentException("Time slot overlaps with existing blocked slot");

            blockedSlot.CreatedDate = DateTime.Now;
            return await _blockedSlotDAO.AddAsync(blockedSlot);
        }

        public async Task<BlockedSlot> UpdateAsync(BlockedSlot blockedSlot)
        {
            var existingSlot = await _blockedSlotDAO.GetByIdAsync(blockedSlot.Id);
            if (existingSlot == null)
                throw new ArgumentException("Blocked slot not found");

            // Business logic validation (same as create)
            if (blockedSlot.StartTime >= blockedSlot.EndTime)
                throw new ArgumentException("End time must be after start time");

            return await _blockedSlotDAO.UpdateAsync(blockedSlot);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _blockedSlotDAO.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _blockedSlotDAO.ExistsAsync(id);
        }

        public async Task<IEnumerable<BlockedSlot>> GetByDoctorIdAsync(int doctorId)
        {
            return await _blockedSlotDAO.GetByDoctorIdAsync(doctorId);
        }

        public async Task<IEnumerable<BlockedSlot>> GetByDoctorAndDateRangeAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            return await _blockedSlotDAO.GetByDoctorAndDateRangeAsync(doctorId, startDate, endDate);
        }

        public async Task<bool> IsTimeSlotBlockedAsync(int doctorId, DateTime startTime, DateTime endTime)
        {
            return await _blockedSlotDAO.IsTimeSlotBlockedAsync(doctorId, startTime, endTime);
        }

        public async Task<IEnumerable<BlockedSlot>> GetActiveBlockedSlotsAsync(int doctorId, DateTime date)
        {
            return await _blockedSlotDAO.GetActiveBlockedSlotsAsync(doctorId, date);
        }
    }
}
