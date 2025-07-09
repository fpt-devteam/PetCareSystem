using VetClinic.Repository.Entities;

namespace VetClinic.Repository.Repositories
{
    public interface IReminderRepository
    {
        Task<Reminder?> GetByIdAsync(int id);
        Task<IEnumerable<Reminder>> GetAllAsync();
        Task<Reminder> CreateAsync(Reminder reminder);
        Task<Reminder> UpdateAsync(Reminder reminder);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // Reminder-specific methods
        Task<IEnumerable<Reminder>> GetRemindersByUserAsync(int userId);
        Task<IEnumerable<Reminder>> GetPendingRemindersAsync();
        Task<IEnumerable<Reminder>> GetRemindersToSendAsync(DateTime currentTime);
        Task<IEnumerable<Reminder>> GetRemindersByTypeAsync(string type);
        Task<bool> MarkReminderSentAsync(int reminderId);
        Task<bool> MarkReminderFailedAsync(int reminderId);
        Task<IEnumerable<Reminder>> GetUpcomingRemindersAsync(int days = 7);
    }
}