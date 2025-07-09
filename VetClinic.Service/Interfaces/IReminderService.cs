using VetClinic.Repository.Entities;

namespace VetClinic.Service.Interfaces
{
    public interface IReminderService
    {
        Task<Reminder?> GetReminderByIdAsync(int id);
        Task<IEnumerable<Reminder>> GetAllRemindersAsync();
        Task<Reminder> CreateReminderAsync(Reminder reminder);
        Task<Reminder> UpdateReminderAsync(Reminder reminder);
        Task<bool> DeleteReminderAsync(int id);
        Task<bool> ReminderExistsAsync(int id);

        // Reminder-specific business methods
        Task<IEnumerable<Reminder>> GetRemindersByUserAsync(int userId);
        Task<IEnumerable<Reminder>> GetPendingRemindersAsync();
        Task<IEnumerable<Reminder>> GetRemindersToSendAsync(DateTime currentTime);
        Task<IEnumerable<Reminder>> GetRemindersByTypeAsync(string type);
        Task<bool> MarkReminderSentAsync(int reminderId);
        Task<bool> MarkReminderFailedAsync(int reminderId);
        Task<IEnumerable<Reminder>> GetUpcomingRemindersAsync(int days = 7);
        Task<bool> CanUserAccessReminderAsync(int userId, int reminderId, string userRole);
        Task<Reminder> CreateAppointmentReminderAsync(int appointmentId);
        Task<Reminder> CreateVaccinationReminderAsync(int vaccinationId);
        Task<bool> ProcessPendingRemindersAsync();
    }
}