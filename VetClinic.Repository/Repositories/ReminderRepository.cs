using VetClinic.Repository.DAO;
using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly IReminderDAO _reminderDAO;

        public ReminderRepository(IReminderDAO reminderDAO)
        {
            _reminderDAO = reminderDAO;
        }

        public async Task<Reminder?> GetByIdAsync(int id)
        {
            return await _reminderDAO.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Reminder>> GetAllAsync()
        {
            return await _reminderDAO.GetAllAsync();
        }

        public async Task<Reminder> CreateAsync(Reminder reminder)
        {
            // Business logic validation
            if (reminder.UserId <= 0)
                throw new ArgumentException("Valid user ID is required");

            if (string.IsNullOrWhiteSpace(reminder.Type))
                throw new ArgumentException("Reminder type is required");

            if (string.IsNullOrWhiteSpace(reminder.Title))
                throw new ArgumentException("Reminder title is required");

            if (reminder.SendAt == default)
                throw new ArgumentException("Send date/time is required");

            reminder.CreatedDate = DateTime.Now;
            reminder.Status = "Pending";
            return await _reminderDAO.AddAsync(reminder);
        }

        public async Task<Reminder> UpdateAsync(Reminder reminder)
        {
            // Business logic validation
            var existingReminder = await _reminderDAO.GetByIdAsync(reminder.Id);
            if (existingReminder == null)
                throw new ArgumentException("Reminder not found");

            if (string.IsNullOrWhiteSpace(reminder.Type))
                throw new ArgumentException("Reminder type is required");

            if (string.IsNullOrWhiteSpace(reminder.Title))
                throw new ArgumentException("Reminder title is required");

            return await _reminderDAO.UpdateAsync(reminder);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _reminderDAO.DeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _reminderDAO.ExistsAsync(id);
        }

        public async Task<IEnumerable<Reminder>> GetRemindersByUserAsync(int userId)
        {
            return await _reminderDAO.GetRemindersByUserAsync(userId);
        }

        public async Task<IEnumerable<Reminder>> GetPendingRemindersAsync()
        {
            return await _reminderDAO.GetPendingRemindersAsync();
        }

        public async Task<IEnumerable<Reminder>> GetRemindersToSendAsync(DateTime currentTime)
        {
            return await _reminderDAO.GetRemindersToSendAsync(currentTime);
        }

        public async Task<IEnumerable<Reminder>> GetRemindersByTypeAsync(string type)
        {
            return await _reminderDAO.GetRemindersByTypeAsync(type);
        }

        public async Task<bool> MarkReminderSentAsync(int reminderId)
        {
            return await _reminderDAO.MarkReminderSentAsync(reminderId);
        }

        public async Task<bool> MarkReminderFailedAsync(int reminderId)
        {
            return await _reminderDAO.MarkReminderFailedAsync(reminderId);
        }

        public async Task<IEnumerable<Reminder>> GetUpcomingRemindersAsync(int days = 7)
        {
            return await _reminderDAO.GetUpcomingRemindersAsync(days);
        }
    }
}