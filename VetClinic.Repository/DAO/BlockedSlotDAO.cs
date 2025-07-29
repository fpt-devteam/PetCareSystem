using Microsoft.EntityFrameworkCore;
using VetClinic.Repository.Data;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public class BlockedSlotDAO : BaseDAO<BlockedSlot>, IBlockedSlotDAO
    {
        public BlockedSlotDAO(VetClinicDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BlockedSlot>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.Set<BlockedSlot>()
                .Include(bs => bs.Doctor)
                .Where(bs => bs.DoctorId == doctorId && bs.IsActive)
                .OrderBy(bs => bs.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlockedSlot>> GetByDoctorAndDateRangeAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            return await _context.Set<BlockedSlot>()
                .Include(bs => bs.Doctor)
                .Where(bs => bs.DoctorId == doctorId 
                    && bs.IsActive
                    && bs.StartTime.Date >= startDate.Date 
                    && bs.StartTime.Date <= endDate.Date)
                .OrderBy(bs => bs.StartTime)
                .ToListAsync();
        }

        public async Task<bool> IsTimeSlotBlockedAsync(int doctorId, DateTime startTime, DateTime endTime)
        {
            return await _context.Set<BlockedSlot>()
                .AnyAsync(bs => bs.DoctorId == doctorId 
                    && bs.IsActive
                    && ((bs.StartTime <= startTime && bs.EndTime > startTime) ||
                        (bs.StartTime < endTime && bs.EndTime >= endTime) ||
                        (bs.StartTime >= startTime && bs.EndTime <= endTime)));
        }

        public async Task<IEnumerable<BlockedSlot>> GetActiveBlockedSlotsAsync(int doctorId, DateTime date)
        {
            return await _context.Set<BlockedSlot>()
                .Include(bs => bs.Doctor)
                .Where(bs => bs.DoctorId == doctorId 
                    && bs.IsActive
                    && bs.StartTime.Date == date.Date)
                .OrderBy(bs => bs.StartTime)
                .ToListAsync();
        }
    }
}
