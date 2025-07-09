using VetClinic.Repository.Entities;

namespace VetClinic.Repository.DAO
{
    public interface IReminderDAO : IBaseDAO<Reminder>
    {
        Task<IEnumerable<Reminder>> GetRemindersByUserAsync(int userId);
        Task<IEnumerable<Reminder>> GetPendingRemindersAsync();
        Task<IEnumerable<Reminder>> GetRemindersToSendAsync(DateTime currentTime);
        Task<IEnumerable<Reminder>> GetRemindersByTypeAsync(string type);
        Task<bool> MarkReminderSentAsync(int reminderId);
        Task<bool> MarkReminderFailedAsync(int reminderId);
        Task<IEnumerable<Reminder>> GetUpcomingRemindersAsync(int days = 7);
    }
}