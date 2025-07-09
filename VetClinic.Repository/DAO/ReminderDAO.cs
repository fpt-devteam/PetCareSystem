using Microsoft.EntityFrameworkCore;
using VetClinic.Repository.Data;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public class ReminderDAO : BaseDAO<Reminder>, IReminderDAO
    {
        public ReminderDAO(VetClinicDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Reminder>> GetRemindersByUserAsync(int userId)
        {
            return await _dbSet
                .Where(r => r.UserId == userId)
                .Include(r => r.Pet)
                .OrderByDescending(r => r.SendAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reminder>> GetPendingRemindersAsync()
        {
            return await _dbSet
                .Where(r => r.Status == "Pending")
                .Include(r => r.User)
                .Include(r => r.Pet)
                .OrderBy(r => r.SendAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reminder>> GetRemindersToSendAsync(DateTime currentTime)
        {
            return await _dbSet
                .Where(r => r.Status == "Pending" && r.SendAt <= currentTime)
                .Include(r => r.User)
                .Include(r => r.Pet)
                .OrderBy(r => r.SendAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reminder>> GetRemindersByTypeAsync(string type)
        {
            return await _dbSet
                .Where(r => r.Type == type)
                .Include(r => r.User)
                .Include(r => r.Pet)
                .OrderByDescending(r => r.SendAt)
                .ToListAsync();
        }

        public async Task<bool> MarkReminderSentAsync(int reminderId)
        {
            var reminder = await GetByIdAsync(reminderId);
            if (reminder == null)
                return false;

            reminder.Status = "Sent";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkReminderFailedAsync(int reminderId)
        {
            var reminder = await GetByIdAsync(reminderId);
            if (reminder == null)
                return false;

            reminder.Status = "Failed";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Reminder>> GetUpcomingRemindersAsync(int days = 7)
        {
            var endDate = DateTime.Now.AddDays(days);
            return await _dbSet
                .Where(r => r.Status == "Pending" && r.SendAt >= DateTime.Now && r.SendAt <= endDate)
                .Include(r => r.User)
                .Include(r => r.Pet)
                .OrderBy(r => r.SendAt)
                .ToListAsync();
        }
    }
}