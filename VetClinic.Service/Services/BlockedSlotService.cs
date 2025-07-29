using VetClinic.Repository.Entities;
using VetClinic.Repository.Repositories;
using VetClinic.Service.Interfaces;

namespace VetClinic.Service.Services
{
    public class BlockedSlotService : IBlockedSlotService
    {
        private readonly IBlockedSlotRepository _blockedSlotRepository;

        public BlockedSlotService(IBlockedSlotRepository blockedSlotRepository)
        {
            _blockedSlotRepository = blockedSlotRepository;
        }

        public async Task<BlockedSlot?> GetBlockedSlotByIdAsync(int id)
        {
            return await _blockedSlotRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<BlockedSlot>> GetAllBlockedSlotsAsync()
        {
            return await _blockedSlotRepository.GetAllAsync();
        }

        public async Task<BlockedSlot> CreateBlockedSlotAsync(BlockedSlot blockedSlot)
        {
            // Note: Appointment conflict validation should be done at a higher level 
            // (e.g., in DoctorAvailabilityService) to avoid circular dependencies
            return await _blockedSlotRepository.CreateAsync(blockedSlot);
        }

        public async Task<BlockedSlot> UpdateBlockedSlotAsync(BlockedSlot blockedSlot)
        {
            return await _blockedSlotRepository.UpdateAsync(blockedSlot);
        }

        public async Task<bool> DeleteBlockedSlotAsync(int id)
        {
            return await _blockedSlotRepository.DeleteAsync(id);
        }

        public async Task<bool> BlockedSlotExistsAsync(int id)
        {
            return await _blockedSlotRepository.ExistsAsync(id);
        }

        public async Task<IEnumerable<BlockedSlot>> GetDoctorBlockedSlotsAsync(int doctorId)
        {
            return await _blockedSlotRepository.GetByDoctorIdAsync(doctorId);
        }

        public async Task<IEnumerable<BlockedSlot>> GetDoctorBlockedSlotsInRangeAsync(int doctorId, DateTime startDate, DateTime endDate)
        {
            return await _blockedSlotRepository.GetByDoctorAndDateRangeAsync(doctorId, startDate, endDate);
        }

        public async Task<bool> IsDoctorTimeSlotBlockedAsync(int doctorId, DateTime startTime, DateTime endTime)
        {
            return await _blockedSlotRepository.IsTimeSlotBlockedAsync(doctorId, startTime, endTime);
        }

        public async Task<IEnumerable<BlockedSlot>> GetDoctorDailyBlockedSlotsAsync(int doctorId, DateTime date)
        {
            return await _blockedSlotRepository.GetActiveBlockedSlotsAsync(doctorId, date);
        }

        public async Task<BlockedSlot> CreateBlockedSlotAsync(int doctorId, DateTime startTime, DateTime endTime, string blockType, string? reason = null)
        {
            var blockedSlot = new BlockedSlot
            {
                DoctorId = doctorId,
                StartTime = startTime,
                EndTime = endTime,
                BlockType = blockType,
                Reason = reason,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            return await CreateBlockedSlotAsync(blockedSlot);
        }

        public async Task<bool> UnblockTimeSlotAsync(int doctorId, int blockedSlotId)
        {
            var blockedSlot = await _blockedSlotRepository.GetByIdAsync(blockedSlotId);
            if (blockedSlot == null || blockedSlot.DoctorId != doctorId)
            {
                return false;
            }

            return await _blockedSlotRepository.DeleteAsync(blockedSlotId);
        }

        // Note: Appointment conflict validation moved to DoctorAvailabilityService
        // to avoid circular dependencies between AppointmentService and BlockedSlotService
    }
}
